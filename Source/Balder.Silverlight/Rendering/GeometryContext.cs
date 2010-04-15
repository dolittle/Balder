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
using System.Windows.Media;
using Balder.Core;
using Balder.Core.Display;
using Balder.Core.Lighting;
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using Balder.Core.Rendering;
using Balder.Silverlight.Rendering.Drawing;
using Color = Balder.Core.Color;
using Matrix = Balder.Core.Math.Matrix;

namespace Balder.Silverlight.Rendering
{
	public class BoundingGeometry
	{
		public BoundingGeometry(Vector dimension)
		{
			Vertices = new Vertex[8];
			Vertices[0] = new Vertex(-dimension.X, dimension.Y, -dimension.Z);
			Vertices[1] = new Vertex(dimension.X, dimension.Y, -dimension.Z);
			Vertices[2] = new Vertex(-dimension.X, -dimension.Y, -dimension.Z);
			Vertices[3] = new Vertex(dimension.X, -dimension.Y, -dimension.Z);
			Vertices[4] = new Vertex(-dimension.X, dimension.Y, dimension.Z);
			Vertices[5] = new Vertex(dimension.X, dimension.Y, dimension.Z);
			Vertices[6] = new Vertex(-dimension.X, -dimension.Y, dimension.Z);
			Vertices[7] = new Vertex(dimension.X, -dimension.Y, dimension.Z);

			Lines = new Line[12];
			Lines[0] = new Line(0, 1);
			Lines[1] = new Line(2, 3);
			Lines[2] = new Line(4, 5);
			Lines[3] = new Line(6, 7);

			Lines[4] = new Line(0, 4);
			Lines[5] = new Line(1, 5);
			Lines[6] = new Line(2, 6);
			Lines[7] = new Line(3, 7);

			Lines[8] = new Line(0, 2);
			Lines[9] = new Line(1, 3);
			Lines[10] = new Line(4, 6);
			Lines[11] = new Line(5, 7);

			Faces = new Face[12];
			Faces[0] = new Face(2, 1, 0);
			Faces[1] = new Face(1, 2, 3);
				 
			
			Faces[2] = new Face(4, 5, 6);
			Faces[3] = new Face(7, 6, 5);
			
			Faces[4] = new Face(0, 4, 2);
			Faces[5] = new Face(6, 2, 4);
									   
			Faces[6] = new Face(3, 5, 1);
			Faces[7] = new Face(5, 3, 7);
				 
				 
			Faces[8] = new Face(0, 1, 4);
			Faces[9] = new Face(5, 4, 1);
									   
			Faces[10] = new Face(6, 3, 2);
			Faces[11] = new Face(3, 6, 7);
		}

		public Vertex[] Vertices;
		public Line[] Lines;
		public Face[] Faces;
	}

	public class GeometryContext : IGeometryContext
	{
		private static readonly FlatTriangle FlatTriangleRenderer = new FlatTriangle();
		private static readonly FlatTriangleAdditive FlatTriangleAdditiveRenderer = new FlatTriangleAdditive();
		private static readonly GouraudTriangle GouraudTriangleRenderer = new GouraudTriangle();
		private static readonly TextureTriangle TextureTriangleRenderer = new TextureTriangle();
		private static readonly Point PointRenderer = new Point();
		private readonly ILightCalculator _lightCalculator;

		private BoundingGeometry _boundingGeometry;
		
		
		


		private bool _hasPrepared;

		public GeometryContext(ILightCalculator lightCalculator)
		{
			_lightCalculator = lightCalculator;
		}


		private Vertex[] _vertices;
		public Vertex[] Vertices
		{
			get { return _vertices; }
			private set { _vertices = value; }
		}

		private Face[] _faces;
		public Face[] Faces
		{
			get { return _faces; }
			private set { _faces = value; }
		}

		public TextureCoordinate[] TextureCoordinates { get; private set; }
		private Line[] _lines;
		public Line[] Lines
		{
			get { return _lines; }
			private set { _lines = value; }
		}

		public int FaceCount { get { return null == Faces ? 0 : Faces.Length; } }
		public int VertexCount { get { return null == Vertices ? 0 : Vertices.Length; } }
		public int TextureCoordinateCount { get { return null == TextureCoordinates ? 0 : TextureCoordinates.Length; } }
		public int LineCount { get { return null == Lines ? 0 : Lines.Length; } }

