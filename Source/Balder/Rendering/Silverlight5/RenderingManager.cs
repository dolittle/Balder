using System;
using System.Collections.Generic;
using Balder.Display;
using Balder.Execution;
using Balder.Objects;
using Balder.Rendering.Xna;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Balder.Math.Matrix;

namespace Balder.Rendering.Silverlight5
{
	[Singleton]
	public class RenderingManager : IRenderingManager
	{
		Dictionary<INode, RenderingObject> _objects = new Dictionary<INode, RenderingObject>();
		List<RenderingObject> _sprites = new List<RenderingObject>();

		Viewport _skyboxViewport;
		Skybox _skybox;
		SkyboxContext _skyboxContext;

		
		public void Initialize()
		{
			if (_objects != null)
				_objects.Clear();

			
			_objects = new Dictionary<INode, RenderingObject>();
			_sprites = new List<RenderingObject>();
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

		public void RegisterForRendering(SkyboxContext skyboxContext, Viewport viewport, Skybox skybox)
		{
			_skyboxViewport = viewport;
			_skyboxContext = skyboxContext;
			_skybox = skybox;
		}

		public void RegisterForRendering(SpriteContext sprite, Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			RegisterForRendering<RenderingSprite>(viewport, node, view, projection, world, r =>
			                                                                               	{
			                                                                               		r.Sprite = sprite;
																								_sprites.Add(r);
			                                                                               	});
		}

	
		public void Render(GraphicsDevice graphicsDevice)
		{
			graphicsDevice.BlendState = BlendState.Opaque;
			if( _skyboxContext != null )
			{
				graphicsDevice.DepthStencilState = DepthStencilState.None;
				graphicsDevice.RasterizerState = RasterizerState.CullNone;
				_skyboxContext.ActualRender(graphicsDevice, _skyboxViewport, _skybox);
				graphicsDevice.RasterizerState = RasterizerState.CullCounterClockwise;
			}


			lock (_objects)
			{
				graphicsDevice.DepthStencilState = DepthStencilState.Default;

				foreach (var renderingObject in _objects.Values)
					if( !_sprites.Contains(renderingObject))
						Render(graphicsDevice, renderingObject);

				
				_sprites.Sort((a, b) =>
				              	{
				              		var viewPosition = a.View.GetTranslation();

				              		var aDistance = viewPosition - a.World.GetTranslation();
				              		var bDistance = viewPosition - b.World.GetTranslation();

									if (aDistance == bDistance)
										return 0;

				              		return aDistance < bDistance ? -1 : 1;
				              	});

				graphicsDevice.BlendState = BlendState.AlphaBlend;
				graphicsDevice.DepthStencilState = DepthStencilState.DepthRead;

				foreach (var renderingObject in _sprites)
					Render(graphicsDevice, renderingObject);
			}
		}

		static void Render(GraphicsDevice graphicsDevice, RenderingObject renderingObject)
		{
			//if (!renderingObject.IsVisible) return;

			renderingObject.Render(graphicsDevice);
			//renderingObject.IsVisible = false;
		}
	}
}