using System.Collections.Generic;
using Balder.Display;
using Balder.Execution;
using Balder.Rendering.Xna;
using Balder.Rendering.Xna.Shaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Balder.Math.Matrix;
using XnaColor = Microsoft.Xna.Framework.Color;

namespace Balder.Rendering.Silverlight5
{
	[Singleton]
	public class RenderingManager : IRenderingManager
	{
		Dictionary<INode, RenderingGeometry> _geometries = new Dictionary<INode, RenderingGeometry>();


		public void Initialize()
		{
			if (_geometries != null)
				_geometries.Clear();

			_geometries = new Dictionary<INode, RenderingGeometry>();
		}

		public void RegisterForRendering(GeometryDetailLevel geometry, Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			RenderingGeometry renderingGeometry = null;
			if (_geometries.ContainsKey(node))
			{
				renderingGeometry = _geometries[node];
			}
			else
			{
				renderingGeometry = new RenderingGeometry
										{
											Geometry = geometry,
											Node = node
										};
				lock (_geometries)
					_geometries[node] = renderingGeometry;
			}

			renderingGeometry.View = view;
			renderingGeometry.Projection = projection;
			renderingGeometry.World = world;
			renderingGeometry.Viewport = viewport;
			renderingGeometry.IsVisible = true;
		}

		public void Render(GraphicsDevice graphicsDevice)
		{
			lock (_geometries)
			{
				foreach (var renderingGeometry in _geometries.Values)
				{
					renderingGeometry.Render(graphicsDevice);
				}
			}
		}


		public void HandleLights(GraphicsDevice graphicsDevice, Viewport viewport)
		{
			var lightInfo = new LightInfo();
			var lightCount = 0;

			foreach (Lighting.ILight light in viewport.Scene.Lights)
				SetLightInfo(ref lightInfo, lightCount++, light);

			var viewPosition = (Vector4)viewport.View.Position;
			graphicsDevice.SetVertexShaderConstantFloat4(8, ref viewPosition);
			graphicsDevice.SetVertexShaderConstantFloat4(13, ref lightInfo);
		}

		public void SetMaterial(GraphicsDevice graphicsDevice, Materials.Material material)
		{
			var shaderMaterial = material.ToMaterial();
			//graphicsDevice.SetVertexShaderConstantFloat4(9, ref shaderMaterial);
		}

		static void SetLightInfo(ref LightInfo lightInfo, int lightNumber, Lighting.ILight light)
		{
			object li = lightInfo;
			LightInfo.SetField(li, lightNumber, "Ambient", (XnaColor)light.Ambient);

			LightInfo.SetField(li, lightNumber, "Diffuse", (XnaColor)light.Diffuse);
			LightInfo.SetField(li, lightNumber, "Specular", (XnaColor)light.Specular);
			LightInfo.SetField(li, lightNumber, "Strength", (float)light.Strength);

			if (light is Lighting.DirectionalLight)
			{
				LightInfo.SetField(li, lightNumber, "PositionOrDirection", (Vector4)((Lighting.DirectionalLight)light).Direction);
				LightInfo.SetField(li, lightNumber, "LightType", LightType.Directional);
			}
			if (light is Lighting.OmniLight)
			{
				LightInfo.SetField(li, lightNumber, "PositionOrDirection", (Vector4)((Lighting.OmniLight)light).Position);
				LightInfo.SetField(li, lightNumber, "Range", (float)((Lighting.OmniLight)light).Range);
				LightInfo.SetField(li, lightNumber, "LightType", LightType.Omni);
			}

			lightInfo = (LightInfo) li;
		}
	}
}