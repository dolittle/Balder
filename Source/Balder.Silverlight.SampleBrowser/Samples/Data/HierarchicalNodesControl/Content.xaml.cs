using System;
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

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			((ViewModel)DataContext).GenerateData();
		}

		private static Random rnd = new Random();
		private void Button_Click_1(object sender, System.Windows.RoutedEventArgs e)
		{
			var viewModel = (ViewModel) DataContext;

			var depth = rnd.Next(10, 20);
			var row = rnd.Next(10, 20);
			var column = rnd.Next(10, 20);
			var newColumn = new Column(depth, row, column);
			viewModel.Depths[0].Rows[0].Columns.Add(newColumn);

		}
	}
}
