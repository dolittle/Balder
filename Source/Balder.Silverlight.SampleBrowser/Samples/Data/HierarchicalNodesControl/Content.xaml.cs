using System.Windows.Controls;
using Balder.Input;
using Balder.Objects.Geometries;

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

		private void Box_MouseEnter(object sender, MouseEventArgs e)
		{
			var box = sender as Box;
			if( null != box )
			{
				if( null != box.DataContext && box.DataContext is Column )
				{
					var column = box.DataContext as Column;
					column.Box = box;
				}
			}

		}
	}
}