		public void AllocateFaces(int count)
		{
			Faces = new Face[count];
		}

		public void SetFace(int index, Face face)
		{
			var v1 = Vertices[face.C].Vector - Vertices[face.A].Vector;
			var v2 = Vertices[face.B].Vector - Vertices[face.A].Vector;

			var cross = v1.Cross(v2);
			cross.Normalize();
			face.Normal = cross;

			var v = Vertices[face.A].Vector + Vertices[face.B].Vector + Vertices[face.C].Vector;
			face.Position = v / 3;

			Faces[index] = face;
		}

		public void SetFaceTextureCoordinateIndex(int index, int a, int b, int c)
		{
			Faces[index].DiffuseA = a;
			Faces[index].DiffuseB = b;
			Faces[index].DiffuseC = c;
		}

		public TextureCoordinate[] GetTextureCoordinates()
		{
			return TextureCoordinates;
		}

		public void SetMaterial(int index, Material material)
		{
			Faces[index].Material = material;
		}

		public void SetMaterialForAllFaces(Material material)
		{
			if (null == Faces)
			{
				return;
			}

			for (var index = 0; index < Faces.Length; index++)
			{
				Faces[index].Material = material;
			}
		}

		public Face[] GetFaces()
		{
			return Faces;
		}

		public void AllocateVertices(int count)
		{
			Vertices = new Vertex[count];
		}

		public void SetVertex(int index, Vertex vertex)
		{
			Vertices[index] = vertex;
		}

		public Vertex[] GetVertices()
		{
			return Vertices;
		}

		public void AllocateLines(int count)
		{
			Lines = new Line[count];
		}

		public void SetLine(int index, Line line)
		{
			Lines[index] = line;
		}

		public Line[] GetLines()
		{
			return Lines;
		}

		public void AllocateTextureCoordinates(int count)
		{
			TextureCoordinates = new TextureCoordinate[count];
		}

		public void SetTextureCoordinate(int index, TextureCoordinate textureCoordinate)
		{
			TextureCoordinates[index] = textureCoordinate;
		}

		private void Prepare()
		{
			if (null != TextureCoordinates && TextureCoordinates.Length > 0 && null != Faces)
			{
				for (var index = 0; index < Faces.Length; index++)
				{
					Faces[index].DiffuseTextureCoordinateA = TextureCoordinates[Faces[index].DiffuseA];
					Faces[index].DiffuseTextureCoordinateB = TextureCoordinates[Faces[index].DiffuseB];
					Faces[index].DiffuseTextureCoordinateC = TextureCoordinates[Faces[index].DiffuseC];
				}
			}

			var smallest = new Vector(0, 0, 0);
			var largest = new Vector(0, 0, 0);
			for (var index = 0; index < Vertices.Length; index++)
			{
				var vertex = Vertices[index];
				if( vertex.Vector.X < smallest.X )
				{
					smallest.X = vertex.Vector.X;
				}
				if (vertex.Vector.Y < smallest.Y)
				{
					smallest.Y = vertex.Vector.Y;
				}
				if (vertex.Vector.Z < smallest.Z)
				{
					smallest.Z = vertex.Vector.Z;
				}

				if (vertex.Vector.X > largest.X)
				{
					largest.X = vertex.Vector.X;
				}
				if (vertex.Vector.Y > largest.Y)
				{
					largest.Y = vertex.Vector.Y;
				}
				if (vertex.Vector.Z > largest.Z)
				{
					largest.Z = vertex.Vector.Z;
				}
			}

			var delta = largest - smallest;
			delta.X = Math.Abs(delta.X);
			delta.Y = Math.Abs(delta.Y);
			delta.Z = Math.Abs(delta.Z);
			var dimension = delta/2f;
			_boundingGeometry = new BoundingGeometry(dimension);
		}

		public void CalculateVertices(Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			TransformAndTranslateVertices(viewport, node, ref _vertices, view, projection, world);
		}

