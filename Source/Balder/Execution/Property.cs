using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Balder.Rendering;
using Balder.Extensions;
#if(SILVERLIGHT)
using System.Windows;
using Balder.Extensions.Silverlight;
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

		private Property(Expression<Func<T, TP>> expression, TP defaultValue)
		{
			_ownerType = typeof (T);
			_objectPropertyBag = new Dictionary<object, ObjectProperty<TP>>();
			_ownerTypeName = _ownerType.Name;
			_propertyInfo = expression.GetPropertyInfo();
			_defaultValue = defaultValue;
			_isValueType = typeof(TP).IsValueType;

			Initialize();
			
			if ( null != _ownerType.GetInterface(typeof(ICanNotifyChanges).Name,false))
			{
				_canNotify = true;
			} else
			{
				_canNotify = false;
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

		public void SetValue(T obj, TP value)
		{
			var objectProperty = GetObjectProperty(obj);
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

		private readonly PassiveRenderingSignal _renderSignal = new PassiveRenderingSignal();

		private void HandleNotification(T obj, TP newValue, TP oldValue)
		{
			if( _canNotify )
			{
				((ICanNotifyChanges)obj).Notify(_propertyInfo.Name,oldValue,newValue);
				
			}
			Messenger.DefaultContext.Send(_renderSignal);
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
