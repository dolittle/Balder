﻿using System.Collections.Generic;
using Balder.Display;
using Balder.Lighting;
using Balder.Rendering.Xna.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XnaColor = Microsoft.Xna.Framework.Color;

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

			graphicsDevice.SetVertexShaderConstantFloat4(17, ref viewPosition);

			var registerOffset = 18;
			for (var lightIndex = 0; lightIndex < LightInfo.MaxLights; lightIndex++)
			{
				graphicsDevice.SetVertexShaderConstantFloat4(registerOffset, ref lightInfo.PositionOrDirection[lightIndex]);
				graphicsDevice.SetShaderConstant(registerOffset + 1, lightInfo.Ambient[lightIndex]);
				graphicsDevice.SetShaderConstant(registerOffset + 2, lightInfo.Diffuse[lightIndex]);
				graphicsDevice.SetShaderConstant(registerOffset + 3, lightInfo.Specular[lightIndex]);
				graphicsDevice.SetShaderLightingDetails(registerOffset + 4, lightInfo.Strength[lightIndex],
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

			graphicsDevice.SetShaderConstant(12, shaderMaterial.Ambient);
			graphicsDevice.SetShaderConstant(13, shaderMaterial.Diffuse);
			graphicsDevice.SetShaderConstant(14, shaderMaterial.Specular);
			graphicsDevice.SetShaderMaterialDetails(15, shaderMaterial);
			graphicsDevice.SetShaderMaterialMapOpacities(16, shaderMaterial);
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