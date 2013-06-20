#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
using Balder.Display;
using Balder.Execution;
using Balder.Materials;
using Balder.Math;
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif

namespace Balder.Objects.Geometries
{
	public enum BoxSide
	{
		None = -1,
		Front = 0,
		Back,
		Left,
		Right,
		Top,
		Bottom
	}

	public class Box : GeneratedGeometry
	{
		private readonly Dictionary<BoxSide, List<int>> _facesBySide;

#if(DEFAULT_CONSTRUCTOR)
		public Box()
			: this(Runtime.Instance.Kernel.Get<IGeometryContext>())
		{
			
		}
#endif


		public Box(IGeometryContext geometryContext)
			: base(geometryContext)
		{
			_facesBySide = new Dictionary<BoxSide, List<int>>();
		}


		public Material GetMaterialOnSide(BoxSide side)
		{
			var index = (int)side;
			if (!Material.SubMaterials.ContainsKey(index))
			{
				return null;
			}
			return Material.SubMaterials[index];
		}

		public void SetMaterialOnSide(BoxSide side, Material material)
		{
			var index = (int) side;
			if( null == Material )
			{
				Material = new Material();
			}
			Material.SubMaterials[index] = material;
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

		public override void Prepare(Viewport viewport)
		{
			GenerateVertices();
			GenerateTextureCoordinates();
			GenerateFaces();

			GeometryHelper.CalculateNormals(FullDetailLevel);

			base.Prepare(viewport);
		}

		protected void AddFaceToSidesInfo(int face, BoxSide side)
		{
			List<int> faces;
			if (_facesBySide.ContainsKey(side))
			{
				faces = _facesBySide[side];
			}
			else
			{
				faces = new List<int>();
				_facesBySide[side] = faces;
			}
			faces.Add(face);
		}

		private void GenerateVertices()
		{
			var dimensionAsVector = (Vector)Dimension;
			var halfDimension = dimensionAsVector / 2f;

			var frontUpperLeft = new Vertex(-halfDimension.X, halfDimension.Y, -halfDimension.Z);
			var frontUpperRight = new Vertex(halfDimension.X, halfDimension.Y, -halfDimension.Z);
			var frontLowerLeft = new Vertex(-halfDimension.X, -halfDimension.Y, -halfDimension.Z);
			var frontLowerRight = new Vertex(halfDimension.X, -halfDimension.Y, -halfDimension.Z);

			var backUpperLeft = new Vertex(-halfDimension.X, halfDimension.Y, halfDimension.Z);
			var backUpperRight = new Vertex(halfDimension.X, halfDimension.Y, halfDimension.Z);
			var backLowerLeft = new Vertex(-halfDimension.X, -halfDimension.Y, halfDimension.Z);
			var backLowerRight = new Vertex(halfDimension.X, -halfDimension.Y, halfDimension.Z);

			FullDetailLevel.AllocateVertices(8);
			FullDetailLevel.SetVertex(0, frontUpperLeft);
			FullDetailLevel.SetVertex(1, frontUpperRight);
			FullDetailLevel.SetVertex(2, frontLowerLeft);
			FullDetailLevel.SetVertex(3, frontLowerRight);

			FullDetailLevel.SetVertex(4, backUpperLeft);
			FullDetailLevel.SetVertex(5, backUpperRight);
			FullDetailLevel.SetVertex(6, backLowerLeft);
			FullDetailLevel.SetVertex(7, backLowerRight);
		}

		private void GenerateTextureCoordinates()
		{
			FullDetailLevel.AllocateTextureCoordinates(4);
			FullDetailLevel.SetTextureCoordinate(0, new TextureCoordinate(0f, 0f));
			FullDetailLevel.SetTextureCoordinate(1, new TextureCoordinate(1f, 0f));
			FullDetailLevel.SetTextureCoordinate(2, new TextureCoordinate(0f, 1f));
			FullDetailLevel.SetTextureCoordinate(3, new TextureCoordinate(1f, 1f));
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
			if (normal.Equals(Vector.Left))
			{
				return BoxSide.Left;
			}
			if (normal.Equals(Vector.Right))
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
			return BoxSide.None;
		}

		protected void SetFace(int faceIndex, int a, int b, int c, Vector normal, int diffuseA, int diffuseB, int diffuseC, int smoothingGroup)
		{
			var face = CreateFace(a, b, c);
			face.Normal = normal;
			face.DiffuseA = diffuseA;
			face.DiffuseB = diffuseB;
			face.DiffuseC = diffuseC;

			var boxSide = GetBoxSideFromNormal(normal);
			face.MaterialId = (int) boxSide;
			face.SmoothingGroup = smoothingGroup;
			AddFaceToSidesInfo(faceIndex, boxSide);
			FullDetailLevel.SetFace(faceIndex, face);
		}


		private void GenerateFaces()
		{
			FullDetailLevel.AllocateFaces(12);

			SetFace(0, 2, 1, 0, Vector.Backward, 2, 1, 0, 0);
			SetFace(1, 1, 2, 3, Vector.Backward, 1, 2, 3, 0);

			SetFace(2, 4, 5, 6, Vector.Forward, 1, 0, 3, 1);
			SetFace(3, 7, 6, 5, Vector.Forward, 2, 3, 0, 1);

			SetFace(4, 0, 4, 2, Vector.Left, 1, 0, 3, 2);
			SetFace(5, 6, 2, 4, Vector.Left, 2, 3, 0, 2);


			SetFace(6, 3, 5, 1, Vector.Right, 2, 1, 0, 3);
			SetFace(7, 5, 3, 7, Vector.Right, 1, 2, 3, 3);


			SetFace(8, 0, 1, 4, Vector.Up, 2, 3, 0, 4);
			SetFace(9, 5, 4, 1, Vector.Up, 1, 0, 3, 4);


			SetFace(10, 6, 3, 2, Vector.Down, 2, 1, 0, 5);
			SetFace(11, 3, 6, 7, Vector.Down, 1, 2, 3, 5);
		}
	}
}
