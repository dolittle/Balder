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
using Balder.Objects.Geometries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Xna
{
	public struct RenderVertex 
#if(!SILVERLIGHT)
		: IVertexType
#endif
    {
        public Vector3 Position;
		public Vector3 Normal;
		public Microsoft.Xna.Framework.Color Color;
		public Vector2 TextureCoordinate;

        public RenderVertex(Vertex vertex)
        {
            Position = new Vector3(vertex.X,vertex.Y,vertex.Z);
            Normal = new Vector3(vertex.NormalX, vertex.NormalY, vertex.NormalZ);
            Color = new Microsoft.Xna.Framework.Color(0,0,0xff,0xff);
        	TextureCoordinate = Vector2.Zero;
        }

        public RenderVertex(Vertex vertex, Color color)
        {
            Position = new Vector3(vertex.X, vertex.Y, vertex.Z);
            Normal = new Vector3(vertex.NormalX, vertex.NormalY, vertex.NormalZ);
            Color = color;
			TextureCoordinate = Vector2.Zero;
        }

        public RenderVertex(Vector3 position, Microsoft.Xna.Framework.Color color)
        {
            Position = position;
            Color = color;
            Normal = Vector3.Zero;
			TextureCoordinate = Vector2.Zero;
        }

		public RenderVertex(Vector3 position, Microsoft.Xna.Framework.Color color, Vector3 normal)
		{
			Position = position;
			Color = color;
			Normal = normal;
			TextureCoordinate = Vector2.Zero;
		}

		public RenderVertex(Vector3 position, Microsoft.Xna.Framework.Color color, Vector3 normal, Vector2 textureCoordinate)
		{
			Position = position;
			Color = color;
			Normal = normal;
			TextureCoordinate = textureCoordinate;
		}

        public RenderVertex(Vertex vertex, Color color, Normal normal)
        {
            Position = new Vector3(vertex.X, vertex.Y, vertex.Z);
            Normal = new Vector3(normal.X, normal.Y, normal.Z);
            Color = color;
			TextureCoordinate = Vector2.Zero;
        }

		public RenderVertex(Vertex vertex, Color color, Normal normal, TextureCoordinate textureCoordinate)
		{
			Position = new Vector3(vertex.X, vertex.Y, vertex.Z);
			Normal = new Vector3(normal.X, normal.Y, normal.Z);
			Color = color;
			TextureCoordinate = new Vector2(textureCoordinate.U, textureCoordinate.V);
		}


        public static readonly VertexDeclaration VertexDeclaration = new VertexDeclaration
			(
				new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
				new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
				new VertexElement(24, VertexElementFormat.Color, VertexElementUsage.Color, 0),
				new VertexElement(28, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
			);

#if(!SILVERLIGHT)
        public VertexDeclaration VertexDeclaration
        {
            get { return Declaration; }
        }
#endif
    }
}
#endif