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
using Balder.Materials;
using Balder.Math;
using Balder.Objects;
using Balder.Rendering.Silverlight5;
using Microsoft.Xna.Framework;
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
	public class SkyboxContext : ISkyboxContext
	{
		private const float Size = 50f;
		readonly IRenderingManager _renderingManager;


		VertexBuffer _frontVertexBuffer;
		VertexBuffer _backVertexBuffer;
		VertexBuffer _leftVertexBuffer;
		VertexBuffer _rightVertexBuffer;
		VertexBuffer _topVertexBuffer;
		VertexBuffer _bottomVertexBuffer;



		public SkyboxContext(IRenderingManager renderingManager)
		{
			_renderingManager = renderingManager;
			Prepare();
		}

		void Prepare()
		{
			var upperFrontLeft = new Vector(-Size,Size,Size,1f);
			var upperFrontRight = new Vector(Size, Size, Size, 1f);
			var lowerFrontLeft = new Vector(-Size, -Size, Size, 1f);
			var lowerFrontRight = new Vector(Size, -Size, Size, 1f);

			var upperBackLeft = new Vector(-Size, Size, -Size, 1f);
			var upperBackRight = new Vector(Size, Size, -Size, 1f);
			var lowerBackLeft = new Vector(-Size, -Size, -Size, 1f);
			var lowerBackRight = new Vector(Size, -Size, -Size, 1f);

			var frontVertices = new[]

			                    	{
										new RenderVertex(upperFrontLeft, new Vector2(0f,0f)), 
										new RenderVertex(upperFrontRight, new Vector2(1f,0f)), 
										new RenderVertex(lowerFrontRight, new Vector2(1f,1f)), 

										new RenderVertex(upperFrontLeft, new Vector2(0f,0f)), 
										new RenderVertex(lowerFrontRight, new Vector2(1f,1f)), 
										new RenderVertex(lowerFrontLeft, new Vector2(0f,1f)), 
			                    	};
			_frontVertexBuffer = new VertexBuffer(D.GraphicsDevice, RenderVertex.VertexDeclaration, frontVertices.Length, BufferUsage.WriteOnly);
			_frontVertexBuffer.SetData(0, frontVertices, 0, frontVertices.Length, 0);

			var backVertices = new[]

			                    	{
										new RenderVertex(upperBackRight, new Vector2(0f,0f)), 
										new RenderVertex(upperBackLeft, new Vector2(1f,0f)), 
										new RenderVertex(lowerBackLeft, new Vector2(1f,1f)), 

										new RenderVertex(upperBackRight, new Vector2(0f,0f)), 
										new RenderVertex(lowerBackLeft, new Vector2(1f,1f)), 
										new RenderVertex(lowerBackRight, new Vector2(0f,1f)), 
			                    	};
			_backVertexBuffer = new VertexBuffer(D.GraphicsDevice, RenderVertex.VertexDeclaration, backVertices.Length, BufferUsage.WriteOnly);
			_backVertexBuffer.SetData(0, backVertices, 0, backVertices.Length, 0);

			var leftVertices = new[]

			                    	{
										new RenderVertex(upperBackLeft, new Vector2(0f,0f)), 
										new RenderVertex(upperFrontLeft, new Vector2(1f,0f)), 
										new RenderVertex(lowerFrontLeft, new Vector2(1f,1f)), 

										new RenderVertex(upperBackLeft, new Vector2(0f,0f)), 
										new RenderVertex(lowerFrontLeft, new Vector2(1f,1f)), 
										new RenderVertex(lowerBackLeft, new Vector2(0f,1f)), 
			                    	};
			_leftVertexBuffer = new VertexBuffer(D.GraphicsDevice, RenderVertex.VertexDeclaration, leftVertices.Length, BufferUsage.WriteOnly);
			_leftVertexBuffer.SetData(0, leftVertices, 0, leftVertices.Length, 0);

			var rightVertices = new[]

			                    	{
										new RenderVertex(upperFrontRight, new Vector2(0f,0f)), 
										new RenderVertex(upperBackRight, new Vector2(1f,0f)), 
										new RenderVertex(lowerBackRight, new Vector2(1f,1f)), 

										new RenderVertex(upperFrontRight, new Vector2(0f,0f)), 
										new RenderVertex(lowerBackRight, new Vector2(1f,1f)), 
										new RenderVertex(lowerFrontRight, new Vector2(0f,1f)), 
			                    	};
			_rightVertexBuffer = new VertexBuffer(D.GraphicsDevice, RenderVertex.VertexDeclaration, rightVertices.Length, BufferUsage.WriteOnly);
			_rightVertexBuffer.SetData(0, rightVertices, 0, rightVertices.Length, 0);

			var topVertices = new[]

			                    	{
										new RenderVertex(upperBackLeft, new Vector2(0f,0f)), 
										new RenderVertex(upperBackRight, new Vector2(1f,0f)), 
										new RenderVertex(upperFrontRight, new Vector2(1f,1f)), 

										new RenderVertex(upperBackLeft, new Vector2(0f,0f)), 
										new RenderVertex(upperFrontRight, new Vector2(1f,1f)), 
										new RenderVertex(upperFrontLeft, new Vector2(0f,1f)), 
			                    	};
			_topVertexBuffer = new VertexBuffer(D.GraphicsDevice, RenderVertex.VertexDeclaration, topVertices.Length, BufferUsage.WriteOnly);
			_topVertexBuffer.SetData(0, topVertices, 0, topVertices.Length, 0);

			var bottomVertices = new[]

			                    	{
										new RenderVertex(lowerFrontLeft, new Vector2(0f,0f)), 
										new RenderVertex(lowerFrontRight, new Vector2(1f,0f)), 
										new RenderVertex(lowerBackRight, new Vector2(1f,1f)), 

										new RenderVertex(lowerFrontLeft, new Vector2(0f,0f)), 
										new RenderVertex(lowerBackRight, new Vector2(1f,1f)), 
										new RenderVertex(lowerBackLeft, new Vector2(0f,1f)), 
			                    	};
			_bottomVertexBuffer = new VertexBuffer(D.GraphicsDevice, RenderVertex.VertexDeclaration, bottomVertices.Length, BufferUsage.WriteOnly);
			_bottomVertexBuffer.SetData(0, bottomVertices, 0, bottomVertices.Length, 0);

		}

		public void Render(Viewport viewport, Skybox skybox)
		{
			_renderingManager.RegisterForRendering(this, viewport, skybox);
		}

		internal void ActualRender(GraphicsDevice graphicsDevice, Viewport viewport, Skybox skybox)
		{
			var frontTexture = ((ImageContext)((ImageMap)skybox.Front).Image.ImageContext).Texture;
			var backTexture = ((ImageContext)((ImageMap)skybox.Back).Image.ImageContext).Texture;
			var leftTexture = ((ImageContext)((ImageMap)skybox.Left).Image.ImageContext).Texture;
			var rightTexture = ((ImageContext)((ImageMap)skybox.Right).Image.ImageContext).Texture;
			var topTexture = ((ImageContext)((ImageMap)skybox.Top).Image.ImageContext).Texture;
			var bottomTexture = ((ImageContext)((ImageMap)skybox.Bottom).Image.ImageContext).Texture;

			XnaMatrix worldView = viewport.View.ViewMatrix;
			worldView.Translation = Vector3.Zero;

			var matrix = worldView * viewport.View.ProjectionMatrix;

			graphicsDevice.SetVertexShaderConstantFloat4(0, ref matrix);
			graphicsDevice.SetVertexShaderConstantFloat4(4, ref worldView);

			graphicsDevice.SetShader(ShaderManager.Instance.Texture);

			RenderPart(graphicsDevice, frontTexture, _frontVertexBuffer);
			RenderPart(graphicsDevice, backTexture, _backVertexBuffer);
			RenderPart(graphicsDevice, leftTexture, _leftVertexBuffer);
			RenderPart(graphicsDevice, rightTexture, _rightVertexBuffer);
			RenderPart(graphicsDevice, topTexture, _topVertexBuffer);
			RenderPart(graphicsDevice, bottomTexture, _bottomVertexBuffer);
		}

		private void RenderPart(GraphicsDevice graphicsDevice, Texture2D texture, VertexBuffer vertexBuffer)
		{
			graphicsDevice.SetVertexBuffer(vertexBuffer);
			graphicsDevice.Textures[0] = texture;
			graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList, 0, vertexBuffer.VertexCount / 3);
		}
	}
}
#endif