using System;
using System.Collections.Generic;
using System.Reflection;
#if(XAML)
using System.Windows;
#endif

namespace Balder.Execution
{
	public interface IProperty
	{

		Type OwnerType { get; }
		Type PropertyType { get; }
		PropertyInfo PropertyInfo { get; }
		IEnumerable<IProperty> ChildProperties { get; }
		bool IsUnique { get; }
		bool CanNotify { get; }
		bool IsValueType { get; }
		bool IsCopyable { get; }
		object DefaultValue { get; }

		void OnPropertyValueChanged(object sender, object oldValue, object newValue);

#if(XAML)
		bool IsValueNotifyPropertyChanged { get; }
		DependencyProperty ActualDependencyProperty { get; }
#endif
	}
}