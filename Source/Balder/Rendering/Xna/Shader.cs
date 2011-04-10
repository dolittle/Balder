using System.Collections.Generic;
using Balder.Display;
using Balder.Lighting;
using Balder.Rendering.Xna.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Xna
{
	public class Shader
	{
		public VertexShader Vertex { get; set; }
		public PixelShader Pixel { get; set; }

		public Shader()
		{
			EnableDefaultLighting = true;
			EnableDefaultMaterialHandling = true;
		}

		public bool EnableDefaultLighting { get; set; }
		public bool EnableDefaultMaterialHandling { get; set; }


		public virtual void HandleLighting(GraphicsDevice graphicsDevice, Viewport viewport)
		{
			if (!EnableDefaultLighting)
				return;

			var lightInfo = LightInfo.Create();
			var lightCount = 0;

			foreach (ILight light in viewport.Scene.Lights )
				SetLightInfo(ref lightInfo, lightCount++, light);

			var viewPosition = (Vector4)viewport.View.Position;
			graphicsDevice.SetVertexShaderConstantFloat4(8, ref viewPosition);

			var registerOffset = 13;
			for (var lightIndex = 0; lightIndex < LightInfo.MaxLights; lightIndex++)
			{
				graphicsDevice.SetVertexShaderConstantFloat4(registerOffset, ref lightInfo.PositionOrDirection[lightIndex]);
				graphicsDevice.SetVertexShaderConstant(registerOffset + 1, lightInfo.Ambient[lightIndex]);
				graphicsDevice.SetVertexShaderConstant(registerOffset + 2, lightInfo.Diffuse[lightIndex]);
				graphicsDevice.SetVertexShaderConstant(registerOffset + 3, lightInfo.Specular[lightIndex]);
				graphicsDevice.SetVertexShaderLightingDetails(registerOffset + 4, lightInfo.Strength[lightIndex],
															  lightInfo.Range[lightIndex], lightInfo.LightType[lightIndex]);
				registerOffset += 5;
			}
			
		}


		public virtual void HandleMaterial(GraphicsDevice graphicsDevice, Materials.Material material)
		{
			Material shaderMaterial;
			if (material != null)
				shaderMaterial = material.ToMaterial();
			else
				shaderMaterial = Material.Default;

			graphicsDevice.SetVertexShaderConstant(9, shaderMaterial.Ambient);
			graphicsDevice.SetVertexShaderConstant(10, shaderMaterial.Diffuse);
			graphicsDevice.SetVertexShaderConstant(11, shaderMaterial.Specular);
			graphicsDevice.SetVertexShaderMaterialDetails(12, shaderMaterial);
		}

		static void SetLightInfo(ref LightInfo lightInfo, int lightNumber, Lighting.ILight light)
		{
			lightInfo.Ambient[lightNumber] = light.Ambient;
			lightInfo.Diffuse[lightNumber] = light.Diffuse;
			lightInfo.Specular[lightNumber] = light.Specular;
			lightInfo.Strength[lightNumber] = (float)light.Strength;

			if (light is Lighting.DirectionalLight)
			{
				lightInfo.LightType[lightNumber] = LightType.Directional;
				lightInfo.PositionOrDirection[lightNumber] = ((Lighting.DirectionalLight)light).Direction;
			}
			if (light is Lighting.OmniLight)
			{
				lightInfo.LightType[lightNumber] = LightType.Omni;
				lightInfo.PositionOrDirection[lightNumber] = ((Lighting.OmniLight)light).Position;
				lightInfo.Range[lightNumber] = (float)((Lighting.OmniLight)light).Range;
			}
		}

	}
}