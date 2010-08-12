using System;
using Balder.Rendering;

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
    	public IRuntimeContext RuntimeContext;
        private bool _isValueType;
    	public bool ChildrenRuntimeContextSet;


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
			ChildrenRuntimeContextSet = false;
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

		public void SignalRendering()
		{
			if( null != RuntimeContext )
			{
				RuntimeContext.SignalRendering();
			}
		}
    }
}