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
												Glossiness = 1f, 
												Opacity = 1f, 
												SpecularLevel = 1f,
												DiffuseMapOpacity = 1f,
												ReflectionMapOpacity = 1f
		                                 	};

		public XnaColor Ambient;
		public XnaColor Diffuse;
		public XnaColor Specular;
		public float Glossiness;
		public float SpecularLevel;
		public float Opacity;
		public float DiffuseMapOpacity;
		public float ReflectionMapOpacity;
	}
}