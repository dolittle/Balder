using System.Windows.Controls;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl
{
	public partial class Content : UserControl
	{
		public Content()
		{
			InitializeComponent();
		}

		private void RotationSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			Nodes.Rotation.Y = e.NewValue;

		}
	}
}
