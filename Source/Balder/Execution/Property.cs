#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Balder.Extensions;
using Balder.Rendering;

#if(SILVERLIGHT)
using System.Windows;

#endif

namespace Balder.Execution
{
    public class Property<T,TP>
#if(SILVERLIGHT)
		where T:DependencyObject
#endif
    {
		private readonly bool _canNotify;
		private readonly PropertyInfo _propertyInfo;
		private readonly Dictionary<object, ObjectProperty<TP>> _objectPropertyBag;
		private readonly string _ownerTypeName;
		private readonly TP _defaultValue;
		private readonly Type _ownerType;
		private readonly bool _isValueType;
    	private readonly List<object> _childProperties;

		private Property(Expression<Func<T, TP>> expression, TP defaultValue)
		{
			_ownerType = typeof (T);
			_objectPropertyBag = new Dictionary<object, ObjectProperty<TP>>();
			_ownerTypeName = _ownerType.Name;
			_propertyInfo = expression.GetPropertyInfo();
			_defaultValue = defaultValue;
			_isValueType = typeof(TP).IsValueType;
			_childProperties = new List<object>();
			PopulateChildProperties();

			Initialize();
			
			if ( null != _ownerType.GetInterface(typeof(ICanNotifyChanges).Name,false))
			{
				_canNotify = true;
			} else
			{
				_canNotify = false;
			}
		}

		private void PopulateChildProperties()
		{
			var type = typeof (TP);
			var fields = type.GetFields(BindingFlags.Static | BindingFlags.Public);
			var query = from p in fields
						where p.Name.StartsWith("Property") || p.FieldType.IsGenericType
			            select p;
			foreach( var field in query )
			{
				var property = field.GetValue(null);
				_childProperties.Add(property);
			}
		}

		private object GetObjectIdentifier(T obj)
		{
			object key;
			if (obj is IAmUnique)
			{
				key = ((IAmUnique)obj).GetIdentifier();
			}
			else
			{
				key = obj.GetHashCode();
			}
			return key;
		}

		private ObjectProperty<TP>	GetObjectProperty(T obj)
		{
			var key = GetObjectIdentifier(obj);
			ObjectProperty<TP> objectProperty;
			if( _objectPropertyBag.ContainsKey(key))
			{
				objectProperty = _objectPropertyBag[key];
			} else
			{
				objectProperty = new ObjectProperty<TP>(obj, _isValueType, _defaultValue);
				objectProperty.Value = _defaultValue;
				_objectPropertyBag[key] = objectProperty;
			}
			return objectProperty;
		}


		public static Property<T, TP> Register(Expression<Func<T,TP>> expression)
		{
			var property = Register(expression, default(TP));
			return property;
		}

		public static Property<T, TP> Register(Expression<Func<T,TP>> expression, TP defaultValue)
		{
			var property = new Property<T, TP>(expression, defaultValue);
			return property;
		}

		public void SetRuntimeContext(T obj, IRuntimeContext runtimeContext)
		{
			if( null == obj )
			{
				return;
			}
			var objectProperty = GetObjectProperty(obj);
			if( null != objectProperty.RuntimeContext)
			{
				return;
			}
			objectProperty.RuntimeContext = runtimeContext;
		}

		private void SetRuntimeContextOnChildren(TP obj, IRuntimeContext runtimeContext)
		{
			foreach( var childProperty in _childProperties )
			{
				var method = childProperty.GetType().GetMethod("SetRuntimeContext");
				if( null != method )
				{
					method.Invoke(childProperty, new object[] {obj, runtimeContext});
				}
			}
		}

		public void SetValue(T obj, TP value)
		{
			var objectProperty = GetObjectProperty(obj);

			if (null == objectProperty.RuntimeContext && obj is IHaveRuntimeContext)
			{
				var runtimeContext = ((IHaveRuntimeContext)obj).RuntimeContext;
#if(SILVERLIGHT)
				if (null == runtimeContext && obj is FrameworkElement)
				{
					var frameworkElement = obj as FrameworkElement;
					frameworkElement.Loaded += (s, e) =>
					                                   	{
					                                   		runtimeContext = ((IHaveRuntimeContext) obj).RuntimeContext;
					                                   		SetRuntimeContext(obj, runtimeContext);
															if (null != objectProperty.RuntimeContext && !objectProperty.ChildrenRuntimeContextSet)
															{
																SetRuntimeContextOnChildren(value, objectProperty.RuntimeContext);
																objectProperty.ChildrenRuntimeContextSet = true;
															}
														};
				}
#endif

				if (null != runtimeContext)
				{
					SetRuntimeContext(obj, runtimeContext);
				}

			}

			if( null != objectProperty.RuntimeContext && !objectProperty.ChildrenRuntimeContextSet )
			{
				SetRuntimeContextOnChildren(value, objectProperty.RuntimeContext);
				objectProperty.ChildrenRuntimeContextSet = true;
			}

			objectProperty.SignalRendering();
			if (objectProperty.CallFromExternal)
			{
				return;
			}


			objectProperty.CallFromProperty = true;
			if ( objectProperty.DoesValueCauseChange(value))
			{
				var oldValue = objectProperty.Value;
				objectProperty.Value = value;
				OnSet(obj, value, objectProperty);
				HandleNotification(obj, value, oldValue);
			}
			objectProperty.CallFromProperty = false;
		}


		private void HandleNotification(T obj, TP newValue, TP oldValue)
		{
			if( _canNotify )
			{
				((ICanNotifyChanges)obj).Notify(_propertyInfo.Name,oldValue,newValue);
			}
		}

		public TP GetValue(T obj)
		{
			var objectProperty = GetObjectProperty(obj);
			return objectProperty.Value;
		}

#if(SILVERLIGHT)

		public DependencyProperty ActualDependencyProperty { get; private set; }

		private void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var objectProperty = GetObjectProperty((T)obj);
			var oldValue = objectProperty.Value;
			var newValue = (TP) e.NewValue;
			if( objectProperty.DoesValueCauseChange(newValue) &&
				!objectProperty.CallFromProperty)
			{
				HandleNotification((T)obj,newValue,oldValue);
			}
			objectProperty.Value = newValue;

			if (!objectProperty.CallFromProperty)
			{
				objectProperty.CallFromExternal = true;
				_propertyInfo.SetValue(obj, e.NewValue, null);
				objectProperty.CallFromExternal = false;
			}
		}

		private void Initialize()
		{
			ActualDependencyProperty =
				DependencyProperty.Register(
					_propertyInfo.Name,
					_propertyInfo.PropertyType,
					_ownerType,
					new PropertyMetadata(_defaultValue, PropertyChanged)
				);
		}

		private void OnSet(T obj, TP value, ObjectProperty<TP> objectProperty)
		{
			if( null == Deployment.Current || Deployment.Current.Dispatcher.CheckAccess())
			{
				obj.SetValue(ActualDependencyProperty, value);
			} else
			{
				Deployment.Current.Dispatcher.BeginInvoke(
						() => obj.SetValue(ActualDependencyProperty, value));
			}
		}

#else
		private void Initialize()
		{
			
		}

		private void OnSet(T obj, TP value, ObjectProperty<TP> objectProperty)
		{
			
		}

#endif


	}
}
