namespace Balder.Silverlight.SampleBrowser.Samples.Data.InstancingNodesControl
{
	public partial class ColumnNode
	{
		public ColumnNode()
		{
			InitializeComponent();
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
			Position = column.Position;
			Box.IsVisible = column.IsBox;
			Cylinder.IsVisible = column.IsCylinder;
			
			Color = column.Color;
		}
	}
}
