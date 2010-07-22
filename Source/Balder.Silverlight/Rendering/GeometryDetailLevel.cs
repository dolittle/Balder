using System;
using System.Collections.Generic;
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
	public class GeometryDetailLevel : IGeometryDetailLevel
	{
		private static readonly FlatTriangle FlatTriangleRenderer = new FlatTriangle();
		private static readonly FlatTriangleAdditive FlatTriangleAdditiveRenderer = new FlatTriangleAdditive();
		private static readonly GouraudTriangle GouraudTriangleRenderer = new GouraudTriangle();
		private static readonly FlatTextureTriangle FlatTextureTriangleRenderer = new FlatTextureTriangle();
		private static readonly TextureTriangle TextureTriangleRenderer = new TextureTriangle();
		private static readonly GouraudTextureTriangle GouraudTextureTriangleRenderer = new GouraudTextureTriangle();
		private static readonly Point PointRenderer = new Point();
		private readonly ILightCalculator _lightCalculator;
		private readonly IMetaDataPixelBuffer _metaDataPixelBuffer;

		private RenderVertex[] _vertices;
		private RenderFace[] _faces;
		private Vertex[] _normals;
		private TextureCoordinate[] _textureCoordinates;
		private Line[] _lines;

		private bool _hasPrepared;

		public GeometryDetailLevel(ILightCalculator lightCalculator, IMetaDataPixelBuffer metaDataPixelBuffer)
		{
			_lightCalculator = lightCalculator;
			_metaDataPixelBuffer = metaDataPixelBuffer;
		}



		public int FaceCount { get { return null == _faces ? 0 : _faces.Length; } }
		public int VertexCount { get { return null == _vertices ? 0 : _vertices.Length; } }
		public int TextureCoordinateCount { get { return null == _textureCoordinates ? 0 : _textureCoordinates.Length; } }
		public int LineCount { get { return null == _lines ? 0 : _lines.Length; } }
		public int NormalCount { get { return null == _normals ? 0 : _normals.Length; } }

		#region Face related
		public void AllocateFaces(int count)
		{
			_faces = new RenderFace[count];
		}

		public void SetFace(int index, Face face)
		{
			var renderFace = new RenderFace(face) {Index = (UInt16)index};

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

		public Face GetFace(int index)
		{
			var face = _faces[index];
			return face;
		}

		public Vector GetFaceNormal(int index)
		{
			var face = _faces[index];
			return face.TransformedNormal;
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

		#endregion

		#region Normal related
		public void InvalidateVertex(int index)
		{

		}

		public void AllocateNormals(int count)
		{
			_normals = new Vertex[count];

		}

		public void SetNormal(int index, Vertex normal)
		{
			_normals[index] = normal;
		}

		public Vertex[] GetNormals()
		{
			return _normals;
		}

		public void InvalidateNormal(int index)
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

			if (null != _faces)
			{
				GenerateSmoothingInformation();
			}
		}

		private void GenerateSmoothingInformation()
		{
			var vertexCount = new Dictionary<int, Dictionary<int, int>>();
			var vertexNormal = new Dictionary<int, Dictionary<int, SmoothingGroupVertex>>();

			Action<int, Face> addNormal =
				delegate(int vertex, Face face)
				{
					Dictionary<int, SmoothingGroupVertex> smoothingGroupVertices;
					Dictionary<int, int> smoothingGroupCount;
					if (!vertexNormal.ContainsKey(vertex))
					{
						smoothingGroupVertices = new Dictionary<int, SmoothingGroupVertex>();
						vertexNormal[vertex] = smoothingGroupVertices;
						smoothingGroupCount = new Dictionary<int, int>();
						vertexCount[vertex] = smoothingGroupCount;
					}
					else
					{
						smoothingGroupVertices = vertexNormal[vertex];
						smoothingGroupCount = vertexCount[vertex];
					}

					if (!smoothingGroupCount.ContainsKey(face.SmoothingGroup))
					{
						smoothingGroupCount[face.SmoothingGroup] = 1;
					}

					SmoothingGroupVertex smoothingGroup;
					if (!smoothingGroupVertices.ContainsKey(face.SmoothingGroup))
					{
						smoothingGroup = new SmoothingGroupVertex();
						smoothingGroupVertices[face.SmoothingGroup] = smoothingGroup;
					}
					else
					{
						smoothingGroup = smoothingGroupVertices[face.SmoothingGroup];
					}

					smoothingGroup.Normal += face.Normal;
					smoothingGroup.Number = face.SmoothingGroup;
					smoothingGroupCount[face.SmoothingGroup]++;
				};

			foreach (var face in _faces)
			{
				addNormal(face.A, face);
				addNormal(face.B, face);
				addNormal(face.C, face);
			}

			foreach (var vertex in vertexNormal.Keys)
			{
				var countPerSmoothingGroup = vertexCount[vertex];

				var smoothingGroups = vertexNormal[vertex];


				foreach (var smoothingGroup in smoothingGroups.Values)
				{
					var count = countPerSmoothingGroup[smoothingGroup.Number];
					var normal = new Vector(smoothingGroup.Normal.X / count,
											smoothingGroup.Normal.Y / count,
											smoothingGroup.Normal.Z / count);
					normal.Normalize();
					smoothingGroup.Normal = normal;

					_vertices[vertex].SmoothingGroups[smoothingGroup.Number] = smoothingGroup;
				}

				InvalidateVertex(vertex);
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

				foreach( var smoothingGroup in vertex.SmoothingGroups.Values )
				{
					smoothingGroup.IsColorCalculated = false;
				}
			}
		}


		private Color CalculateColorForVertex(RenderVertex vertex, Viewport viewport, INode node, int smoothingGroup)
		{
			Color lightColor;
			if( null != vertex.SmoothingGroups && vertex.SmoothingGroups.ContainsKey(smoothingGroup))
			{
				var smoothingGroupVertex = vertex.SmoothingGroups[smoothingGroup];
				
				if (smoothingGroupVertex.IsColorCalculated)
				{
					lightColor = smoothingGroupVertex.CalculatedColor;
				}
				else
				{
					lightColor = _lightCalculator.Calculate(viewport, null, vertex.TransformedVector, smoothingGroupVertex.TransformedNormal);
					smoothingGroupVertex.CalculatedColor = lightColor;
				}
			} else
			{
				lightColor = _lightCalculator.Calculate(viewport, null, vertex.TransformedVector, vertex.TransformedNormal);
			}


			return lightColor; // vertex.Color.Additive(lightColor);
		}


		private void CalculateVertexColorsForFace(RenderFace face, Viewport viewport, INode node)
		{
			if (null == face.Material || face.Material.Shade == MaterialShade.Gouraud)
			{
				var vertexA = _vertices[face.A];
				var vertexB = _vertices[face.B];
				var vertexC = _vertices[face.C];
				face.CalculatedColorA = CalculateColorForVertex(vertexA, viewport, node, face.SmoothingGroup);
				face.CalculatedColorB = CalculateColorForVertex(vertexB, viewport, node, face.SmoothingGroup);
				face.CalculatedColorC = CalculateColorForVertex(vertexC, viewport, node, face.SmoothingGroup);
			}
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

		private bool IsFaceInView(Viewport viewport, Face face)
		{
			return (_vertices[face.A].TransformedVector.Z >= viewport.View.Near &&
					_vertices[face.B].TransformedVector.Z >= viewport.View.Near) &&
				   _vertices[face.C].TransformedVector.Z >= viewport.View.Near;
		}

		private bool IsLineInView(Viewport viewport, Line line)
		{
			return (_vertices[line.A].TransformedVector.Z >= viewport.View.Near &&
					_vertices[line.B].TransformedVector.Z >= viewport.View.Near);
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

				var nodeIdentifier = _metaDataPixelBuffer.GetIdentifier(node, face, face.Material);

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

				face.TransformNormal(matrix);
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
								face.Color = _lightCalculator.Calculate(viewport, null, face.TransformedPosition, face.TransformedNormal);
								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									FlatTextureTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
								}
								else
								{
									FlatTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
								}
							}
							break;

						case MaterialShade.Gouraud:
							{
								face.CalculatedColorA = face.CalculatedColorA;
								face.CalculatedColorB = face.CalculatedColorB;
								face.CalculatedColorC = face.CalculatedColorC;

								if (null != face.Material.DiffuseMap || null != face.Material.ReflectionMap)
								{
									GouraudTextureTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
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
					face.CalculatedColorA = face.CalculatedColorA.Additive(color);
					face.CalculatedColorB = face.CalculatedColorB.Additive(color);
					face.CalculatedColorC = face.CalculatedColorC.Additive(color);
					GouraudTriangleRenderer.Draw(face, _vertices, nodeIdentifier);
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

				if (!IsLineInView(viewport, line))
				{
					continue;
				}

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