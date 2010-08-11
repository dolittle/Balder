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
using Balder.Execution;
using Balder.Math;
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif

namespace Balder.Objects.Geometries
{
	public class Ring : GeneratedGeometry
	{
		private const int GeneralSmoothingGroup = 0;
		private const int EndsSmoothingGroup = 1;
		private const int SpokesSmoothingGroup = 2;


		public static readonly Property<Ring, double> InnerRadiusProp = Property<Ring, double>.Register(c => c.InnerRadius);
		public double InnerRadius
		{
			get { return InnerRadiusProp.GetValue(this); }
			set
			{
				InnerRadiusProp.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<Ring, double> OuterRadiusProp = Property<Ring, double>.Register(c => c.OuterRadius);
		public double OuterRadius
		{
			get { return OuterRadiusProp.GetValue(this); }
			set
			{
				OuterRadiusProp.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<Ring, bool> CapEndsProp = Property<Ring, bool>.Register(c => c.CapEnds);
		public bool CapEnds
		{
			get { return CapEndsProp.GetValue(this); }
			set
			{
				CapEndsProp.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<Ring, bool> SpokesProp = Property<Ring, bool>.Register(c => c.Spokes);
		public bool Spokes
		{
			get { return SpokesProp.GetValue(this); }
			set
			{
				SpokesProp.SetValue(this, value);
				InvalidatePrepare();
			}
		}



		public static readonly Property<Ring, int> SegmentsProp = Property<Ring, int>.Register(c => c.Segments);
		public int Segments
		{
			get { return SegmentsProp.GetValue(this); }
			set
			{
				SegmentsProp.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<Ring, int> StacksProp = Property<Ring, int>.Register(c => c.Stacks);
		public int Stacks
		{
			get { return StacksProp.GetValue(this); }
			set
			{
				StacksProp.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<Ring, double> SizeProperty = Property<Ring, double>.Register(c => c.Size);
		public double Size
		{
			get { return SizeProperty.GetValue(this); }
			set
			{
				SizeProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<Ring, double> StartAngleProperty = Property<Ring, double>.Register(c => c.StartAngle);
		public double StartAngle
		{
			get { return StartAngleProperty.GetValue(this); }
			set
			{
				StartAngleProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static readonly Property<Ring, double> EndAngleProperty = Property<Ring, double>.Register(c => c.EndAngle);
		public double EndAngle
		{
			get { return EndAngleProperty.GetValue(this); }
			set
			{
				EndAngleProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

#if(DEFAULT_CONSTRUCTOR)
		public Ring()
			: this(Runtime.Instance.Kernel.Get<IGeometryContext>())
		{
		}
#endif

		public Ring(IGeometryContext geometryContext)
			: base(geometryContext)
		{
			StartAngle = 0;
			EndAngle = 360;
			Segments = 8;
			Stacks = 1;
			CapEnds = true;
			Spokes = true;
		}


		private void Validate()
		{
			if (InnerRadius <= 0 || OuterRadius <= 0)
			{
				throw new ArgumentException("Top and Bottom radius must be set to a number higher than 0");
			}

			if (InnerRadius >= OuterRadius)
			{
				throw new ArgumentException("Inner radius must be less than outer radius");
			}

			if (Segments <= 2)
			{
				throw new ArgumentException("You must have at least 2 segments");
			}

			if (StartAngle > EndAngle)
			{
				throw new ArgumentException("StartAngle must be less than EndAngle");
			}

			if (StartAngle < 0 || EndAngle < 0)
			{
				throw new ArgumentException("Start or End angle must be 0 or more");
			}

			if (StartAngle > 360 || EndAngle > 360)
			{
				throw new ArgumentException("Start or End angle must be 360 or less");
			}

			if (Stacks < 1)
			{
				throw new ArgumentException("You must have at least 1 stack");
			}
		}

		public override void Prepare(Viewport viewport)
		{
			Validate();

			var actualStacks = Stacks + 1;
			var actualSegments = Segments;

			var open = false;
			if (StartAngle > 0 || EndAngle < 360)
			{
				open = true;
			}

			var vertexCount = (actualSegments * 2) * actualStacks;
			BuildVertices(actualSegments, actualStacks, vertexCount, open);
			BuildFaces(actualSegments, actualStacks, vertexCount);

			GeometryHelper.CalculateNormals(FullDetailLevel);
			//InitializeBoundingSphere();

			base.Prepare(viewport);
		}

		private void BuildFaces(int actualSegments, int actualStacks, int vertexCount)
		{
			var faceCount = 0;
			var verticesPerStack = actualSegments * 2;

			var openRing = false;
			if (StartAngle > 0 || EndAngle < 360)
			{
				actualSegments--;
				openRing = true;

				if (Spokes)
				{
					faceCount += (4 * Stacks);
				} 
			}

			var topFaceCount = actualSegments * 2;
			var bottomFaceCount = actualSegments * 2;
			var innerSidesFaceCount = actualSegments * 2;
			var outerSidesFaceCount = actualSegments * 2;
			faceCount += (innerSidesFaceCount + outerSidesFaceCount) * Stacks;
			if (CapEnds)
			{
				faceCount += topFaceCount;
				faceCount += bottomFaceCount;
			}


			FullDetailLevel.AllocateFaces(faceCount);

			var faceIndex = 0;

			Face face;

			for (var stack = 0; stack < actualStacks - 1; stack++)
			{
				var currentStack = (verticesPerStack * stack);
				var nextStack = currentStack + verticesPerStack;
				if (openRing&&Spokes)
				{
					face = CreateFace(nextStack + 1, nextStack, currentStack, SpokesSmoothingGroup);
					FullDetailLevel.SetFace(faceIndex, face);
					faceIndex++;

					face = CreateFace(nextStack + 1, currentStack, currentStack + 1, SpokesSmoothingGroup);
					FullDetailLevel.SetFace(faceIndex, face);
					faceIndex++;
				}
				faceIndex = BuildInteriorAndExteriorFaces(actualSegments, verticesPerStack, currentStack, faceIndex);
				if (openRing && Spokes)
				{
					currentStack += (verticesPerStack - 2);
					nextStack += (verticesPerStack - 2);
					face = CreateFace(currentStack, nextStack, nextStack + 1, SpokesSmoothingGroup);
					FullDetailLevel.SetFace(faceIndex, face);
					faceIndex++;

					face = CreateFace(currentStack + 1, currentStack, nextStack + 1, SpokesSmoothingGroup);
					FullDetailLevel.SetFace(faceIndex, face);
					faceIndex++;
				}
			}

			if (CapEnds)
			{
				faceIndex = BuildCapFaces(vertexCount, actualSegments, verticesPerStack, faceIndex);
			}
		}

		private int BuildInteriorAndExteriorFaces(int actualSegments, int verticesPerStack, int currentStack, int faceIndex)
		{
			Face face;
			for (var segment = 0; segment < actualSegments; segment++)
			{
				var segmentOffset = (segment * 2) + currentStack;
				var nextSegmentOffset = (((segment * 2) + 2) % verticesPerStack) + currentStack;
				var nextStackOffset = segmentOffset + (verticesPerStack);
				var nextStackAndSegmentOffset = nextSegmentOffset + (verticesPerStack);

				face = CreateFace(nextStackOffset, nextSegmentOffset, segmentOffset, GeneralSmoothingGroup);
				FullDetailLevel.SetFace(faceIndex, face);
				faceIndex++;
				face = CreateFace(nextSegmentOffset, nextStackOffset, nextStackAndSegmentOffset, GeneralSmoothingGroup);
				FullDetailLevel.SetFace(faceIndex, face);
				faceIndex++;


				face = CreateFace(segmentOffset + 1, nextSegmentOffset + 1, nextStackOffset + 1, GeneralSmoothingGroup);
				FullDetailLevel.SetFace(faceIndex, face);
				faceIndex++;
				face = CreateFace(nextStackAndSegmentOffset + 1, nextStackOffset + 1, nextSegmentOffset + 1, GeneralSmoothingGroup);
				FullDetailLevel.SetFace(faceIndex, face);
				faceIndex++;
			}
			return faceIndex;
		}

		private int BuildCapFaces(int vertexCount, int actualSegments, int verticesPerStack, int faceIndex)
		{
			Face face;
			for (var segment = 0; segment < actualSegments; segment++)
			{
				var segmentOffset = segment * 2;
				var nextSegmentOffset = (segmentOffset + 2) % verticesPerStack;

				face = CreateFace(nextSegmentOffset + 1, segmentOffset + 1, segmentOffset, EndsSmoothingGroup);
				FullDetailLevel.SetFace(faceIndex, face);
				faceIndex++;
				face = CreateFace(nextSegmentOffset, nextSegmentOffset + 1, segmentOffset, EndsSmoothingGroup);
				FullDetailLevel.SetFace(faceIndex, face);
				faceIndex++;
			}

			var bottomCapOffset = vertexCount - verticesPerStack;
			for (var segment = 0; segment < actualSegments; segment++)
			{
				var segmentOffset = bottomCapOffset + (segment * 2);
				var nextSegmentOffset = bottomCapOffset + ((segmentOffset + 2) % verticesPerStack);

				face = CreateFace(segmentOffset, segmentOffset + 1, nextSegmentOffset + 1, EndsSmoothingGroup);
				FullDetailLevel.SetFace(faceIndex, face);
				faceIndex++;
				face = CreateFace(segmentOffset, nextSegmentOffset + 1, nextSegmentOffset, EndsSmoothingGroup);
				FullDetailLevel.SetFace(faceIndex, face);
				faceIndex++;
			}
			return faceIndex;
		}

		private void BuildVertices(int actualSegments, int actualStacks, int vertexCount, bool open)
		{
			FullDetailLevel.AllocateVertices(vertexCount);
			FullDetailLevel.AllocateTextureCoordinates(vertexCount);

			var yStart = (float)Size / 2;
			var yEnd = (float)-Size / 2;

			var yDelta = yEnd - yStart;
			var yAdd = yDelta / (actualStacks-1);

			var startRadian = MathHelper.ToRadians((float)StartAngle);
			var endRadian = MathHelper.ToRadians((float)EndAngle);
			var deltaRadian = endRadian - startRadian;

			var addRadian = deltaRadian / actualSegments;
			if( open )
			{
				addRadian = deltaRadian / (actualSegments-1);
			}

			var uDelta = 1f;
			var vDelta = 2f;

			var uAdd = uDelta / actualSegments;
			var vAdd = vDelta / actualStacks;

			var vertexIndex = 0;
			var currentY = yStart;
			var currentV = 0f;

			for (var stack = 0; stack < actualStacks; stack++)
			{
				var currentU = 0f;
				var currentRadian = startRadian;
				for (var segment = 0; segment < actualSegments; segment++)
				{
					var innerX = (float)(System.Math.Sin(currentRadian) * InnerRadius);
					var innerZ = (float)(System.Math.Cos(currentRadian) * InnerRadius);

					var innerVertex = new Vertex(innerX, currentY, innerZ);
					FullDetailLevel.SetVertex(vertexIndex, innerVertex);

					var textureCoordinate = new TextureCoordinate(1f - currentU, 0);
					FullDetailLevel.SetTextureCoordinate(vertexIndex, textureCoordinate);

					vertexIndex++;

					var outerX = (float)(System.Math.Sin(currentRadian) * OuterRadius);
					var outerZ = (float)(System.Math.Cos(currentRadian) * OuterRadius);

					var outerVertex = new Vertex(outerX, currentY, outerZ);
					FullDetailLevel.SetVertex(vertexIndex, outerVertex);

					textureCoordinate = new TextureCoordinate(1f - currentU, 1);
					FullDetailLevel.SetTextureCoordinate(vertexIndex, textureCoordinate);

					vertexIndex++;

					currentRadian += addRadian;
					currentU += uAdd;
				}

				currentY += yAdd;
				currentV += vAdd;
			}
		}

		private new Face CreateFace(int a, int b, int c, int smoothingGroup)
		{
			var face = base.CreateFace(a, b, c);
			face.DiffuseA = a;
			face.DiffuseB = b;
			face.DiffuseC = c;
			face.SmoothingGroup = smoothingGroup;
			return face;
		}
	}
}
