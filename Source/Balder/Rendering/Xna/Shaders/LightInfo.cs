using Microsoft.Xna.Framework;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Balder.Rendering.Xna.Shaders
{
	public struct LightInfo
	{
		public const int MaxLights = 5;

		public Vector4[] PositionOrDirection;
		public XnaColor[] Ambient;
		public XnaColor[] Diffuse;
		public XnaColor[] Specular;
		public float[] Strength;
		public float[] Range;
		public LightType[] LightType;


		public static LightInfo Create()
		{
			var lightInfo = new LightInfo
			                	{
			                		PositionOrDirection = new Vector4[MaxLights],
			                		Ambient = new XnaColor[MaxLights],
			                		Diffuse = new XnaColor[MaxLights],
			                		Specular = new XnaColor[MaxLights],
			                		Strength = new float[MaxLights],
			                		Range = new float[MaxLights],
			                		LightType = new LightType[MaxLights]
			                	};
			return lightInfo;
		}
	}
}