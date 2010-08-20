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
#if(SILVERLIGHT)
using System.Windows.Media;
#else
#if(!IOS)
using Colors = System.Drawing.Color;
#endif
#endif
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif
using Balder.Display;
using Balder.Execution;
using Balder.Math;
using Matrix = Balder.Math.Matrix;

namespace Balder.Objects.Geometries
{
	public class Heightmap : Geometry
	{
		private static readonly HeightmapEventArgs EventArgs = new HeightmapEventArgs();
		public event EventHandler<HeightmapEventArgs> HeightInput;

		private bool _heightsInvalidated;
		private bool _invalidateNormals;

#if(DEFAULT_CONSTRUCTOR)
		public Heightmap()
			: this(Runtime.Instance.Kernel.Get<IGeometryContext>())
		{

		}
#endif


		public Heightmap(IGeometryContext geometryContext)
			: base(geometryContext)
		{
			LengthSegments = 1;
			HeightSegments = 1;
			_heightsInvalidated = true;
		}


		public static Property<Heightmap, int> LengthSegmentsProperty = Property<Heightmap, int>.Register(p => p.LengthSegments);
		public int LengthSegments
		{
			get { return LengthSegmentsProperty.GetValue(this); }
			set
			{
				LengthSegmentsProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static Property<Heightmap, int> HeightSegmentsProperty = Property<Heightmap, int>.Register(p => p.HeightSegments);
		public int HeightSegments
		{
			get { return HeightSegmentsProperty.GetValue(this); }
			set
			{
				HeightSegmentsProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

		public static Property<Heightmap, Dimension> DimensionProperty = Property<Heightmap, Dimension>.Register(p => p.Dimension);
		public Dimension Dimension
		{
			get { return DimensionProperty.GetValue(this); }
			set
			{
				DimensionProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}


		public static Property<Heightmap, bool> UseStaticHeightmapProperty =
			Property<Heightmap, bool>.Register(h => h.UseStaticHeightmap, true);
		public bool UseStaticHeightmap
		{
			get { return UseStaticHeightmapProperty.GetValue(this); }
			set { UseStaticHeightmapProperty.SetValue(this, value); }
		}

		public static Property<Heightmap, float[,]> HeightmapArrayProperty =
			Property<Heightmap, float[,]>.Register(h => h.HeightmapArray);
		public float[,] HeightmapArray
		{
			get { return HeightmapArrayProperty.GetValue(this); }
			set
			{
				HeightmapArrayProperty.SetValue(this, value);
				if( null != value )
				{
					var length = value.GetLength(0);
					var height = value.GetLength(1);
					LengthSegments = length-1;
					HeightSegments = height-1;
				}
				InvalidateHeights();
			}
		}

		public void InvalidateHeights()
		{
			_heightsInvalidated = true;

		}

		private void ValidateArray(float[,] array)
		{
			if( array.GetLength(0) != LengthSegments+1 )
			{
				throw new ArgumentException("First dimension of array is not the same as LengthSegments - this property is not necessary to set manually when using an array");
			}

			if (array.GetLength(1) != HeightSegments+1)
			{
				throw new ArgumentException("Second dimension of array is not the same as HeightSegments - this property is not necessary to set manually when using an array");
			}
		}

		private void SetHeights()
		{
			var actualLength = LengthSegments + 1;
			var actualHeight = HeightSegments + 1;
			var changes = false;

			
			var array = HeightmapArray;
			if( null != array )
			{
				ValidateArray(array);
			}

			var vertexIndex = 0;
			var vertices = FullDetailLevel.GetVertices();
			for (var y = 0; y < actualHeight; y++)
			{
				var offset = y * actualLength;
				for (var x = 0; x < actualLength; x++)
				{
					var vertex = vertices[offset + x];
					var heightBefore = vertex.Y;
					var colorBefore = vertex.Color;

					if (null != HeightInput)
					{
						EventArgs.Color = Colors.Black;
						EventArgs.ActualVertex = vertex;
						EventArgs.GridX = x;
						EventArgs.GridY = y;
						HeightInput(this, EventArgs);
						vertex.Y = EventArgs.Height;
						vertex.Color = EventArgs.Color;
					}
					else
					{
						if( null != array )
						{
							vertex.Y = array[x, y];
							vertex.Color = Colors.White;	
						}
					}

					if (heightBefore != vertex.Y ||
						!colorBefore.Equals(vertex.Color))
					{
						
						FullDetailLevel.SetVertex(vertexIndex, vertex);
						changes = true;
					}

					vertexIndex++;
				}
			}

			if (changes)
			{
				_invalidateNormals = true;
			}
		}


		private bool ShouldSetHeights
		{
			get { return _heightsInvalidated || !UseStaticHeightmap; }
		}



		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if (ShouldSetHeights)
			{
				SetHeights();
			}
			if (_invalidateNormals)
			{
				GeometryHelper.CalculateNormals(FullDetailLevel, false);
			}

			_invalidateNormals = false;
			_heightsInvalidated = false;
			base.BeforeRendering(viewport, view, projection, world);
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

			var xStep = ((float)Dimension.Width) / (float)actualLength;
			var yStep = ((float)Dimension.Height) / (float)actualHeight;
			var yStart = (float)-(Dimension.Height / 2);
			var xStart = (float)-(Dimension.Width / 2);

			var xPos = xStart + (xStep * gridX);
			var zPos = yStart + (yStep * gridY);

			var index = (gridY * actualLength) + gridX;

			var vertex = new Vertex(xPos, height, zPos);
			vertex.Color = color;
			FullDetailLevel.SetVertex(index, vertex);
			_invalidateNormals = true;
		}


		public override void Prepare(Viewport viewport)
		{
			if (LengthSegments <= 0 || HeightSegments <= 0)
			{
				throw new ArgumentException("LengthSegments and HeightSegments must be 1 or more");
			}

			PrepareVertices();
			PrepareFaces();

			// Todo: only do this when there are changes.
			GeometryHelper.CalculateNormals(FullDetailLevel, false);

			base.Prepare(viewport);
		}


		private void PrepareVertices()
		{
			var actualLength = LengthSegments + 1;
			var actualHeight = HeightSegments + 1;
			FullDetailLevel.AllocateVertices(actualLength * actualHeight);
			var yStart = (float)-(Dimension.Height / 2);
			var xStep = ((float)Dimension.Width) / (float)actualLength;
			var yStep = ((float)Dimension.Height) / (float)actualHeight;

			var uStep = ((float)1.0f) / (float)actualLength;
			var vStep = ((float)1.0f) / (float)actualHeight;

			var v = 0f;
			var vertexIndex = 0;
			for (var y = 0; y < actualHeight; y++)
			{
				var xStart = (float)-(Dimension.Width / 2);

				var u = 0f;

				for (var x = 0; x < actualLength; x++)
				{

					var vertex = new Vertex(xStart, 0f, yStart, 0f, 1f, 0f);
					FullDetailLevel.SetVertex(vertexIndex, vertex);
					xStart += xStep;
					u += uStep;
					vertexIndex++;
				}

				yStart += yStep;
				v += vStep;
			}
		}

		private void PrepareFaces()
		{
			var actualLength = LengthSegments + 1;
			var actualHeight = HeightSegments + 1;

			var faceCount = ((actualLength - 1) * 2) * (actualHeight - 1);
			FullDetailLevel.AllocateFaces(faceCount);
			var faceIndex = 0;

			for (var y = 0; y < actualHeight - 1; y++)
			{
				for (var x = 0; x < actualLength - 1; x++)
				{
					var offset = (y * actualLength) + x;
					var offsetNextLine = offset + actualLength;
					var face = new Face(offset, offset + 1, offsetNextLine);
					face.Normal = Vector.Up;
					face.Material = Material;
					FullDetailLevel.SetFace(faceIndex, face);
					face = new Face(offsetNextLine + 1, offsetNextLine, offset + 1);
					face.Normal = Vector.Up;
					face.Material = Material;
					FullDetailLevel.SetFace(faceIndex + 1, face);

					faceIndex += 2;
				}
			}
		}
	}
}