		public void Render(Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			if (null == Vertices)
			{
				return;
			}
			if (!_hasPrepared)
			{
				Prepare();
				_hasPrepared = true;
			}

			//RenderBoundingBox(viewport, node, view, projection, world);
			//return;

			CalculateVertices(viewport, node, view, projection, world);
			RenderFaces(node, viewport, ref _faces, ref _vertices, view, projection, world);
			RenderLines(node, viewport, ref _lines, _vertices, view, projection, world);

			if (viewport.DebugInfo.ShowVertices)
			{
				RenderVertices(node, viewport, view, projection, world);
			}
		}

		private void RenderBoundingBox(Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			TransformAndTranslateVertices(viewport, node, ref _boundingGeometry.Vertices, view, projection, world);
			RenderLines(node, viewport, ref _boundingGeometry.Lines, _boundingGeometry.Vertices, view, projection, world);
			//RenderFaces(node, viewport, ref _boundingGeometry.Faces, ref _boundingGeometry.Vertices, view, projection, world);
		}

		private static Color GetColorFromNode(INode node)
		{
			if (node is IHaveColor)
			{
				return ((IHaveColor)node).Color;
			}
			return Colors.Gray;
		}

		private static void TransformAndTranslateVertex(ref Vertex vertex, Viewport viewport, Matrix view, Matrix projection)
		{
			vertex.Transform(view);
			vertex.Translate(projection, viewport.Width, viewport.Height);
			vertex.MakeScreenCoordinates();

			vertex.TransformedVectorNormalized = vertex.TransformedNormal;
			vertex.TransformedVectorNormalized.Normalize();
			var z = ((vertex.TransformedVector.Z / viewport.View.DepthDivisor) + viewport.View.DepthZero);
			vertex.DepthBufferAdjustedZ = z;
		}

