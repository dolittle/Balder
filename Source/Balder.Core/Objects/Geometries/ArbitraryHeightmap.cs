#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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
using System.Windows.Media;
using Balder.Core.Execution;
using Balder.Core.Math;
using Matrix=Balder.Core.Math.Matrix;

namespace Balder.Core.Objects.Geometries
{
	/// <summary>
	/// Represents a heightmap were the 4 corners can be placed arbitrary in 3D space
	/// </summary>
	/// <remarks>
	/// Point 1 and 2 represents one side of the quad
	/// Point 3 and 4 represents the other side
	/// </remarks>
	public class ArbitraryHeightmap : Geometry
	{
		private static readonly HeightmapEventArgs EventArgs = new HeightmapEventArgs();
		public event EventHandler<HeightmapEventArgs> HeightInput;

		private Vertex[] _vertices;


		public ArbitraryHeightmap()
		{
			LengthSegments = 1;
			HeightSegments = 1;
		}


		public static Property<ArbitraryHeightmap, int> LengthSegmentsProperty = Property<ArbitraryHeightmap, int>.Register(p => p.LengthSegments);
		public int LengthSegments
		{
			get { return LengthSegmentsProperty.GetValue(this); }
			set
			{
				LengthSegmentsProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static Property<ArbitraryHeightmap, int> HeightSegmentsProperty = Property<ArbitraryHeightmap, int>.Register(p => p.HeightSegments);
		public int HeightSegments
		{
			get { return HeightSegmentsProperty.GetValue(this); }
			set
			{
				HeightSegmentsProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static Property<ArbitraryHeightmap, Coordinate> Point1Property = Property<ArbitraryHeightmap, Coordinate>.Register(p => p.Point1);
		public Coordinate Point1
		{
			get { return Point1Property.GetValue(this); }
			set
			{
				Point1Property.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static Property<ArbitraryHeightmap, Coordinate> Point2Property = Property<ArbitraryHeightmap, Coordinate>.Register(p => p.Point2);
		public Coordinate Point2
		{
			get { return Point2Property.GetValue(this); }
			set
			{
				Point2Property.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static Property<ArbitraryHeightmap, Coordinate> Point3Property = Property<ArbitraryHeightmap, Coordinate>.Register(p => p.Point3);
		public Coordinate Point3
		{
			get { return Point3Property.GetValue(this); }
			set
			{
				Point3Property.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static Property<ArbitraryHeightmap, Coordinate> Point4Property = Property<ArbitraryHeightmap, Coordinate>.Register(p => p.Point4);
		public Coordinate Point4
		{
			get { return Point4Property.GetValue(this); }
			set
			{
				Point4Property.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public override void BeforeRendering(Display.Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if (null != HeightInput)
			{
				var actualLength = LengthSegments + 1;
				var actualHeight = HeightSegments + 1;

				var vertexIndex = 0;
				//var vertices = GeometryContext.GetVertices();
				for (var y = 0; y < actualHeight; y++)
				{
					var offset = y * actualLength;
					for (var x = 0; x < actualLength; x++)
					{
						var vertex = _vertices[offset + x];
						var heightBefore = vertex.Vector.Y;
						var colorBefore = vertex.Color;
						EventArgs.Color = Colors.Black;
						EventArgs.ActualVertex = vertex;
						EventArgs.GridX = x;
						EventArgs.GridY = y;

						HeightInput(this, EventArgs);

						if (heightBefore != EventArgs.Height ||
							!colorBefore.Equals(EventArgs.Color))
						{
							vertex.Vector = GetVectorHeightFromVertex(vertex, EventArgs.Height);
								//EventArgs.Height;
							vertex.Color = EventArgs.Color;
							GeometryContext.SetVertex(vertexIndex, vertex);
						}

						vertexIndex++;
					}
				}
			}
			base.BeforeRendering(viewport, view, projection, world);
		}

		private Vector GetVectorHeightFromVertex(Vertex vertex, float height)
		{
			var newVector = vertex.Vector + (vertex.Normal*height);
			return newVector;
			
		}


		public void SetHeightForGridPoint(int gridX, int gridY, float height)
		{
			SetHeightForGridPoint(gridX, gridY, height, Colors.Black);
		}


		public void SetHeightForGridPoint(int gridX, int gridY, float height, Color color)
		{
			var actualLength = LengthSegments + 1;
			var actualHeight = HeightSegments + 1;

			if (gridX >= actualLength || gridY >= actualHeight)
			{
				throw new ArgumentException("Point outside grid");
			}

			var index = (gridY * actualLength) + gridX;

			var vertex = _vertices[index];
			vertex.Vector = GetVectorHeightFromVertex(vertex, EventArgs.Height);
				//.Y = height;
			vertex.Color = color;
			GeometryContext.SetVertex(index, vertex);
		}


		private static Interpolator Line1Interpolator = new Interpolator();
		private static Interpolator Line2Interpolator = new Interpolator();
		private static Interpolator ContentInterpolator = new Interpolator();

		static ArbitraryHeightmap()
		{
			Line1Interpolator.SetNumberOfInterpolationPoints(3);
			Line2Interpolator.SetNumberOfInterpolationPoints(3);
			ContentInterpolator.SetNumberOfInterpolationPoints(3);
		}

		public override void Prepare()
		{
			if (LengthSegments <= 0 || HeightSegments <= 0)
			{
				throw new ArgumentException("LengthSegments and HeightSegments must be 1 or more");
			}

			PrepareVertices();
			PrepareFaces();

			GeometryHelper.CalculateFaceNormals(GeometryContext);
			GeometryHelper.CalculateVertexNormals(GeometryContext);

			var vertices = GeometryContext.GetVertices();

			for( var vertexIndex=0; vertexIndex<vertices.Length; vertexIndex++ )
			{
				_vertices[vertexIndex].Normal = vertices[vertexIndex].Normal;
			}

			base.Prepare();
		}


		private void PrepareVertices()
		{
			var actualLength = LengthSegments + 1;
			var actualHeight = HeightSegments + 1;
			var vertexCount = actualLength*actualHeight;
			GeometryContext.AllocateVertices(vertexCount);
			_vertices = new Vertex[vertexCount];


			Line1Interpolator.SetPoint(0, (float)Point1.X, (float)Point2.X);
			Line1Interpolator.SetPoint(1, (float)Point1.Y, (float)Point2.Y);
			Line1Interpolator.SetPoint(2, (float)Point1.Z, (float)Point2.Z);

			Line2Interpolator.SetPoint(0, (float)Point3.X, (float)Point4.X);
			Line2Interpolator.SetPoint(1, (float)Point3.Y, (float)Point4.Y);
			Line2Interpolator.SetPoint(2, (float)Point3.Z, (float)Point4.Z);

			Line1Interpolator.Interpolate(actualHeight);
			Line2Interpolator.Interpolate(actualHeight);

			var vertexIndex = 0;
			for (var point = 0; point < actualHeight; point++)
			{
				var xstart = Line1Interpolator.Points[0].InterpolatedValues[point];
				var ystart = Line1Interpolator.Points[1].InterpolatedValues[point];
				var zstart = Line1Interpolator.Points[2].InterpolatedValues[point];

				var xend = Line2Interpolator.Points[0].InterpolatedValues[point];
				var yend = Line2Interpolator.Points[1].InterpolatedValues[point];
				var zend = Line2Interpolator.Points[2].InterpolatedValues[point];

				ContentInterpolator.SetPoint(0, xstart, xend);
				ContentInterpolator.SetPoint(1, ystart, yend);
				ContentInterpolator.SetPoint(2, zstart, zend);

				ContentInterpolator.Interpolate(actualLength);

				for (var contentPoint = 0; contentPoint < actualLength; contentPoint++)
				{
					var x = ContentInterpolator.Points[0].InterpolatedValues[contentPoint];
					var y = ContentInterpolator.Points[1].InterpolatedValues[contentPoint];
					var z = ContentInterpolator.Points[2].InterpolatedValues[contentPoint];

					var vertex = new Vertex(x, y, z);
					GeometryContext.SetVertex(vertexIndex, vertex);
					_vertices[vertexIndex] = vertex;
					vertexIndex++;
				}
			}
		}


		private void PrepareFaces()
		{
			var actualLength = LengthSegments + 1;
			var actualHeight = HeightSegments + 1;

			var faceCount = ((actualLength - 1) * 2) * (actualHeight - 1);
			GeometryContext.AllocateFaces(faceCount);
			var faceIndex = 0;

			for (var y = 0; y < actualHeight - 1; y++)
			{
				for (var x = 0; x < actualLength - 1; x++)
				{
					var offset = (y * actualLength) + x;
					var offsetNextLine = offset + actualLength;
					var face = new Face(offsetNextLine, offset + 1, offset);
					face.Normal = Vector.Up;
					GeometryContext.SetFace(faceIndex, face);
					face = new Face(offset + 1, offsetNextLine, offsetNextLine + 1);
					face.Normal = Vector.Up;
					GeometryContext.SetFace(faceIndex + 1, face);


					faceIndex += 2;
				}
			}
		}

	}

}
