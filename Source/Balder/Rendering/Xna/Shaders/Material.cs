using XnaColor = Microsoft.Xna.Framework.Color;

namespace Balder.Rendering.Xna.Shaders
{
	public struct Material
	{
		public static Material Default = new Material
		                                 	{
		                                 		Ambient = XnaColor.Black, 
												Diffuse = XnaColor.FromNonPremultiplied(0, 0, 0xff, 0xff), 
												Specular = XnaColor.White, 
												Glossiness = 1, 
												Opacity = 1, 
												SpecularLevel = 1
		                                 	};

		public XnaColor Ambient;
		public XnaColor Diffuse;
		public XnaColor Specular;
		public float Glossiness;
		public float SpecularLevel;
		public float Opacity;
	}
}