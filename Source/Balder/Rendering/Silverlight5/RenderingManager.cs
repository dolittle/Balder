using System;
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
		Dictionary<INode, RenderingObject> _objects = new Dictionary<INode, RenderingObject>();
		
		public void Initialize()
		{
			if (_objects != null)
				_objects.Clear();

			_objects = new Dictionary<INode, RenderingObject>();
		}


		private void RegisterForRendering<T>(Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world, Action<T> creationCallback) where T:RenderingObject, new()
		{
			T renderingObject = null;
			if (_objects.ContainsKey(node))
				renderingObject = _objects[node] as T;
			else
			{
				renderingObject = new T
				{
					Node = node
				};
				creationCallback(renderingObject);
				lock (_objects)
					_objects[node] = renderingObject;
			}
			if (renderingObject == null)
				return;

			renderingObject.View = view ?? Matrix.Identity;
			renderingObject.Projection = projection ?? Matrix.Identity;
			renderingObject.World = world ?? Matrix.Identity;
			renderingObject.Viewport = viewport;
			renderingObject.IsVisible = true;
		}



		public void RegisterForRendering(GeometryDetailLevel geometry, Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			RegisterForRendering<RenderingGeometry>(viewport, node, view, projection, world, r => r.Geometry = geometry);
		}

		public void RegisterForRendering(SpriteContext sprite, Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			RegisterForRendering<RenderingSprite>(viewport, node, view, projection, world, r => r.Sprite = sprite);
		}
		

		public void Render(GraphicsDevice graphicsDevice)
		{
			lock (_objects)
				foreach (var renderingObject in _objects.Values)
					renderingObject.Render(graphicsDevice);
		}
	}
}