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
using Balder.Objects.Geometries;
using Balder.Rendering.Silverlight5;
using Microsoft.Xna.Framework.Graphics;

#if(WINDOWS_PHONE)
using D = Balder.Display.WP7.Display;
#else
#if(SILVERLIGHT)
using D = Balder.Display.Silverlight5.Display;
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
    	RenderVertex _vertex;
    	VertexBuffer _vertexBuffer;


        public SpriteContext(IRenderingManager renderingManager)
        {
        	_renderingManager = renderingManager;
        	_vertex = new RenderVertex(new Vertex(0, 0, 0));
			//_vertexBuffer = new VertexBuffer();
        }


    	public void Render(Viewport viewport, Sprite sprite, Matrix view, Matrix projection, Matrix world, float xScale, float yScale, float rotation)
        {
			_renderingManager.RegisterForRendering(this, viewport, sprite, view, projection, sprite.RenderingWorld);
        }

		internal void ActualRender(GraphicsDevice graphicsDevice, Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			XnaMatrix worldView = world * view;
			var matrix = worldView * projection;
			graphicsDevice.SetVertexShaderConstantFloat4(0, ref matrix);
			graphicsDevice.SetVertexShaderConstantFloat4(4, ref worldView);

			graphicsDevice.SetShader(ShaderManager.Instance.Sprite);

			//graphicsDevice.SetVertexBuffer(_vertexBuffer);
			//_vertex.TransformAndProject(viewport, worldView, matrix);
			
		}
    }
}
#endif