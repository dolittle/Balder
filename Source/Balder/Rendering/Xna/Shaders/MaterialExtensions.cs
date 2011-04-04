namespace Balder.Rendering.Xna.Shaders
{
	public static class MaterialExtensions
	{
		public static Material ToMaterial(this Materials.Material inputMaterial)
		{
			var material = new Material
							{
								Ambient = inputMaterial.CachedAmbient,
								Diffuse = inputMaterial.CachedDiffuse,
								Specular = inputMaterial.CachedSpecular,
								Glossiness = inputMaterial.GlossinessAsFloat,
								SpecularLevel = inputMaterial.SpecularLevelAsFloat,
								Opacity = 1f
							};

			return material;
		}
	}
}