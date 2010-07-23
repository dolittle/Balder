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

using System;
using Balder.Display;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;

#if(XNA)
namespace Balder.Rendering.Xna
{
    public class GeometryDetailLevel : IGeometryDetailLevel
    {
        public int FaceCount { get; private set; }
        public int VertexCount { get; private set; }
        public int TextureCoordinateCount { get; private set; }
        public int LineCount { get; private set; }
        public int NormalCount { get; private set; }
        public void AllocateFaces(int count)
        {
            
        }

        public void SetFace(int index, Face face)
        {
            
        }

        public Face[] GetFaces()
        {
            throw new NotImplementedException();
        }

        public void InvalidateFace(int index)
        {
            
        }

        public void AllocateVertices(int count)
        {
            
        }

        public void SetVertex(int index, Vertex vertex)
        {
            throw new NotImplementedException();
        }

        public Vertex[] GetVertices()
        {
            throw new NotImplementedException();
        }

        public void InvalidateVertex(int index)
        {
            
        }

        public void AllocateNormals(int count)
        {
            
        }

        public void SetNormal(int index, Vertex normal)
        {
            
        }

        public Vertex[] GetNormals()
        {
            throw new NotImplementedException();
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

        public void SetMaterial(int index, Material material)
        {
            
        }

        public void SetMaterialForAllFaces(Material material)
        {
            
        }

        public void CalculateVertices(Viewport viewport, INode node)
        {
            
        }

        public void Render(Viewport viewport, INode node)
        {
            
        }
    }
}
#endif