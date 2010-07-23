using System;
#if(SILVERLIGHT)
using System.Windows;
#endif

namespace Balder.Execution
{
    public class ObjectProperty<T>
    {
        public T Value;
        public WeakReference Object;
        public bool CallFromExternal;
        public bool CallFromProperty;
        private bool _isValueType;

#if(SILVERLIGHT)
        internal ObjectProperty(DependencyObject obj, bool isValueType, T defaultValue)
#else
		internal ObjectProperty(object obj, bool isValueType, T defaultValue)
#endif
		{
        	Value = defaultValue;
            Object = new WeakReference(obj);
            CallFromExternal = false;
            CallFromProperty = false;
            _isValueType = isValueType;
        }

        public bool DoesValueCauseChange(T value)
        {
            if (!_isValueType)
            {

                if ((null == (object)Value && null != (object)value) ||
                    null == (object)value || !value.Equals(Value))
                {
                    return true;
                }
                return false;
            }
            else
            {
                return !value.Equals(Value);
            }
        }
    }
}