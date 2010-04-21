using System.Windows.Media;
using Balder.Core;
using Balder.Core.Display;
using Balder.Core.Lighting;
using Balder.Core.Materials;
using Balder.Core.Objects.Geometries;
using Balder.Core.Rendering;
using Balder.Silverlight.Rendering.Drawing;
using Color=Balder.Core.Color;
using Matrix=Balder.Core.Math.Matrix;

namespace Balder.Silverlight.Rendering
{
	public class GeometryDetailLevel : IGeometryDetailLevel
	{
		private static readonly FlatTriangle FlatTriangleRenderer = new FlatTriangle();
		private static readonly FlatTriangleAdditive FlatTriangleAdditiveRenderer = new FlatTriangleAdditive();
		private static readonly GouraudTriangle GouraudTriangleRenderer = new GouraudTriangle();
		private static readonly TextureTriangle TextureTriangleRenderer = new TextureTriangle();
		private static readonly Point PointRenderer = new Point();
		private readonly ILightCalculator _lightCalculator;
		private readonly INodesPixelBuffer _nodesPixelBuffer;


		private bool _hasPrepared;

		public GeometryDetailLevel(ILightCalculator lightCalculator, INodesPixelBuffer nodesPixelBuffer)
		{
			_lightCalculator = lightCalculator;
			_nodesPixelBuffer = nodesPixelBuffer;
		}


		private RenderVertex[] _vertices;
		public Vertex[] Vertices
		{
			get { return _vertices; }
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
			var aVector = _vertices[face.A].ToVector();
			var bVector = _vertices[face.A].ToVector();
			var cVector = _vertices[face.A].ToVector();

			var v1 = cVector - aVector;
			var v2 = bVector - aVector;

			var cross = v1.Cross(v2);
			cross.Normalize();
			face.Normal = cross;

			var v = aVector + bVector + cVector;
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
			_vertices = new RenderVertex[count];
		}

		public void SetVertex(int index, Vertex vertex)
		{
			Vertices[index] = new RenderVertex(vertex);
		}

		public Vertex[] GetVertices()
		{
			return Vertices;
		}

		public void InvalidateVertex(int index)
		{
			
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
		}

		public void CalculateVertices(Viewport viewport, INode node)
		{
			TransformAndTranslateVertices(viewport, node);
		}

		public void Render(Viewport viewport, INode node)
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

			CalculateVertices(viewport, node);
			RenderFaces(node, viewport, ref _faces);
			RenderLines(node, viewport, ref _lines);

			if (viewport.DebugInfo.ShowVertices)
			{
				RenderVertices(node, viewport);
			}
		}


		private static Color GetColorFromNode(INode node)
		{
			if (node is IHaveColor)
			{
				return ((IHaveColor)node).Color;
			}
			return Colors.Gray;
		}

		private static void TransformAndTranslateVertex(RenderVertex vertex, Viewport viewport, Matrix localView, Matrix projection)
		{

			vertex.Transform(localView);
			vertex.Translate(projection, viewport.Width, viewport.Height);
			vertex.MakeScreenCoordinates();

			vertex.TransformedVectorNormalized = vertex.TransformedNormal;
			vertex.TransformedVectorNormalized.Normalize();
			var z = ((vertex.TransformedVector.Z / viewport.View.DepthDivisor) + viewport.View.DepthZero);
			vertex.DepthBufferAdjustedZ = z;
		}

		private void TransformAndTranslateVertices(Viewport viewport, INode node)
		{
			var view = viewport.View.ViewMatrix;
			var projection = viewport.View.ProjectionMatrix;
			var world = node.RenderingWorld;

			var localView = (world * view);
			for (var vertexIndex = 0; vertexIndex < _vertices.Length; vertexIndex++)
			{
				var vertex = _vertices[vertexIndex];
				TransformAndTranslateVertex(vertex, viewport, localView, projection);
				vertex.IsColorCalculated = false;
			}
		}


		private void CalculateColorForVertex(RenderVertex vertex, Viewport viewport, INode node)
		{
			var lightColor = _lightCalculator.Calculate(viewport, vertex.TransformedVector, vertex.TransformedNormal);
			vertex.CalculatedColor = vertex.Color.Additive(lightColor);
		}

		private void RenderVertices(INode node, Viewport viewport)
		{
			for (var vertexIndex = 0; vertexIndex < Vertices.Length; vertexIndex++)
			{
				var vertex = _vertices[vertexIndex];
				PointRenderer.Draw((int)vertex.TranslatedScreenCoordinates.X,
				                   (int)vertex.TranslatedScreenCoordinates.Y,
				                   viewport.DebugInfo.Color,
				                   4);
			}
		}

