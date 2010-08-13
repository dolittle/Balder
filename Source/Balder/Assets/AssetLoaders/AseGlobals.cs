using Balder.Materials;
using Balder.Math;

namespace Balder.Assets.AssetLoaders
{
	public class AseGlobals
	{
		public Material[] Materials { get; set; }
		public IAssetLoaderService AssetLoaderService { get; set; }
		public string RootPath { get; set; }
		public Matrix CurrentObjectsInvertedMatrix { get; set; }

		public Color[] CurrentObjectVertexColors { get; set; }
	}
}