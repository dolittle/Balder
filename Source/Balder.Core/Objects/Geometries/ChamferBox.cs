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

using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public class ChamferBox : Box
	{
		public static readonly Property<ChamferBox, float> FilletProperty = Property<ChamferBox, float>.Register(c => c.Fillet,0);
		public float Fillet
		{
			get { return FilletProperty.GetValue(this); }
			set
			{
				FilletProperty.SetValue(this,value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<ChamferBox, int> FilletSegmentsProperty = Property<ChamferBox, int>.Register(c => c.FilletSegments,2);
		public int FilletSegments
		{
			get { return FilletSegmentsProperty.GetValue(this); }
			set
			{
				FilletSegmentsProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public override void Prepare(Viewport viewport)
		{
			GenerateVertices();
			GenerateTextureCoordinates();
			GenerateFaces();

			GeometryHelper.CalculateFaceNormals(FullDetailLevel);
			GeometryHelper.CalculateVertexNormals(FullDetailLevel);


			//base.Prepare(viewport);
		}

		private void GenerateVertices()
		{
			var dimensionAsVector = (Vector)Dimension;
			var sideDimension = dimensionAsVector;
			sideDimension.X -= Fillet;
			sideDimension.Y -= Fillet;
			sideDimension.Z -= Fillet;
			sideDimension /= 2f;

			// Front
			var frontUpperLeft = new Vertex(-sideDimension.X, sideDimension.Y, -sideDimension.Z);
			var frontUpperRight = new Vertex(sideDimension.X, sideDimension.Y, -sideDimension.Z);
			var frontLowerLeft = new Vertex(-sideDimension.X, -sideDimension.Y, -sideDimension.Z);
			var frontLowerRight = new Vertex(sideDimension.X, -sideDimension.Y, -sideDimension.Z);
			

			// Back
			var backUpperLeft = new Vertex(-sideDimension.X, sideDimension.Y, sideDimension.Z);
			var backUpperRight = new Vertex(sideDimension.X, sideDimension.Y, sideDimension.Z);
			var backLowerLeft = new Vertex(-sideDimension.X, -sideDimension.Y, sideDimension.Z);
			var backLowerRight = new Vertex(sideDimension.X, -sideDimension.Y, sideDimension.Z);


			// Left
			var leftUpperLeft = new Vertex(-sideDimension.X, sideDimension.Y, sideDimension.Z);
			var leftUpperRight = new Vertex(-sideDimension.X, sideDimension.Y, -sideDimension.Z);
			var leftLowerLeft = new Vertex(-sideDimension.X, -sideDimension.Y, sideDimension.Z);
			var leftLowerRight = new Vertex(-sideDimension.X, -sideDimension.Y, -sideDimension.Z);

			// Right
			var rightUpperLeft = new Vertex(sideDimension.X, sideDimension.Y, -sideDimension.Z);
			var rightUpperRight = new Vertex(sideDimension.X, sideDimension.Y, sideDimension.Z);
			var rightLowerLeft = new Vertex(sideDimension.X, -sideDimension.Y, -sideDimension.Z);
			var rightLowerRight = new Vertex(sideDimension.X, -sideDimension.Y, sideDimension.Z);

			// Top
			var topUpperLeft = new Vertex(-sideDimension.X, sideDimension.Y, sideDimension.Z);
			var topUpperRight = new Vertex(sideDimension.X, sideDimension.Y, sideDimension.Z);
			var topLowerLeft = new Vertex(-sideDimension.X, sideDimension.Y, -sideDimension.Z);
			var topLowerRight = new Vertex(sideDimension.X, sideDimension.Y, -sideDimension.Z);


			// Bottom
			var bottomUpperLeft = new Vertex(-sideDimension.X, -sideDimension.Y, -sideDimension.Z);
			var bottomUpperRight = new Vertex(sideDimension.X, -sideDimension.Y, -sideDimension.Z);
			var bottomLowerLeft = new Vertex(-sideDimension.X, -sideDimension.Y, sideDimension.Z);
			var bottomLowerRight = new Vertex(sideDimension.X, -sideDimension.Y, sideDimension.Z);



			FullDetailLevel.AllocateVertices(24);
			FullDetailLevel.SetVertex(0, frontUpperLeft);
			FullDetailLevel.SetVertex(1, frontUpperRight);
			FullDetailLevel.SetVertex(2, frontLowerLeft);
			FullDetailLevel.SetVertex(3, frontLowerRight);

			FullDetailLevel.SetVertex(4, backUpperLeft);
			FullDetailLevel.SetVertex(5, backUpperRight);
			FullDetailLevel.SetVertex(6, backLowerLeft);
			FullDetailLevel.SetVertex(7, backLowerRight);

			FullDetailLevel.SetVertex(8, leftUpperLeft);
			FullDetailLevel.SetVertex(9, leftUpperRight);
			FullDetailLevel.SetVertex(10, leftLowerLeft);
			FullDetailLevel.SetVertex(11, leftLowerRight);

			FullDetailLevel.SetVertex(12, rightUpperLeft);
			FullDetailLevel.SetVertex(13, rightUpperRight);
			FullDetailLevel.SetVertex(14, rightLowerLeft);
			FullDetailLevel.SetVertex(15, rightLowerRight);

			FullDetailLevel.SetVertex(16, topUpperLeft);
			FullDetailLevel.SetVertex(17, topUpperRight);
			FullDetailLevel.SetVertex(18, topLowerLeft);
			FullDetailLevel.SetVertex(19, topLowerRight);

			FullDetailLevel.SetVertex(20, bottomUpperLeft);
			FullDetailLevel.SetVertex(21, bottomUpperRight);
			FullDetailLevel.SetVertex(22, bottomLowerLeft);
			FullDetailLevel.SetVertex(23, bottomLowerRight);

		}

		private void GenerateFaces()
		{
			FullDetailLevel.AllocateFaces(12);

			SetFace(0, 2, 1, 0, Vector.Backward, 2, 1, 0);
			SetFace(1, 1, 2, 3, Vector.Backward, 1, 2, 3);

			SetFace(2, 4, 5, 6, Vector.Forward, 1, 0, 3);
			SetFace(3, 7, 6, 5, Vector.Forward, 2, 3, 0);

			SetFace(4, 10, 9, 8, Vector.Left, 2, 1, 0);
			SetFace(5, 9, 10, 11, Vector.Left, 1, 2, 3);


			SetFace(6, 14, 13, 12, Vector.Right, 2, 1, 0);
			SetFace(7, 13, 14, 15, Vector.Right, 1, 2, 3);


			SetFace(8, 18, 17, 16, Vector.Up, 2, 1, 0);
			SetFace(9, 17, 18, 19, Vector.Up, 1, 2, 3);


			SetFace(10, 22, 21, 20, Vector.Down, 2, 1, 0);
			SetFace(11, 21, 22, 23, Vector.Down, 1, 2, 3);

		}

		private void GenerateTextureCoordinates()
		{
			FullDetailLevel.AllocateTextureCoordinates(4);
			FullDetailLevel.SetTextureCoordinate(0, new TextureCoordinate(0f, 0f));
			FullDetailLevel.SetTextureCoordinate(1, new TextureCoordinate(1f, 0f));
			FullDetailLevel.SetTextureCoordinate(2, new TextureCoordinate(0f, 1f));
			FullDetailLevel.SetTextureCoordinate(3, new TextureCoordinate(1f, 1f));
		}

	}
}
