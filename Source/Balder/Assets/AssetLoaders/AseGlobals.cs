using Balder.Materials;

namespace Balder.Assets.AssetLoaders
{
	public class AseGlobals
	{
		public Material[] Materials { get; set; }
		public IAssetLoaderService AssetLoaderService { get; set; }
		public string RootPath { get; set; }
	}
}