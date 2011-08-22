using Balder.Rendering;
#if(XAML)
using System.Windows;
#endif

namespace Balder.Execution
{
	public interface IProperty
	{
#if(XAML)
		DependencyProperty ActualDependencyProperty { get; }
#endif
		PropertyDescriptor Descriptor { get; }
	}
}