		private void TransformAndTranslateVertices(Viewport viewport, INode node, ref Vertex[] vertices, Matrix view, Matrix projection, Matrix world)
		{
			var localView = (world * view);
			for (var vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
			{
				var vertex = vertices[vertexIndex];
				TransformAndTranslateVertex(ref vertex, viewport, localView, projection);
				vertex.IsColorCalculated = false;
				vertices[vertexIndex] = vertex;
			}
		}


		private void CalculateColorForVertex(ref Vertex vertex, Viewport viewport, INode node)
		{
			var lightColor = _lightCalculator.Calculate(viewport, vertex.TransformedVector, vertex.TransformedNormal);
			vertex.CalculatedColor = vertex.Color.Additive(lightColor);
		}

		private void RenderVertices(INode node, Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			for (var vertexIndex = 0; vertexIndex < Vertices.Length; vertexIndex++)
			{
				PointRenderer.Draw((int)Vertices[vertexIndex].TranslatedScreenCoordinates.X,
								   (int)Vertices[vertexIndex].TranslatedScreenCoordinates.Y,
								   viewport.DebugInfo.Color,
								   4);
			}
		}

		private void CalculateVertexColorsForFace(ref Face face, ref Vertex[] vertices, Viewport viewport, INode node)
		{
			if (null == face.Material || face.Material.Shade == MaterialShade.Gouraud)
			{
				if (!vertices[face.A].IsColorCalculated)
				{
					CalculateColorForVertex(ref Vertices[face.A], viewport, node);
					vertices[face.A].IsColorCalculated = true;
				}
				if (!vertices[face.B].IsColorCalculated)
				{
					CalculateColorForVertex(ref Vertices[face.B], viewport, node);
					vertices[face.B].IsColorCalculated = true;
				}
				if (!Vertices[face.C].IsColorCalculated)
				{
					CalculateColorForVertex(ref Vertices[face.C], viewport, node);
					vertices[face.C].IsColorCalculated = true;
				}
			}
		}

		private bool IsFaceInView(Viewport viewport, Face face, ref Vertex[] vertices)
		{
			return (vertices[face.A].TransformedVector.Z >= viewport.View.Near && 
				vertices[face.B].TransformedVector.Z >= viewport.View.Near) && 
				vertices[face.C].TransformedVector.Z >= viewport.View.Near;
		}


		private void RenderFaces(INode node, Viewport viewport, ref Face[] faces, ref Vertex[] vertices, Matrix view, Matrix projection, Matrix world)
		{
			if (null == faces)
			{
				return;
			}

			var matrix = world * view;

			for (var faceIndex = 0; faceIndex < faces.Length; faceIndex++)
			{
				var face = faces[faceIndex];

				var nodeIdentifier =
					((Balder.Silverlight.Display.Display)viewport.Display).NodesPixelBuffer.GetNodeIdentifier(node, face.Material);

				var a = vertices[face.A];
				var b = vertices[face.B];
				var c = vertices[face.C];

				var mixedProduct = (b.TranslatedVector.X - a.TranslatedVector.X) * (c.TranslatedVector.Y - a.TranslatedVector.Y) -
								   (c.TranslatedVector.X - a.TranslatedVector.X) * (b.TranslatedVector.Y - a.TranslatedVector.Y);


				var visible = mixedProduct < 0 && IsFaceInView(viewport, face, ref vertices);
				//&& viewport.View.IsInView(a.TransformedVector);
				if (null != face.Material)
				{
					visible |= face.Material.DoubleSided;
				}
				if (!visible)
				{
					continue;
				}

				CalculateVertexColorsForFace(ref face, ref vertices, viewport, node);
				if (null != face.Material)
				{
					switch (face.Material.Shade)
					{
						case MaterialShade.None:
							{
								face.Color = face.Material.Diffuse;

								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									TextureTriangleRenderer.Draw(face, vertices, nodeIdentifier);
								}
								else
								{
									FlatTriangleRenderer.Draw(face, vertices, nodeIdentifier);
								}
							}
							break;

						case MaterialShade.Flat:
							{
								face.Transform(matrix);
								var color = face.Material.Diffuse;
								face.Color = color.Additive(_lightCalculator.Calculate(viewport, face.TransformedPosition, face.TransformedNormal));
								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									TextureTriangleRenderer.Draw(face, vertices, nodeIdentifier);
								}
								else
								{
									FlatTriangleRenderer.Draw(face, vertices, nodeIdentifier);
								}
							}
							break;

						case MaterialShade.Gouraud:
							{
								var color = face.Material.Diffuse;
								Vertices[face.A].CalculatedColor = color.Additive(vertices[face.A].CalculatedColor);
								Vertices[face.B].CalculatedColor = color.Additive(vertices[face.B].CalculatedColor);
								Vertices[face.C].CalculatedColor = color.Additive(vertices[face.C].CalculatedColor);

								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									TextureTriangleRenderer.Draw(face, vertices, nodeIdentifier);
								}
								else
								{
									GouraudTriangleRenderer.Draw(face, vertices, nodeIdentifier);
								}

							}
							break;
					}
				}
				else
				{
					var color = GetColorFromNode(node);
					var aColor = vertices[face.A].CalculatedColor;
					var bColor = vertices[face.B].CalculatedColor;
					var cColor = vertices[face.C].CalculatedColor;
					vertices[face.A].CalculatedColor = vertices[face.A].CalculatedColor.Additive(color);
					vertices[face.B].CalculatedColor = vertices[face.B].CalculatedColor.Additive(color);
					vertices[face.C].CalculatedColor = vertices[face.C].CalculatedColor.Additive(color);
					GouraudTriangleRenderer.Draw(face, vertices, nodeIdentifier);
					vertices[face.A].CalculatedColor = aColor;
					vertices[face.B].CalculatedColor = bColor;
					vertices[face.C].CalculatedColor = cColor;
				}
			}
		}

		private void RenderLines(INode node, Viewport viewport, ref Line[] lines, Vertex[] vertices, Matrix view, Matrix projection, Matrix world)
		{
			if (null == lines)
			{
				return;
			}
			for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
			{
				var line = lines[lineIndex];
				var a = vertices[line.A];
				var b = vertices[line.B];
				var xstart = a.TranslatedScreenCoordinates.X;
				var ystart = a.TranslatedScreenCoordinates.Y;
				var xend = b.TranslatedScreenCoordinates.X;
				var yend = b.TranslatedScreenCoordinates.Y;
				Shapes.DrawLine(viewport,
								(int)xstart,
								(int)ystart,
								(int)xend,
								(int)yend,
								GetColorFromNode(node));
			}
		}
	}
}