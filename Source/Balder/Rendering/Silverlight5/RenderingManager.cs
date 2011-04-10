using System.Collections.Generic;
using Balder.Display;
using Balder.Execution;
using Balder.Rendering.Xna;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Balder.Math.Matrix;

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
	}
}