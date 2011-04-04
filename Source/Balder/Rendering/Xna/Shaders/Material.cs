using XnaColor = Microsoft.Xna.Framework.Color;

namespace Balder.Rendering.Xna.Shaders
{
	public struct Material
	{
		public XnaColor Ambient;
		public XnaColor Diffuse;
		public XnaColor Specular;
		public float Glossiness;
		public float SpecularLevel;
		public float Opacity;
	}
}