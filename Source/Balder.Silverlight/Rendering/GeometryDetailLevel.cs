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

		private RenderVertex[] _vertices;
		private RenderFace[] _faces;
		private TextureCoordinate[] _textureCoordinates;
		private Line[] _lines;

		private bool _hasPrepared;

		public GeometryDetailLevel(ILightCalculator lightCalculator, INodesPixelBuffer nodesPixelBuffer)
		{
			_lightCalculator = lightCalculator;
			_nodesPixelBuffer = nodesPixelBuffer;
		}



		public int FaceCount { get { return null == _faces ? 0 : _faces.Length; } }
		public int VertexCount { get { return null == _vertices ? 0 : _vertices.Length; } }
		public int TextureCoordinateCount { get { return null == _textureCoordinates ? 0 : _textureCoordinates.Length; } }
		public int LineCount { get { return null == _lines ? 0 : _lines.Length; } }

		#region Face related
		public void AllocateFaces(int count)
		{
			_faces = new RenderFace[count];
		}

		public void SetFace(int index, Face face)
		{
			var renderFace = new RenderFace(face);
			var aVector = _vertices[renderFace.A].ToVector();
			var bVector = _vertices[renderFace.A].ToVector();
			var cVector = _vertices[renderFace.A].ToVector();

			var v1 = cVector - aVector;
			var v2 = bVector - aVector;

			var cross = v1.Cross(v2);
			cross.Normalize();
			renderFace.Normal = cross;

			var v = aVector + bVector + cVector;
			renderFace.Position = v / 3;

			_faces[index] = renderFace;
		}


		public void SetMaterial(int index, Material material)
		{
			_faces[index].Material = material;
		}

		public void SetMaterialForAllFaces(Material material)
		{
			if (null == _faces)
			{
				return;
			}

			for (var index = 0; index < _faces.Length; index++)
			{
				_faces[index].Material = material;
			}
		}

		public Face[] GetFaces()
		{
			return _faces;
		}

		public void InvalidateFace(int index)
		{

		}
		#endregion

		#region Vertex related
		public void AllocateVertices(int count)
		{
			_vertices = new RenderVertex[count];
		}

		public void SetVertex(int index, Vertex vertex)
		{
			_vertices[index] = new RenderVertex(vertex);
		}

		public Vertex[] GetVertices()
		{
			return _vertices;
		}

		public void InvalidateVertex(int index)
		{

		}
		#endregion

		#region Line related
		public void AllocateLines(int count)
		{
			_lines = new Line[count];
		}

		public void SetLine(int index, Line line)
		{
			_lines[index] = line;
		}

		public Line[] GetLines()
		{
			return _lines;
		}
		#endregion

		#region TextureCoordinate related

		public void AllocateTextureCoordinates(int count)
		{
			_textureCoordinates = new TextureCoordinate[count];
		}

		public void SetTextureCoordinate(int index, TextureCoordinate textureCoordinate)
		{
			_textureCoordinates[index] = textureCoordinate;
		}

		public void SetFaceTextureCoordinateIndex(int index, int a, int b, int c)
		{
			_faces[index].DiffuseA = a;
			_faces[index].DiffuseB = b;
			_faces[index].DiffuseC = c;
		}

		public TextureCoordinate[] GetTextureCoordinates()
		{
			return _textureCoordinates;
		}

		#endregion

		private void Prepare()
		{
			if (null != _textureCoordinates && _textureCoordinates.Length > 0 && null != _faces)
			{
				for (var index = 0; index < _faces.Length; index++)
				{
					_faces[index].DiffuseTextureCoordinateA = _textureCoordinates[_faces[index].DiffuseA];
					_faces[index].DiffuseTextureCoordinateB = _textureCoordinates[_faces[index].DiffuseB];
					_faces[index].DiffuseTextureCoordinateC = _textureCoordinates[_faces[index].DiffuseC];
				}
			}
		}

		public void CalculateVertices(Viewport viewport, INode node)
		{
			TransformAndTranslateVertices(viewport, node);
		}

		public void Render(Viewport viewport, INode node)
		{
			if (null == _vertices)
			{
				return;
			}
			if (!_hasPrepared)
			{
				Prepare();
				_hasPrepared = true;
			}

			CalculateVertices(viewport, node);
			RenderFaces(node, viewport);
			RenderLines(node, viewport);

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
			for (var vertexIndex = 0; vertexIndex < _vertices.Length; vertexIndex++)
			{
				var vertex = _vertices[vertexIndex];
				PointRenderer.Draw((int)vertex.TranslatedScreenCoordinates.X,
				                   (int)vertex.TranslatedScreenCoordinates.Y,
				                   viewport.DebugInfo.Color,
				                   4);
			}
		}

		private void CalculateVertexColorsForFace(Face face, Viewport viewport, INode node)
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


		private void RenderFaces(INode node, Viewport viewport)
		{
			if (null == _faces)
			{
				return;
			}

			var view = viewport.View.ViewMatrix;
			var projection = viewport.View.ProjectionMatrix;
			var world = node.RenderingWorld;

			var matrix = world * view;

			for (var faceIndex = 0; faceIndex < _faces.Length; faceIndex++)
			{
				var face = _faces[faceIndex];
				
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

				CalculateVertexColorsForFace(face, viewport, node);
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

		private void RenderLines(INode node, Viewport viewport)
		{
			if (null == _lines)
			{
				return;
			}
			for (var lineIndex = 0; lineIndex < _lines.Length; lineIndex++)
			{
				var line = _lines[lineIndex];
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