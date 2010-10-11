using Balder.Materials;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.InstancingNodesControl
{
	public partial class ColumnNode
	{
		private Material _redMaterial;
		private Material _blueMaterial;


		public ColumnNode()
		{
			InitializeComponent();

			_redMaterial = Material.FromColor(Colors.Red);
			_blueMaterial = Material.FromColor(Colors.Blue);
			Box.Material = _redMaterial;
			Box.IsVisible = true;
		}

		
		public override object DataItem
		{
			get
			{
				return base.DataItem;
			}
			set
			{
				base.DataItem = value;
				if( value is Column )
				{
					SetColumn(value as Column);
				}
			}
		}


		private void SetColumn(Column column)
		{
			//Position = column.Position;
			
			Position.X = column.Position.X;
			Position.Y = column.Position.Y;
			Position.Z = column.Position.Z;
			 
			//Box.IsVisible = column.IsBox;
			//Cylinder.IsVisible = column.IsCylinder;


			//Box.Material = column.IsHovering ? _redMaterial : _blueMaterial;

			//Color = column.Color;
		}
	}
}
