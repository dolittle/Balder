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
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;
using Microsoft.Xna.Framework.Graphics;
using Viewport = Balder.Display.Viewport;



namespace Balder.Rendering.Xna
{
    public class GeometryDetailLevel : IGeometryDetailLevel
    {
        private BasicEffect _effect;

        public int FaceCount { get; private set; }
        public int VertexCount { get; private set; }
        public int TextureCoordinateCount { get; private set; }
        public int LineCount { get; private set; }
        public int NormalCount { get; private set; }

        private VertexBuffer _vertexBuffer;
        private IndexBuffer _indexBuffer;

        private RenderVertex[] _vertices;
        private ushort[] _indices;

        private bool _verticesPrepared;
        private bool _indicesPrepared;

        private Vertex[] _originalVertices;
        private Face[] _originalFaces;

        private Normal[] _originalNormals;

    	private Material _colorMaterial;
        

        public GeometryDetailLevel()
        {
            _effect = new BasicEffect(Display.WP7.Display.GraphicsDevice);
            _effect.EnableDefaultLighting();
            _effect.VertexColorEnabled = true;
        	_colorMaterial = Material.FromColor(Colors.Blue);
        }

        public void AllocateFaces(int count)
        {
            var actualCount = count*3;
            _indexBuffer = new IndexBuffer(Display.WP7.Display.GraphicsDevice, IndexElementSize.SixteenBits, actualCount, BufferUsage.WriteOnly);
            
            _indices = new ushort[actualCount];
            _originalFaces = new Face[count];

            if( null != _vertexBuffer )
            {
                _vertexBuffer.Dispose();
                _vertexBuffer = null;
            }
            _vertices = null;
        }

        public void SetFace(int index, Face face)
        {
            var actualIndex = index*3;
            _indices[actualIndex+2] = (ushort)face.A;
            _indices[actualIndex+1] = (ushort)face.B;
            _indices[actualIndex] = (ushort)face.C;

            _originalFaces[index] = face;
        }

        public Face[] GetFaces()
        {
            return _originalFaces;
        }

        public void InvalidateFace(int index)
        {
            SetFace(index, _originalFaces[index]);
        }

        public void AllocateVertices(int count)
        {
            //_vertexBuffer = new VertexBuffer(Display.WP7.Display.GraphicsDevice,typeof(RenderVertex),count,BufferUsage.WriteOnly);
            //_vertices = new RenderVertex[count];
            _originalVertices = new Vertex[count];
        	_verticesPrepared = false;
        }

        public void SetVertex(int index, Vertex vertex)
        {
            //var renderVertex = new RenderVertex(vertex);
            //_vertices[index] = renderVertex;
            _originalVertices[index] = vertex;
        }

        public Vertex[] GetVertices()
        {
            return _originalVertices;
        }

        public void InvalidateVertex(int index)
        {
            SetVertex(index, _originalVertices[index]);
        }

        public void AllocateNormals(int count)
        {
            _originalNormals = new Normal[count];
        }


        public void InvalidateNormal(int index)
        {
            
        }

        public void AllocateLines(int count)
        {
            
        }

        public void SetLine(int index, Line line)
        {
            
        }

        public Line[] GetLines()
        {
            throw new NotImplementedException();
        }

        public Face GetFace(int index)
        {
            throw new NotImplementedException();
        }

        public Vector GetFaceNormal(int index)
        {
            throw new NotImplementedException();
        }

        public void AllocateTextureCoordinates(int count)
        {
            
        }

        public void SetTextureCoordinate(int index, TextureCoordinate textureCoordinate)
        {
            
        }

        public void SetFaceTextureCoordinateIndex(int index, int a, int b, int c)
        {
            
        }

        public TextureCoordinate[] GetTextureCoordinates()
        {
            throw new NotImplementedException();
        }

    	public void CalculateVertices(Viewport viewport, INode node)
    	{
    		
    	}


    	private Material GetActualMaterialFromFace(Material material, Face face)
		{
			if (null != material &&
				material.SubMaterials.Count >= face.MaterialId &&
				material.SubMaterials.Count != 0)
			{
				if (material.SubMaterials.ContainsKey(face.MaterialId))
				{
					material = material.SubMaterials[face.MaterialId];
				}
			}
			return material;
		}

		private Material GetMaterialForFace(Face face, INode node, Material material)
		{
			var actualMaterial = GetActualMaterialFromFace(material, face);

			if (null == actualMaterial)
			{
				if (node is IHaveColor)
				{
					actualMaterial = _colorMaterial;
					actualMaterial.Diffuse = ((IHaveColor)node).Color;
				}
				else
				{
					actualMaterial = Material.Default;
				}
			}
			return actualMaterial;
		}


        private void PrepareVertexBuffer(Geometry geometry)
        {
        
            if (null == _vertices)
            {
                _vertices = new RenderVertex[_originalFaces.Length*3];
            }
            var vertexIndex = 0;

        	var nodeMaterial = geometry.Material;

            foreach( var face in _originalFaces )
            {
            	var color = Colors.Blue;
            	var material = GetMaterialForFace(face, geometry, nodeMaterial);
                if( null != material )
                {
                    color = material.Diffuse;
                }


                _vertices[vertexIndex++] = new RenderVertex(_originalVertices[face.C], color, _originalNormals[face.NormalC]);
                _vertices[vertexIndex++] = new RenderVertex(_originalVertices[face.B], color, _originalNormals[face.NormalB]);
                _vertices[vertexIndex++] = new RenderVertex(_originalVertices[face.A], color, _originalNormals[face.NormalA]);
            }
            if (null == _vertexBuffer)
            {
                _vertexBuffer = new VertexBuffer(Display.WP7.Display.GraphicsDevice, typeof (RenderVertex), vertexIndex,
                                                 BufferUsage.WriteOnly);
            }
            _vertexBuffer.SetData(_vertices);
        }

        public void Render(Viewport viewport, INode node)
        {
            /*
            if( !_indicesPrepared )
            {
                if (null == _indices)
                {
                    return;
                }
                _indicesPrepared = true;
                _indexBuffer.SetData(_indices);
            }
            if( !_verticesPrepared )
            {
                if( null == _vertices )
                {
                    return;
                }
                _verticesPrepared = true;
                _vertexBuffer.SetData(_vertices);
            }*/
            if (!_verticesPrepared)
            {
                _verticesPrepared = true;
                PrepareVertexBuffer(node as Geometry);
            }

            _effect.World = node.RenderingWorld;
            _effect.View = viewport.View.ViewMatrix;
            _effect.Projection = viewport.View.ProjectionMatrix;
			

            var graphicsDevice = Display.WP7.Display.GraphicsDevice;
            //graphicsDevice.Indices = _indexBuffer;
            graphicsDevice.SetVertexBuffer(_vertexBuffer);

			if( null == _vertices )
			{
				return;
			}

            foreach( var pass in _effect.CurrentTechnique.Passes )
            {
                pass.Apply();
            
                graphicsDevice.DrawPrimitives(PrimitiveType.TriangleList,0,_vertices.Length/3);

                //graphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, _vertices.Length, 0, _indices.Length / 3);
            }
        }


        public void SetNormal(int index, Normal normal)
        {

            _originalNormals[index] = normal;
        }

        Normal[] IGeometryDetailLevel.GetNormals()
        {
            return _originalNormals;
        }
    }
}
#endif