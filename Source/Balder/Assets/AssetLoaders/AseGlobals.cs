using System.Collections.Generic;
using Balder.Materials;
using Balder.Math;

namespace Balder.Assets.AssetLoaders
{
	public class AseGlobals
	{
		public Dictionary<int, Material> Materials { get; set; }
		public Dictionary<Material, Dictionary<int, Material>> SubMaterials { get; set; }

		public IAssetLoaderService AssetLoaderService { get; set; }
		public string RootPath { get; set; }

		public AseFace[] Faces { get; set; }

		public Matrix CurrentObjectsInvertedMatrix { get; set; }
		public Color[] CurrentObjectVertexColors { get; set; }
		public Material CurrentMaterial { get; set; }

		public int CurrentMaterialRef { get; set; }


		public AseGlobals()
		{
			Materials = new Dictionary<int, Material>();
			SubMaterials = new Dictionary<Material, Dictionary<int, Material>>();
		}
	}
}