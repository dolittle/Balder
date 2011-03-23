using System;
using System.Collections.Generic;
using Balder.Execution;
using Balder.Math;
using Balder.Rendering.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Silverlight5
{
	[Singleton]
	public class RenderingManager : IRenderingManager
	{
		Dictionary<INode, RenderingGeometry> _geometries = new Dictionary<INode, RenderingGeometry>();


		public void Initialize()
		{
			if( _geometries != null )
				_geometries.Clear();

			_geometries = new Dictionary<INode, RenderingGeometry>();
		}

		public void RegisterForRendering(GeometryDetailLevel geometry, INode node, Matrix view, Matrix projection, Matrix world)
		{
			RenderingGeometry renderingGeometry = null;
			if( _geometries.ContainsKey(node))
			{
				renderingGeometry = _geometries[node];
			} else
			{
				renderingGeometry = new RenderingGeometry
				                    	{
				                    		Geometry = geometry,
											Node = node
				                    	};
				_geometries[node] = renderingGeometry;
			}

			renderingGeometry.View = view;
			renderingGeometry.Projection = projection;
			renderingGeometry.World = world;
			renderingGeometry.IsVisible = true;
		}

		public void Render(GraphicsDevice graphicsDevice)
		{
			foreach( var renderingGeometry in _geometries.Values )
			{
				renderingGeometry.Render(graphicsDevice);
			}
		}
	}
}