		private void CalculateVertexColorsForFace(ref Face face, Viewport viewport, INode node)
		{
			if (null == face.Material || face.Material.Shade == MaterialShade.Gouraud)
			{
				var vertexA = _vertices[face.A];
				var vertexB = _vertices[face.B];
				var vertexC = _vertices[face.C];
				if (!vertexA.IsColorCalculated)
				{
					CalculateColorForVertex(vertexA, viewport, node);
					vertexA.IsColorCalculated = true;
				}
				if (!vertexB.IsColorCalculated)
				{
					CalculateColorForVertex(vertexB, viewport, node);
					vertexB.IsColorCalculated = true;
				}
				if (!vertexC.IsColorCalculated)
				{
					CalculateColorForVertex(vertexC, viewport, node);
					vertexC.IsColorCalculated = true;
				}
			}
		}

		private bool IsFaceInView(Viewport viewport, Face face)
		{
			return (_vertices[face.A].TransformedVector.Z >= viewport.View.Near &&
			        _vertices[face.B].TransformedVector.Z >= viewport.View.Near) &&
			       _vertices[face.C].TransformedVector.Z >= viewport.View.Near;
		}


		private void RenderFaces(INode node, Viewport viewport, ref Face[] faces)
		{
			if (null == faces)
			{
				return;
			}

			var view = viewport.View.ViewMatrix;
			var projection = viewport.View.ProjectionMatrix;
			var world = node.RenderingWorld;

			var matrix = world * view;

			for (var faceIndex = 0; faceIndex < faces.Length; faceIndex++)
			{
				var face = faces[faceIndex];
				
				var nodeIdentifier = _nodesPixelBuffer.GetNodeIdentifier(node, face.Material);

				var a = _vertices[face.A];
				var b = _vertices[face.B];
				var c = _vertices[face.C];

				var mixedProduct = (b.TranslatedVector.X - a.TranslatedVector.X) * (c.TranslatedVector.Y - a.TranslatedVector.Y) -
				                   (c.TranslatedVector.X - a.TranslatedVector.X) * (b.TranslatedVector.Y - a.TranslatedVector.Y);


				var visible = mixedProduct < 0 && IsFaceInView(viewport, face);
				//&& viewport.View.IsInView(a.TransformedVector);
				if (null != face.Material)
				{
					visible |= face.Material.DoubleSided;
				}
				if (!visible)
				{
					continue;
				}

				CalculateVertexColorsForFace(ref face, viewport, node);
				if (null != face.Material)
				{
					switch (face.Material.Shade)
					{
						case MaterialShade.None:
							{
								face.Color = face.Material.Diffuse;

								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									TextureTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
								}
								else
								{
									FlatTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
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
									TextureTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
								}
								else
								{
									FlatTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
								}
							}
							break;

						case MaterialShade.Gouraud:
							{
								var color = face.Material.Diffuse;
								_vertices[face.A].CalculatedColor = color.Additive(_vertices[face.A].CalculatedColor);
								_vertices[face.B].CalculatedColor = color.Additive(_vertices[face.B].CalculatedColor);
								_vertices[face.C].CalculatedColor = color.Additive(_vertices[face.C].CalculatedColor);

								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									TextureTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
								}
								else
								{
									GouraudTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
								}

							}
							break;
					}
				}
				else
				{
					var color = GetColorFromNode(node);
					var aColor = _vertices[face.A].CalculatedColor;
					var bColor = _vertices[face.B].CalculatedColor;
					var cColor = _vertices[face.C].CalculatedColor;
					_vertices[face.A].CalculatedColor = _vertices[face.A].CalculatedColor.Additive(color);
					_vertices[face.B].CalculatedColor = _vertices[face.B].CalculatedColor.Additive(color);
					_vertices[face.C].CalculatedColor = _vertices[face.C].CalculatedColor.Additive(color);
					GouraudTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
					_vertices[face.A].CalculatedColor = aColor;
					_vertices[face.B].CalculatedColor = bColor;
					_vertices[face.C].CalculatedColor = cColor;
				}
			}
		}

		private void RenderLines(INode node, Viewport viewport, ref Line[] lines)
		{
			if (null == lines)
			{
				return;
			}
			for (var lineIndex = 0; lineIndex < lines.Length; lineIndex++)
			{
				var line = lines[lineIndex];
				var a = _vertices[line.A];
				var b = _vertices[line.B];
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