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
	public struct RenderVertex : IVertexType
    {
        private Vector3 _position;
        private Vector3 _normal;
        private Microsoft.Xna.Framework.Color _color;
		private Vector2 _textureCoordinate;

        public RenderVertex(Vertex vertex)
        {
            _position = new Vector3(vertex.X,vertex.Y,vertex.Z);
            _normal = new Vector3(vertex.NormalX, vertex.NormalY, vertex.NormalZ);
            _color = Microsoft.Xna.Framework.Color.Blue;
        	_textureCoordinate = Vector2.Zero;
        }

        public RenderVertex(Vertex vertex, Color color)
        {
            _position = new Vector3(vertex.X, vertex.Y, vertex.Z);
            _normal = new Vector3(vertex.NormalX, vertex.NormalY, vertex.NormalZ);
            _color = color;
			_textureCoordinate = Vector2.Zero;
        }

        public RenderVertex(Vector3 position, Microsoft.Xna.Framework.Color color)
        {
            _position = position;
            _color = color;
            _normal = Vector3.Zero;
			_textureCoordinate = Vector2.Zero;
        }

		public RenderVertex(Vector3 position, Microsoft.Xna.Framework.Color color, Vector3 normal)
		{
			_position = position;
			_color = color;
			_normal = normal;
			_textureCoordinate = Vector2.Zero;
		}

		public RenderVertex(Vector3 position, Microsoft.Xna.Framework.Color color, Vector3 normal, Vector2 textureCoordinate)
		{
			_position = position;
			_color = color;
			_normal = normal;
			_textureCoordinate = textureCoordinate;
		}

        public RenderVertex(Vertex vertex, Color color, Normal normal)
        {
            _position = new Vector3(vertex.X, vertex.Y, vertex.Z);
            _normal = new Vector3(normal.X, normal.Y, normal.Z);
            _color = color;
			_textureCoordinate = Vector2.Zero;
        }

		public RenderVertex(Vertex vertex, Color color, Normal normal, TextureCoordinate textureCoordinate)
		{
			_position = new Vector3(vertex.X, vertex.Y, vertex.Z);
			_normal = new Vector3(normal.X, normal.Y, normal.Z);
			_color = color;
			_textureCoordinate = new Vector2(textureCoordinate.U, textureCoordinate.V);
		}


        public static VertexElement[] VertexElements = {
                                                           new VertexElement(0, VertexElementFormat.Vector3,VertexElementUsage.Position, 0), 
                                                           new VertexElement(sizeof(float)*3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                                                           new VertexElement(sizeof(float)*6,VertexElementFormat.Color,VertexElementUsage.Color,0), 
														   new VertexElement((sizeof(float)*6)+sizeof(UInt32), VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
                                                       };

        public static VertexDeclaration Declaration = new VertexDeclaration(VertexElements);

        public VertexDeclaration VertexDeclaration
        {
            get { return Declaration; }
        }
    }
}
#endif