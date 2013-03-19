using System;
using System.Windows;
using System.Windows.Input;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;
using MouseEventArgs = Balder.Input.MouseEventArgs;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.InstancingNodesControl
{
	public partial class Content
	{
        Box _box;

		public Content()
		{
			InitializeComponent();

            _box = Game.ContentManager.Creator.CreateGeometry<Box>();
            _box.Dimension = new Coordinate(10, 10, 10);
            _box.Prepare(Game.Viewport);

		}

		private void RotationSlider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			Game.Camera.Position.X = -e.NewValue;
			//Nodes.Rotation.Y = e.NewValue;

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

		private void GenerateDataClicked(object sender, System.Windows.RoutedEventArgs e)
		{
			((ViewModel)DataContext).GenerateData();
		}

		private static Random rnd = new Random();
		private void AddDataClicked(object sender, System.Windows.RoutedEventArgs e)
		{
			var viewModel = (ViewModel) DataContext;

			var depth = rnd.Next(0, ViewModel.DepthCount);
			var row = rnd.Next(0, ViewModel.RowCount);
			var column = rnd.Next(ViewModel.ColumnCount, ViewModel.ColumnCount*2);
			var newColumn = new Column(depth, row, column);
			viewModel.Depths[0].Rows[0].Columns.Add(newColumn);

		}

		private void SwitchObjectClicked(object sender, RoutedEventArgs e)
		{
			var viewModel = (ViewModel)DataContext;

			foreach( var depth in viewModel.Depths )
			{
				foreach( var row in depth.Rows )
				{
					foreach( var column in row.Columns )
					{
						column.IsBox ^= true;
						column.IsCylinder ^= true;
					}
					
				}
				
			}
			
		}

		private void ClearDataClicked(object sender, RoutedEventArgs e)
		{
			((ViewModel) DataContext).Clear();
		}

		private void Nodes_MouseEnter(object sender, MouseEventArgs args)
		{
			Game.Cursor = Cursors.Hand;

		}

		private void InstancingNodes_MouseEnter(object sender, MouseEventArgs args)
		{
			var column = ((Node)args.OriginalSource).DataContext as Column;
			column.IsHovering = true;
		}

		private void InstancingNodes_MouseLeave(object sender, MouseEventArgs args)
		{
            var column = ((Node)args.OriginalSource).DataContext as Column;
			column.IsHovering = false;
		}

		private void Nodes_MouseLeave(object sender, MouseEventArgs args)
		{
			Game.Cursor = Cursors.Arrow;
		}

        private void InstancingNodes_PrepareDataItemInfo(object dataItem, DataItemInfo dataItemInfo)
        {
            var column = dataItem as Column;

            dataItemInfo.Position = column.Position;
            dataItemInfo.Material = Material.FromColor(column.Color);
            dataItemInfo.Node = _box;
        }
	}
}
