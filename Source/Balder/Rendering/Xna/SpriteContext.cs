#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
#if(XNA)
using System;
using Balder.Display;
using Balder.Math;
using Balder.Objects.Flat;
using Balder.Rendering.Silverlight5;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#if(WINDOWS_PHONE)
using D = Balder.Display.WP7.Display;
#else
#if(SILVERLIGHT)
using D = Balder.Display.Silverlight5.Display;
using Matrix = Balder.Math.Matrix;
#else
using D = Balder.Display.Xna.Display;
#endif
#endif
using XnaMatrix = Microsoft.Xna.Framework.Matrix;



namespace Balder.Rendering.Xna
{
    public class SpriteContext : ISpriteContext
    {
    	readonly IRenderingManager _renderingManager;
    	VertexBuffer _vertexBuffer;

        public SpriteContext(IRenderingManager renderingManager)
        {
        	_renderingManager = renderingManager;
			PrepareVertices();
		}
		
		private void PrepareVertices()
		{
			_vertexBuffer = new VertexBuffer(D.GraphicsDevice, RenderVertex.VertexDeclaration, 6, BufferUsage.WriteOnly);
		}


    	public void Render(Viewport viewport, Sprite sprite, Matrix view, Matrix projection, Matrix world, float xScale, float yScale, float rotation)
        {
			_renderingManager.RegisterForRendering(this, viewport, sprite, view, projection, sprite.RenderingWorld);
        }

		internal void ActualRender(GraphicsDevice graphicsDevice, Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			var sprite = node as Sprite;
			if( sprite == null )
				return;

			var image = ((Sprite)node).CurrentFrame;
			if (null == image)
				return;

			var size = (float)System.Math.Max(sprite.CurrentFrame.Width, sprite.CurrentFrame.Height);
			var actualSize = size / 10f;

			var inverseView = Matrix.Invert(view);
			var upperLeft = Vector.TransformNormal(new Vector(-actualSize, actualSize, 0), inverseView);
			var upperRight = Vector.TransformNormal(new Vector(actualSize, actualSize, 0), inverseView);
			var lowerLeft = Vector.TransformNormal(new Vector(-actualSize, -actualSize, 0), inverseView);
			var lowerRight = Vector.TransformNormal(new Vector(actualSize, -actualSize, 0), inverseView);

			var vertices = new RenderVertex[6]
			               	{
								new RenderVertex(upperLeft, new Vector2(0f,0f)), 
								new RenderVertex(upperRight, new Vector2(1f,0f)), 
								new RenderVertex(lowerRight, new Vector2(1f,1f)), 

								new RenderVertex(upperLeft, new Vector2(0f,0f)), 
								new RenderVertex(lowerRight, new Vector2(1f,1f)), 
								new RenderVertex(lowerLeft, new Vector2(0f,1f)), 

			               	};
			_vertexBuffer.SetData(0, vertices, 0, vertices.Length, 0);

			XnaMatrix xnaWorld = world;
			var worldView = xnaWorld * view; 
			var worldViewProjection = worldView * projection;

			graphicsDevice.SetVertexShaderConstantFloat4(0, ref xnaWorld);
			graphicsDevice.SetVertexShaderConstantFloat4(4, ref worldView);
			graphicsDevice.SetVertexShaderConstantFloat4(8, ref worldViewProjection);


			graphicsDevice.SetShader(ShaderManager.Instance.Sprite);
			graphicsDevice.SetVertexBuffer(_vertexBuffer);

			var imageContext = image.ImageContext as ImageContext;
			graphicsDevice.Textures[0] = imageContext.Texture;

			graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, _vertexBuffer.VertexCount / 3);
		}
    }
}
#endif