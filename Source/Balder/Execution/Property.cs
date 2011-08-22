#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
using System.Linq.Expressions;
using Balder.Rendering;
#if(XAML)
using System.ComponentModel;
using System.Windows;
#endif

namespace Balder.Execution
{
	public class Property<T,TP> : IProperty
#if(XAML)
		where T:DependencyObject, IHavePropertyContainer
#else
		where T:IHavePropertyContainer
#endif

	{
		private readonly PropertyDescriptor _propertyDescriptor;
		private readonly Dictionary<object, ObjectProperty<TP>> _objectPropertyBag;
		private readonly TP _defaultValue;
		private readonly PropertyValueChanged<TP> _propertyValueChanged;

		private Property(Expression<Func<T, TP>> expression, TP defaultValue, PropertyValueChanged<TP> propertyValueChanged)
		{
			_propertyDescriptor = new PropertyDescriptor(typeof(T), typeof(TP), expression);
			_objectPropertyBag = new Dictionary<object, ObjectProperty<TP>>();
			_defaultValue = defaultValue;
			_propertyValueChanged = propertyValueChanged;
			Initialize();
		}

		#region Register
		public static Property<T, TP> Register(Expression<Func<T, TP>> expression)
		{
			var property = Register(expression, default(TP));
			return property;
		}

		public static Property<T, TP> Register(Expression<Func<T, TP>> expression, PropertyValueChanged<TP> propertyValueChanged)
		{
			var property = Register(expression, propertyValueChanged, default(TP));
			return property;
		}

		public static Property<T, TP> Register(Expression<Func<T, TP>> expression, TP defaultValue)
		{
			var property = Register(expression, null, defaultValue);
			return property;
		}

		public static Property<T, TP> Register(Expression<Func<T, TP>> expression, PropertyValueChanged<TP> propertyValueChanged, TP defaultValue)
		{
			var property = new Property<T, TP>(expression, defaultValue, propertyValueChanged);
			return property;
		}
		#endregion

		public TP GetValue(T obj)
		{
			var objectProperty = GetObjectProperty(obj);
			return objectProperty.Value;
		}

		public void SetValue(T obj, TP value)
		{
			var objectProperty = GetObjectProperty(obj);

			HandleRuntimeContext(obj, objectProperty, value);
			var causesChange = objectProperty.DoesValueCauseChange(value);
			if (causesChange)
			{
				objectProperty.SignalRendering();
				CleanupChildren(objectProperty.Value);
			}
			if (objectProperty.CallFromExternal)
			{
				return;
			}

			objectProperty.CallFromProperty = true;
			if (causesChange)
			{
				HandleSetValue(obj, objectProperty, value);
			}
			objectProperty.CallFromProperty = false;
		}

		public void SetRuntimeContext(object obj, IRuntimeContext runtimeContext)
		{
			if (null == obj)
			{
				return;
			}
			var objectProperty = GetObjectProperty((T)obj);
			if (null != objectProperty.RuntimeContext)
			{
				return;
			}
			objectProperty.RuntimeContext = runtimeContext;
		}

		public void RemoveObjectProperties(object obj)
		{
			var identifier = GetObjectIdentifier((T)obj);
			if (_objectPropertyBag.ContainsKey(identifier))
			{
				var objectProperty = GetObjectProperty((T)obj);
				CleanupChildren(objectProperty.Value);
				_objectPropertyBag.Remove(identifier);

			}
		}


		public void CleanupChildren(object previousValue)
		{
			if (null == previousValue)
			{
				return;
			}
			foreach (var childProperty in _propertyDescriptor.ChildProperties)
			{
				childProperty.RemoveObjectProperties(previousValue);
			}
		}


		private object GetObjectIdentifier(T obj)
		{
			object key;
			if (_propertyDescriptor.IsUnique)
			{
				key = ((IAmUnique)obj).GetIdentifier();
			} else 
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
				objectProperty = new ObjectProperty<TP>(obj, _propertyDescriptor, _defaultValue, _propertyValueChanged);
				_objectPropertyBag[key] = objectProperty;
			}
			return objectProperty;
		}



		private void SetRuntimeContextOnChildren(TP obj, IRuntimeContext runtimeContext)
		{
			foreach( var childProperty in _propertyDescriptor.ChildProperties )
			{
				childProperty.SetRuntimeContext(obj, runtimeContext);
			}
		}


    	private void HandleSetValue(T obj, ObjectProperty<TP> objectProperty, TP value)
    	{
			var oldValue = objectProperty.Value;
			if (_propertyDescriptor.IsCopyable && objectProperty.Value != null)
			{
				((ICopyable)value).CopyTo(objectProperty.Value);
			}
			else
			{
				objectProperty.Value = value;
				OnSet(obj, value, objectProperty);
			}
    		HandleNotification(obj, value, oldValue);
    	}

    	private void HandleRuntimeContext(T obj, ObjectProperty<TP> objectProperty, TP value)
    	{
    		if (null == objectProperty.RuntimeContext && obj is IHaveRuntimeContext)
    		{
    			var runtimeContext = ((IHaveRuntimeContext)obj).RuntimeContext;
#if(XAML)
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

			if (null != objectProperty.RuntimeContext && !objectProperty.ChildrenRuntimeContextSet)
			{
				SetRuntimeContextOnChildren(value, objectProperty.RuntimeContext);
				objectProperty.ChildrenRuntimeContextSet = true;
			}
    	}

    	private void HandleNotification(T obj, TP newValue, TP oldValue)
		{
			if (_propertyDescriptor.CanNotify)
			{
				((ICanNotifyChanges)obj).Notify(_propertyDescriptor.PropertyInfo.Name, oldValue, newValue);
			}
			if( null != _propertyValueChanged )
			{
				_propertyValueChanged(obj, oldValue, newValue);
			}
		}


#if(XAML)

		public DependencyProperty ActualDependencyProperty { get; private set; }

		private void PropertyChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var objectProperty = GetObjectProperty((T)obj);
			var oldValue = objectProperty.Value;
			var newValue = (TP) e.NewValue;
			var causesChange = objectProperty.DoesValueCauseChange(newValue); 
			if( causesChange &&
				!objectProperty.CallFromProperty)
			{
				HandleNotification((T)obj,newValue,oldValue);
				objectProperty.SignalRendering();
			}

			objectProperty.Value = newValue;

			if (!objectProperty.CallFromProperty)
			{
				objectProperty.CallFromExternal = true;
				_propertyDescriptor.PropertyInfo.SetValue(obj, e.NewValue, null);
				objectProperty.CallFromExternal = false;
			}
		}

		private void Initialize()
		{
			ActualDependencyProperty =
				DependencyProperty.Register(
					_propertyDescriptor.PropertyInfo.Name,
					_propertyDescriptor.PropertyInfo.PropertyType,
					_propertyDescriptor.OwnerType,
					new PropertyMetadata(_defaultValue, PropertyChanged)
				);
		}

		private void OnSet(T obj, TP value, ObjectProperty<TP> objectProperty)
		{
#if(DESKTOP)
			// Todo : WPF
#else
			if( null == Deployment.Current || Deployment.Current.Dispatcher.CheckAccess())
			{
				obj.SetValue(ActualDependencyProperty, value);
			} else
			{
				Deployment.Current.Dispatcher.BeginInvoke(
						() => obj.SetValue(ActualDependencyProperty, value));
			}
#endif
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
