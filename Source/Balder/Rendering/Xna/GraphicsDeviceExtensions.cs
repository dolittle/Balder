using Balder.Rendering.Xna.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Balder.Rendering.Xna
{
	public static class GraphicsDeviceExtensions
	{
		public static void SetShader(this GraphicsDevice graphicsDevice, Shader shader)
		{
			graphicsDevice.SetVertexShader(shader.Vertex);
			graphicsDevice.SetPixelShader(shader.Pixel);
		}

		public static void SetShaderConstant(this GraphicsDevice graphicsDevice, int register, XnaColor color)
		{
			var vector = new Vector4(
				
					((float)color.B)/255f,
					((float)color.G)/255f,
					((float)color.R)/255f,
					((float)color.A)/255f
				);

			graphicsDevice.SetVertexShaderConstantFloat4(register, ref vector);
			graphicsDevice.SetPixelShaderConstantFloat4(register, ref vector);
		}


		public static int SetShaderConstant(this GraphicsDevice graphicsDevice, int startRegister, Vector4[] vectors)
		{
			for (var vectorIndex = 0; vectorIndex < vectors.Length; vectorIndex++)
			{
				graphicsDevice.SetVertexShaderConstantFloat4(startRegister + (vectorIndex*4), ref vectors[vectorIndex]);
				graphicsDevice.SetPixelShaderConstantFloat4(startRegister + (vectorIndex*4), ref vectors[vectorIndex]);
			}

			return startRegister + (vectors.Length*4);
		}

		public static int SetShaderConstant(this GraphicsDevice graphicsDevice, int startRegister, XnaColor[] colors)
		{
			for (var colorIndex = 0; colorIndex < colors.Length; colorIndex++)
				graphicsDevice.SetShaderConstant(startRegister + (colorIndex * 4), colors[colorIndex]);

			return startRegister + (colors.Length*4);
		}

		public static void SetShaderMaterialDetails(this GraphicsDevice graphicsDevice, int register, Material material)
		{
			var vector = new Vector4
				(
				material.Glossiness,
				material.SpecularLevel,
				material.Opacity,
				0
				);
			graphicsDevice.SetVertexShaderConstantFloat4(register, ref vector);
			graphicsDevice.SetPixelShaderConstantFloat4(register, ref vector);
		}

		public static void SetShaderMaterialMapOpacities(this GraphicsDevice graphicsDevice, int register, Material material)
		{
			var vector = new Vector4
				(
				material.DiffuseMapOpacity,
				material.ReflectionMapOpacity,
				0,
				0
				);
			graphicsDevice.SetVertexShaderConstantFloat4(register, ref vector);
			graphicsDevice.SetPixelShaderConstantFloat4(register, ref vector);
		}

		public static void SetShaderLightingDetails(this GraphicsDevice graphicsDevice, int register, float strength, float range, LightType type)
		{
			var vector = new Vector4
				(
				strength,
				range,
				(float) type,
				0
				);
			graphicsDevice.SetVertexShaderConstantFloat4(register, ref vector);
			graphicsDevice.SetPixelShaderConstantFloat4(register, ref vector);
		}

		public static int SetShaderLightingDetails(this GraphicsDevice graphicsDevice, int startRegister, LightInfo lightInfo)
		{
			for( var lightIndex=0; lightIndex<LightInfo.MaxLights; lightIndex++ )
				graphicsDevice.SetShaderLightingDetails(startRegister + (lightIndex * 4), lightInfo.Strength[lightIndex], lightInfo.Range[lightIndex], lightInfo.LightType[lightIndex]);

			return startRegister + (LightInfo.MaxLights*4);
		}



	}
}