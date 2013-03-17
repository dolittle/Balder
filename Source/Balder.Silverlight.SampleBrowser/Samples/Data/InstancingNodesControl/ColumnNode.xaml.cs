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
	}
}
