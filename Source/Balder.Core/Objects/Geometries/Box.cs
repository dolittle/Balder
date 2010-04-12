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

using System.Collections.Generic;
using Balder.Core.Execution;
using Balder.Core.Materials;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public enum BoxSide
	{
		Front = 0,
		Back,
		Left,
		Right,
		Top,
		Bottom
	}

	public class Box : Geometry
	{
		private readonly Dictionary<BoxSide, Material> _materials;

		public Box()
		{
			_materials = new Dictionary<BoxSide, Material>();
		}


		public void SetMaterialOnSide(BoxSide side, Material material)
		{
			_materials[side] = material;
			InvalidatePrepare();
		}

		public static readonly Property<Box, Coordinate> DimensionProperty = Property<Box, Coordinate>.Register(p => p.Dimension);
		public Coordinate Dimension
		{
			get { return DimensionProperty.GetValue(this); }
			set
			{
				DimensionProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		protected override void Prepare()
		{
			GenerateVertices();
			GenereateTextureCoordinate();
			GenerateFaces();

			GeometryHelper.CalculateFaceNormals(GeometryContext);
			GeometryHelper.CalculateVertexNormals(GeometryContext);
			InitializeBoundingSphere();
		}

		private void GenerateVertices()
		{
			var dimensionAsVector = (Vector)Dimension;
			var halfDimension = dimensionAsVector / 2f;

			var frontUpperRight = new Vertex(-halfDimension.X, halfDimension.Y, -halfDimension.Z);
			var frontUpperLeft = new Vertex(halfDimension.X, halfDimension.Y, -halfDimension.Z);
			var frontLowerRight = new Vertex(-halfDimension.X, -halfDimension.Y, -halfDimension.Z);
			var frontLowerLeft = new Vertex(halfDimension.X, -halfDimension.Y, -halfDimension.Z);

			var backUpperRight = new Vertex(-halfDimension.X, halfDimension.Y, halfDimension.Z);
			var backUpperLeft = new Vertex(halfDimension.X, halfDimension.Y, halfDimension.Z);
			var backLowerRight = new Vertex(-halfDimension.X, -halfDimension.Y, halfDimension.Z);
			var backLowerLeft = new Vertex(halfDimension.X, -halfDimension.Y, halfDimension.Z);

			GeometryContext.AllocateVertices(8);
			GeometryContext.SetVertex(0, frontUpperRight);
			GeometryContext.SetVertex(1, frontUpperLeft);
			GeometryContext.SetVertex(2, frontLowerRight);
			GeometryContext.SetVertex(3, frontLowerLeft);

			GeometryContext.SetVertex(4, backUpperRight);
			GeometryContext.SetVertex(5, backUpperLeft);
			GeometryContext.SetVertex(6, backLowerRight);
			GeometryContext.SetVertex(7, backLowerLeft);
		}

		private void GenereateTextureCoordinate()
		{
			GeometryContext.AllocateTextureCoordinates(4);
			GeometryContext.SetTextureCoordinate(0, new TextureCoordinate(0f, 0f));
			GeometryContext.SetTextureCoordinate(1, new TextureCoordinate(1f, 0f));
			GeometryContext.SetTextureCoordinate(2, new TextureCoordinate(0f, 1f));
			GeometryContext.SetTextureCoordinate(3, new TextureCoordinate(1f, 1f));
		}


		private BoxSide GetBoxSideFromNormal(Vector normal)
		{
			if (normal.Equals(Vector.Backward))
			{
				return BoxSide.Front;
			}
			if (normal.Equals(Vector.Forward))
			{
				return BoxSide.Back;
			}
			if (normal.Equals(Vector.Right))
			{
				return BoxSide.Left;
			}
			if (normal.Equals(Vector.Left))
			{
				return BoxSide.Right;
			}
			if (normal.Equals(Vector.Up))
			{
				return BoxSide.Top;
			}
			if (normal.Equals(Vector.Down))
			{
				return BoxSide.Bottom;
			}
			return BoxSide.Front;
		}

		private void SetFace(int faceIndex, int a, int b, int c, Vector normal, int diffuseA, int diffuseB, int diffuseC)
		{
			var face = new Face(a, b, c) { Normal = normal, DiffuseA = diffuseA, DiffuseB = diffuseB, DiffuseC = diffuseC };
			var boxSide = GetBoxSideFromNormal(normal);
			if (_materials.ContainsKey(boxSide))
			{
				var material = _materials[boxSide];
				if (null != material)
				{
					face.Material = material;
				}
			}
			GeometryContext.SetFace(faceIndex, face);
		}

		private void GenerateFaces()
		{
			GeometryContext.AllocateFaces(12);

			SetFace(0, 2, 1, 0, Vector.Backward, 2, 1, 0);
			SetFace(1, 1, 2, 3, Vector.Backward, 1, 2, 3);

			SetFace(2, 4, 5, 6, Vector.Forward, 1, 0, 3);
			SetFace(3, 7, 6, 5, Vector.Forward, 2, 3, 0);

			SetFace(4, 0, 4, 2, Vector.Left, 1, 0, 3);
			SetFace(5, 6, 2, 4, Vector.Left, 2, 3, 0);


			SetFace(6, 3, 5, 1, Vector.Right, 2, 1, 0);
			SetFace(7, 5, 3, 7, Vector.Right, 1, 2, 3);


			SetFace(8, 0, 1, 4, Vector.Up, 2, 3, 0);
			SetFace(9, 5, 4, 1, Vector.Up, 1, 0, 3);


			SetFace(10, 6, 3, 2, Vector.Down, 2, 1, 0);
			SetFace(11, 3, 6, 7, Vector.Down, 1, 2, 3);
		}
	}
}
