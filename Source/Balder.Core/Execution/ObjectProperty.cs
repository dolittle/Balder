using System;
using System.Windows;

namespace Balder.Core.Execution
{
    public class ObjectProperty<T>
    {
        public T Value;
        public WeakReference Object;
        public bool CallFromExternal;
        public bool CallFromProperty;
        private bool _isValueType;

        internal ObjectProperty(DependencyObject obj, bool isValueType)
        {
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