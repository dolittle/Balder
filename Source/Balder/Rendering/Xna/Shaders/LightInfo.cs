using Microsoft.Xna.Framework;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Balder.Rendering.Xna.Shaders
{
	public struct LightInfo
	{
		public const int MaxLights = 3;

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

			for (var lightIndex = 0; lightIndex < MaxLights; lightIndex++)
			{
				lightInfo.Ambient[lightIndex] = XnaColor.Black;
				lightInfo.Diffuse[lightIndex] = XnaColor.Black;
				lightInfo.Specular[lightIndex] = XnaColor.Black;
				lightInfo.Strength[lightIndex] = 0;
				lightInfo.Range[lightIndex] = 0;
				lightInfo.LightType[lightIndex] = 0;
			}

			return lightInfo;
		}
	}
}