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
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;
#if(SILVERLIGHT)
using Ninject;
#endif


namespace Balder.Core.Objects.Geometries
{
	public class ChamferBox : Box
	{

#if(SILVERLIGHT)
		public ChamferBox()
			: this(Runtime.Instance.Kernel.Get<IGeometryContext>(),
					Runtime.Instance.Kernel.Get<IIdentityManager>())
		{
			
		}
#endif


		public ChamferBox(IGeometryContext geometryContext, IIdentityManager identityManager)
			: base(geometryContext, identityManager)
		{
		}

		public static readonly Property<ChamferBox, double> FilletProperty = Property<ChamferBox, double>.Register(c => c.Fillet, 0);
		public double Fillet
		{
			get { return FilletProperty.GetValue(this); }
			set
			{
				FilletProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<ChamferBox, int> FilletSegmentsProperty = Property<ChamferBox, int>.Register(c => c.FilletSegments, 1);
		public int FilletSegments
		{
			get { return FilletSegmentsProperty.GetValue(this); }
			set
			{
				if (value < 1)
				{
					throw new ArgumentException("Number of fillet segments must at least be 1");
				}
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
		}

		private void GenerateVertices()
		{
			var filletAsFloat = (float)Fillet;
			var dimensionAsVector = (Vector)Dimension;
			var sideDimension = dimensionAsVector;
			sideDimension.X -= filletAsFloat;
			sideDimension.Y -= filletAsFloat;
			sideDimension.Z -= filletAsFloat;
			sideDimension /= 2f;

			var sidePosition = dimensionAsVector / 2;


			// Front
			var frontUpperLeft = new Vertex(-sideDimension.X, sideDimension.Y, -sidePosition.Z);
			var frontUpperRight = new Vertex(sideDimension.X, sideDimension.Y, -sidePosition.Z);
			var frontLowerLeft = new Vertex(-sideDimension.X, -sideDimension.Y, -sidePosition.Z);
			var frontLowerRight = new Vertex(sideDimension.X, -sideDimension.Y, -sidePosition.Z);


			// Back
			var backUpperLeft = new Vertex(-sideDimension.X, sideDimension.Y, sidePosition.Z);
			var backUpperRight = new Vertex(sideDimension.X, sideDimension.Y, sidePosition.Z);
			var backLowerLeft = new Vertex(-sideDimension.X, -sideDimension.Y, sidePosition.Z);
			var backLowerRight = new Vertex(sideDimension.X, -sideDimension.Y, sidePosition.Z);


			// Left
			var leftUpperLeft = new Vertex(-sidePosition.X, sideDimension.Y, sideDimension.Z);
			var leftUpperRight = new Vertex(-sidePosition.X, sideDimension.Y, -sideDimension.Z);
			var leftLowerLeft = new Vertex(-sidePosition.X, -sideDimension.Y, sideDimension.Z);
			var leftLowerRight = new Vertex(-sidePosition.X, -sideDimension.Y, -sideDimension.Z);

			// Right
			var rightUpperLeft = new Vertex(sidePosition.X, sideDimension.Y, -sideDimension.Z);
			var rightUpperRight = new Vertex(sidePosition.X, sideDimension.Y, sideDimension.Z);
			var rightLowerLeft = new Vertex(sidePosition.X, -sideDimension.Y, -sideDimension.Z);
			var rightLowerRight = new Vertex(sidePosition.X, -sideDimension.Y, sideDimension.Z);

			// Top
			var topUpperLeft = new Vertex(-sideDimension.X, sidePosition.Y, sideDimension.Z);
			var topUpperRight = new Vertex(sideDimension.X, sidePosition.Y, sideDimension.Z);
			var topLowerLeft = new Vertex(-sideDimension.X, sidePosition.Y, -sideDimension.Z);
			var topLowerRight = new Vertex(sideDimension.X, sidePosition.Y, -sideDimension.Z);


			// Bottom
			var bottomUpperLeft = new Vertex(-sideDimension.X, -sidePosition.Y, -sideDimension.Z);
			var bottomUpperRight = new Vertex(sideDimension.X, -sidePosition.Y, -sideDimension.Z);
			var bottomLowerLeft = new Vertex(-sideDimension.X, -sidePosition.Y, sideDimension.Z);
			var bottomLowerRight = new Vertex(sideDimension.X, -sidePosition.Y, sideDimension.Z);


			if (FilletSegments > 1)
			{
				GenerateFilletVertices(sideDimension);
			}

			var sideVertices = 24;
			FullDetailLevel.AllocateVertices(sideVertices);


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

		private void GenerateFilletVertices(Vector sideDimension)
		{
			var filletVertices = ((FilletSegments - 1) * 2) * 2;
			var filletVertexOffset = 24;
			var totalVertices = filletVertexOffset + filletVertices;
			FullDetailLevel.AllocateVertices(totalVertices);

			var filletStep = (float)System.Math.PI / FilletSegments;


			// Front - Top
			GenerateFillet(-sideDimension.Z,
						   sideDimension.Y,
						   sideDimension.X,
						   filletStep,
						   ref filletVertexOffset,
						   (v, x) => v.Z = x,
						   (v, y) => v.Y = y,
						   (v, d) => v.X = d);
			// Left - top
			/*
			GenerateFillet(-sideDimension.X,
						   sideDimension.Y,
						   sideDimension.Z,
						   filletStep,
						   ref filletVertexOffset,
						   (v, x) => v.X = x,
						   (v, y) => v.Y = y,
						   (v, d) => v.Z = d);*/

			/*
			for (var filletIndex = 1; filletIndex < FilletSegments; filletIndex++)
			{
				var filletSegmentYPosition = sideDimension.Y + ((float)System.Math.Sin(filletStep * filletIndex) * Fillet);
				var filletSegmentZPosition = -sideDimension.Z + ((float)System.Math.Cos(filletStep * filletIndex) * Fillet);
				var leftVertex = new Vertex(-sideDimension.X, filletSegmentYPosition, filletSegmentZPosition);
				var rightVertex = new Vertex(sideDimension.X, filletSegmentYPosition, filletSegmentZPosition);
				FullDetailLevel.SetVertex(filletVertexOffset++, leftVertex);
				FullDetailLevel.SetVertex(filletVertexOffset++, rightVertex);
			}*/

		}

		private void GenerateFillet(float xDimension,
									float yDimension,
									float sideDimension,
									float filletStep,
									ref int filletVertexOffset,
									Action<Vertex, float> filletXSet,
									Action<Vertex, float> filletYSet,
									Action<Vertex, float> sideDimensionSet)
		{
			for (var filletIndex = 1; filletIndex < FilletSegments; filletIndex++)
			{
				var filletSegmentYPosition = (float)(yDimension + (System.Math.Sin(filletStep * filletIndex) * Fillet));
				var filletSegmentXPosition = (float)(xDimension + (System.Math.Cos(filletStep * filletIndex) * Fillet));
				var leftVertex = new Vertex();
				filletXSet(leftVertex, filletSegmentXPosition);
				filletYSet(leftVertex, filletSegmentYPosition);
				sideDimensionSet(leftVertex, -sideDimension);

				var rightVertex = new Vertex();
				filletXSet(rightVertex, filletSegmentXPosition);
				filletYSet(rightVertex, filletSegmentYPosition);
				sideDimensionSet(rightVertex, -sideDimension);

				FullDetailLevel.SetVertex(filletVertexOffset++, leftVertex);
				FullDetailLevel.SetVertex(filletVertexOffset++, rightVertex);
			}
		}


		private void GenerateFaces()
		{
			var filletFaces = 0;
			if (FilletSegments == 1)
			{
				filletFaces = 32;
			}
			else
			{
				filletFaces = 0;
			}

			var sideFaces = 12;
			var totalFaces = sideFaces + filletFaces;

			FullDetailLevel.AllocateFaces(totalFaces);

			SetFace(0, 2, 1, 0, Vector.Backward, 2, 1, 0,0);
			SetFace(1, 1, 2, 3, Vector.Backward, 1, 2, 3,0);

			SetFace(2, 4, 5, 6, Vector.Forward, 1, 0, 3,0);
			SetFace(3, 7, 6, 5, Vector.Forward, 2, 3, 0,0);

			SetFace(4, 10, 9, 8, Vector.Left, 2, 1, 0,0);
			SetFace(5, 9, 10, 11, Vector.Left, 1, 2, 3,0);


			SetFace(6, 14, 13, 12, Vector.Right, 2, 1, 0,0);
			SetFace(7, 13, 14, 15, Vector.Right, 1, 2, 3,0);


			SetFace(8, 18, 17, 16, Vector.Up, 2, 1, 0,0);
			SetFace(9, 17, 18, 19, Vector.Up, 1, 2, 3,0);


			SetFace(10, 22, 21, 20, Vector.Down, 2, 1, 0,0);
			SetFace(11, 21, 22, 23, Vector.Down, 1, 2, 3,0);

			if (FilletSegments == 1)
			{
				// Upper front fillet
				SetFace(12, 19, 18, 0, Vector.Zero, 0, 0, 0,1);
				SetFace(13, 19, 0, 1, Vector.Zero, 0, 0, 0,1);

				// Upper front left corner fillet
				SetFace(14, 18, 9, 0, Vector.Zero, 0, 0, 0,1);

				// Upper left fillet
				SetFace(15, 8, 18, 16, Vector.Zero, 0, 0, 0, 1);
				SetFace(16, 18, 8, 9, Vector.Zero, 0, 0, 0, 1);

				// Upper back left corner fillet
				SetFace(17, 4, 8, 16, Vector.Zero, 0, 0, 0, 1);

				// Upper back fillet
				SetFace(18, 5, 4, 16, Vector.Zero, 0, 0, 0, 1);
				SetFace(19, 17, 5, 16, Vector.Zero, 0, 0, 0, 1);

				// Upper back right corner fillet
				SetFace(20, 13, 5, 17, Vector.Zero, 0, 0, 0, 1);

				// Upper right fillet
				SetFace(21, 12, 13, 17, Vector.Zero, 0, 0, 0, 1);
				SetFace(22, 12, 17, 19, Vector.Zero, 0, 0, 0, 1);

				// Upper front right corner fillet
				SetFace(23, 19, 1, 12, Vector.Zero, 0, 0, 0, 1);


				// Lower front fillet
				SetFace(24, 2, 20, 3, Vector.Zero, 0, 0, 0, 1);
				SetFace(25, 3, 20, 21, Vector.Zero, 0, 0, 0, 1);

				// Lower front left corner fillet
				SetFace(26, 20, 2, 11, Vector.Zero, 0, 0, 0, 1);

				// Lower left fillet
				SetFace(27, 22, 11, 10, Vector.Zero, 0, 0, 0, 1);
				SetFace(28, 11, 22, 20, Vector.Zero, 0, 0, 0, 1);


				// Lower back left corner fillet
				SetFace(29, 10, 6, 22, Vector.Zero, 0, 0, 0, 1);

				// Lower back fillet
				SetFace(30, 22, 6, 7, Vector.Zero, 0, 0, 0, 1);
				SetFace(31, 7, 23, 22, Vector.Zero, 0, 0, 0, 1);

				// Lower back right corner fillet
				SetFace(32, 23, 7, 15, Vector.Zero, 0, 0, 0, 1);


				// Lower right fillet
				SetFace(33, 21, 23, 15, Vector.Zero, 0, 0, 0, 1);
				SetFace(34, 21, 15, 14, Vector.Zero, 0, 0, 0, 1);

				// Lower front right corner fillet
				SetFace(35, 21, 14, 3, Vector.Zero, 0, 0, 0, 1);




				// Left front fillet
				SetFace(36, 11, 0, 9, Vector.Zero, 0, 0, 0, 1);
				SetFace(37, 0, 11, 2, Vector.Zero, 0, 0, 0, 1);

				// Left back fillet
				SetFace(38, 8, 4, 6, Vector.Zero, 0, 0, 0, 1);
				SetFace(39, 8, 6, 10, Vector.Zero, 0, 0, 0, 1);

				// Right back fillet
				SetFace(40, 7, 5, 13, Vector.Zero, 0, 0, 0, 1);
				SetFace(41, 13, 15, 7, Vector.Zero, 0, 0, 0, 1);

				// Right front fillet
				SetFace(42, 14, 12, 1, Vector.Zero, 0, 0, 0, 1);
				SetFace(43, 3, 14, 1, Vector.Zero, 0, 0, 0, 1);
			}
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
