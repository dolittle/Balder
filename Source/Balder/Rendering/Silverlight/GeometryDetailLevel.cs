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
#if(SILVERLIGHT)
using System;
using Balder.Display;
using Balder.Lighting;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;
using Balder.Rendering.Silverlight.Drawing;
using Matrix = Balder.Math.Matrix;

namespace Balder.Rendering.Silverlight
{
	public class GeometryDetailLevel : IGeometryDetailLevel
	{
		private static readonly Point PointRenderer = new Point();
		private readonly ILightCalculator _lightCalculator;

		private RenderVertex[] _vertices;
		private RenderFace[] _faces;
		private RenderNormal[] _normals;
		private TextureCoordinate[] _textureCoordinates;
		private Line[] _lines;

		private Material _colorMaterial;


		public GeometryDetailLevel(ILightCalculator lightCalculator)
		{
			_lightCalculator = lightCalculator;
			_colorMaterial = Material.FromColor(Colors.Blue);
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
			var renderFace = new RenderFace(face) { Index = (UInt16)index };

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

		public void InvalidateVertex(int index)
		{

		}

		#endregion

		#region Normal related

		public void AllocateNormals(int count)
		{
			_normals = new RenderNormal[count];

		}

		public void SetNormal(int index, Normal normal)
		{
			_normals[index] = new RenderNormal(normal);
		}

		public Normal[] GetNormals()
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


		public void CalculateNormals(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if (null == _normals)
			{
				return;
			}
			var localView = (world * view);
			for (var normalIndex = 0; normalIndex < _normals.Length; normalIndex++)
			{
				var normal = _normals[normalIndex];
				normal.Transformed = Vector.TransformNormal(normal.Vector, localView);
				normal.Transformed.Normalize();
				normal.IsColorCalculated = false;
			}
		}


		public void CalculateVertices(Viewport viewport, INode node)
		{
			CalculateVertices(viewport, viewport.View.ViewMatrix, viewport.View.ProjectionMatrix, node.RenderingWorld);
		}

		public void CalculateVertices(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			TransformAndTranslateVertices(viewport, view, projection, world);
		}

		public void Render(Viewport viewport, INode node)
		{
			Render(viewport, node, viewport.View.ViewMatrix, viewport.View.ProjectionMatrix, node.RenderingWorld);
		}

		public void Render(Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world)
		{
			Render(viewport, node, view, projection, world, true);
		}


		public void Render(Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world, bool depthTest)
		{
			if (null == _vertices)
			{
				return;
			}


			var color = GetColorFromNode(node);

			BeginVerticesTiming(node);

			CalculateVertices(viewport, view, projection, world);
			EndVerticesTiming(node);

			BeginLightingTiming(node);
			CalculateNormals(viewport, view, projection, world);
			EndLightingTiming(node);

			BeginRenderingTiming(node);
			SetRenderedFaces(node, RenderFaces(node, viewport, view, world, depthTest));
			SetRenderedLines(node, RenderLines(viewport, color));
			EndRenderingTiming(node);

			if (viewport.DebugInfo.ShowVertices)
			{
				RenderVertices(node, viewport);
			}
		}

		private void SetRenderedFaces(INode node, int renderedFaces)
		{
			var geometryStatistics = node.Statistics as GeometryStatistics;
			if (null != geometryStatistics)
			{
				geometryStatistics.RenderedFaces = renderedFaces;
			}
		}

		private void SetRenderedLines(INode node, int renderedLines)
		{
			var geometryStatistics = node.Statistics as GeometryStatistics;
			if (null != geometryStatistics)
			{
				geometryStatistics.RenderedLines = renderedLines;
			}
		}

		private void BeginVerticesTiming(INode node)
		{
			var geometryStatistics = node.Statistics as GeometryStatistics;
			if (null != geometryStatistics)
			{
				geometryStatistics.BeginVerticesTiming();
			}
		}

		private void EndVerticesTiming(INode node)
		{
			var geometryStatistics = node.Statistics as GeometryStatistics;
			if (null != geometryStatistics)
			{
				geometryStatistics.EndVerticesTiming();
			}
		}

		private void BeginLightingTiming(INode node)
		{
			var geometryStatistics = node.Statistics as GeometryStatistics;
			if (null != geometryStatistics)
			{
				geometryStatistics.BeginLightingTiming();
			}
		}

		private void EndLightingTiming(INode node)
		{
			var geometryStatistics = node.Statistics as GeometryStatistics;
			if (null != geometryStatistics)
			{
				geometryStatistics.EndLightingTiming();
			}
		}

		private void BeginRenderingTiming(INode node)
		{
			var geometryStatistics = node.Statistics as GeometryStatistics;
			if (null != geometryStatistics)
			{
				geometryStatistics.BeginRenderingTiming();
			}
		}

		private void EndRenderingTiming(INode node)
		{
			var geometryStatistics = node.Statistics as GeometryStatistics;
			if (null != geometryStatistics)
			{
				geometryStatistics.EndRenderingTiming();
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


		private void TransformAndTranslateVertices(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			var worldView = (world * view);
			var worldViewProjection = worldView * projection;
			for (var vertexIndex = 0; vertexIndex < _vertices.Length; vertexIndex++)
			{
				var vertex = _vertices[vertexIndex];
				vertex.TransformAndProject(viewport, worldView, worldViewProjection);
			}
		}

		private static readonly RenderNormal NullNormal = new RenderNormal(new Normal(0,0,0));

		private RenderNormal CalculateColorForNormal(int vertexIndex, int normalIndex, Viewport viewport, Material material)
		{
			
			if (null == _normals || null == _vertices)
			{
				return NullNormal;
			}

			var normal = _normals[normalIndex];
			var vertex = _vertices[vertexIndex];

			if (!normal.IsColorCalculated)
			{

				// Todo : use inverted matrix for lighting - calculate lights according to the vertices original coordinates
				normal.CalculatedColorAsInt =
					_lightCalculator.Calculate(viewport, material, vertex.TransformedVector, normal.Transformed,
					                           out normal.DiffuseColorAsInt, out normal.SpecularColorAsInt);
						
				normal.CalculatedColor = Color.FromInt(normal.CalculatedColorAsInt);
				normal.DiffuseColor = Color.FromInt(normal.DiffuseColorAsInt);
				normal.SpecularColor = Color.FromInt(normal.SpecularColorAsInt);
				normal.IsColorCalculated = true;
			}
			return normal;
		}

		private void CalculateVertexColorsForFace(RenderFace face, Viewport viewport, Material material)
		{
			switch (material.Shade)
			{
				case MaterialShade.None:
					{
						face.CalculatedColorA = face.ColorA;
						face.CalculatedColorB = face.ColorB;
						face.CalculatedColorC = face.ColorC;
						face.CalculatedColorAAsInt = face.ColorAAsInt;
						face.CalculatedColorBAsInt = face.ColorBAsInt;
						face.CalculatedColorCAsInt = face.ColorCAsInt;
					}
					break;
				case MaterialShade.Gouraud:
					{
						var normal = CalculateColorForNormal(face.A, face.NormalA, viewport, material);
						face.CalculatedColorA = face.ColorA * normal.CalculatedColor;
						face.CalculatedColorAAsInt = normal.CalculatedColorAsInt;
						face.DiffuseColorA = normal.DiffuseColor;
						face.SpecularColorA = normal.SpecularColor;

						normal = CalculateColorForNormal(face.B, face.NormalB, viewport, material);
						face.CalculatedColorB = face.ColorB * normal.CalculatedColor;
						face.CalculatedColorBAsInt = normal.CalculatedColorAsInt;
						face.DiffuseColorB = normal.DiffuseColor;
						face.SpecularColorB = normal.SpecularColor;

						normal = CalculateColorForNormal(face.C, face.NormalC, viewport, material);
						face.CalculatedColorC = face.ColorC * normal.CalculatedColor;
						face.CalculatedColorCAsInt = normal.CalculatedColorAsInt;
						face.DiffuseColorC = normal.DiffuseColor;
						face.SpecularColorC = normal.SpecularColor;
					}
					break;
				case MaterialShade.Flat:
					{
						face.ColorAsInt = 
							_lightCalculator.Calculate(viewport, material, face.TransformedPosition, face.TransformedNormal, out face.DiffuseAsInt, out face.SpecularAsInt);
						face.Color = Color.FromInt(face.ColorAsInt);
					}
					break;
			}
		}


		private void RenderVertices(INode node, Viewport viewport)
		{
			for (var vertexIndex = 0; vertexIndex < _vertices.Length; vertexIndex++)
			{
				var vertex = _vertices[vertexIndex];
				PointRenderer.Draw((int)vertex.ProjectedVector.X,
								   (int)vertex.ProjectedVector.Y,
								   viewport.DebugInfo.Color,
								   4);
			}
		}

		private bool IsFaceInView(Viewport viewport, Face face)
		{
			var visible = true;

			visible &= (_vertices[face.A].ProjectedVector.X < viewport.Width || 
						_vertices[face.B].ProjectedVector.X < viewport.Width ||
						_vertices[face.C].ProjectedVector.X < viewport.Width);

			visible &= (_vertices[face.A].ProjectedVector.X > 0 || 
						_vertices[face.B].ProjectedVector.X > 0 || 
						_vertices[face.C].ProjectedVector.X > 0);

			visible &= (_vertices[face.A].ProjectedVector.Y < viewport.Height ||
						_vertices[face.B].ProjectedVector.Y < viewport.Height ||
						_vertices[face.C].ProjectedVector.Y < viewport.Height);

			visible &= (_vertices[face.A].ProjectedVector.Y > 0 ||
						_vertices[face.B].ProjectedVector.Y > 0 ||
						_vertices[face.C].ProjectedVector.Y > 0);

			visible &= (_vertices[face.A].ProjectedVector.Z >= Viewport.MinDepth &&
					 _vertices[face.B].ProjectedVector.Z >= Viewport.MinDepth) &&
					_vertices[face.C].ProjectedVector.Z >= Viewport.MinDepth;
			return visible;
		}

		private bool IsLineInView(Viewport viewport, Line line)
		{
			return (_vertices[line.A].ProjectedVector.Z >= Viewport.MinDepth &&
					_vertices[line.B].ProjectedVector.Z >= Viewport.MinDepth);
		}



		private int RenderFaces(INode node, Viewport viewport, Matrix view, Matrix world, bool depthTest)
		{
			if (null == _faces)
			{
				return 0;
			}

			var faceCount = 0;
			var localView = (world * view);
			for (var faceIndex = 0; faceIndex < _faces.Length; faceIndex++)
			{
				var face = _faces[faceIndex];

				bool visible = IsFaceVisible(face, viewport);
				if (!visible)
				{
					continue;
				}

				face.TransformNormal(localView);
				face.Transform(localView);

				if (null != _textureCoordinates)
				{
					face.Texture1TextureCoordinateA = _textureCoordinates[face.DiffuseA];
					face.Texture1TextureCoordinateB = _textureCoordinates[face.DiffuseB];
					face.Texture1TextureCoordinateC = _textureCoordinates[face.DiffuseC];
				}

				var material = PrepareMaterialForFace(face, node);
				face.Opacity = material.CachedOpacityAsInt;
				face.MaterialDiffuseAsInt = material.DiffuseAsInt;

				if( ShouldCalculateVertexColorsForFace(material))
				{
					CalculateVertexColorsForFace(face, viewport, material);
				}

				if( material.CachedSolid )
				{
					face.Texture1 = material.DiffuseTexture;
					face.Texture2 = material.ReflectionTexture;
					face.Texture1Factor = material.DiffuseTextureFactor;
					face.Texture2Factor = material.ReflectionTextureFactor;

					material.Renderer.Draw(face, _vertices);
				}
				if( material.CachedWireframe )
				{
					if( material.CachedConstantColorForWireframe )
					{
						face.CalculatedColorA = material.CachedDiffuseWireframe;
						face.CalculatedColorAAsInt = material.CachedDiffuseWireframeAsInt;
						face.CalculatedColorB = material.CachedDiffuseWireframe;
						face.CalculatedColorBAsInt = material.CachedDiffuseWireframeAsInt;
						face.CalculatedColorC = material.CachedDiffuseWireframe;
						face.CalculatedColorCAsInt = material.CachedDiffuseWireframeAsInt;
					}
					DrawFaceAsLine(viewport, face, _vertices);
				}

				faceCount++;
			}
			return faceCount;
		}

		private bool ShouldCalculateVertexColorsForFace(Material material)
		{
			return material.CachedSolid || (material.CachedWireframe && !material.CachedConstantColorForWireframe);
		
		}

		private Material PrepareMaterialForFace(RenderFace face, INode node)
		{
			Material material = face.Material;
			if (null == material)
			{
				if (node is IHaveColor)
				{
					material = _colorMaterial;
					material.Diffuse = ((IHaveColor)node).Color;
				}
				else
				{
					material = Material.Default;
				}
			}
			return material;
		}

		private bool IsFaceVisible(RenderFace face, Viewport viewport)
		{
			var a = _vertices[face.A];
			var b = _vertices[face.B];
			var c = _vertices[face.C];

			var mixedproduct = (b.ProjectedVector.X - a.ProjectedVector.X) * (c.ProjectedVector.Y - a.ProjectedVector.Y) -
							   (c.ProjectedVector.X - a.ProjectedVector.X) * (b.ProjectedVector.Y - a.ProjectedVector.Y);
			var visible = mixedproduct < 0 && IsFaceInView(viewport, face);
			//&& viewport.View.IsInView(a.TransformedVector);
			if (null != face.Material)
			{
				visible |= face.Material.CachedDoubleSided;
			}

			return visible;
		}

		private int RenderLines(Viewport viewport, Color color)
		{
			if (null == _lines)
			{
				return 0;
			}

			var lineCount = 0;
			var colorAsInt = color.ToInt();
			for (var lineIndex = 0; lineIndex < _lines.Length; lineIndex++)
			{
				var line = _lines[lineIndex];

				if (!IsLineInView(viewport, line))
				{
					continue;
				}

				var a = _vertices[line.A];
				var b = _vertices[line.B];
				var xstart = a.ProjectedVector.X;
				var ystart = a.ProjectedVector.Y;
				var xend = b.ProjectedVector.X;
				var yend = b.ProjectedVector.Y;
				Shapes.DrawLine(viewport,
								(int)xstart,
								(int)ystart,
								(int)xend,
								(int)yend,
								colorAsInt);
				lineCount++;
			}
			return lineCount;
		}


		private void DrawFaceAsLine(Viewport viewport, RenderFace face, RenderVertex[] vertices)
		{
			var a = _vertices[face.A];
			var b = _vertices[face.B];
			var c = _vertices[face.C];
#if(false)
			Shapes.DrawLine(viewport,
							a.ProjectedVector.X,
							a.ProjectedVector.Y,
							a.ProjectedVector.Z,
							b.ProjectedVector.X,
							b.ProjectedVector.Y,
							b.ProjectedVector.Z,
							face.CalculatedColorAAsInt);

			Shapes.DrawLine(viewport,
							b.ProjectedVector.X,
							b.ProjectedVector.Y,
							b.ProjectedVector.Z,
							c.ProjectedVector.X,
							c.ProjectedVector.Y,
							c.ProjectedVector.Z,
							face.CalculatedColorBAsInt);

			Shapes.DrawLine(viewport,
							a.ProjectedVector.X,
							a.ProjectedVector.Y,
							a.ProjectedVector.Z,
							c.ProjectedVector.X,
							c.ProjectedVector.Y,
							c.ProjectedVector.Z,
							face.CalculatedColorCAsInt);
#else
			Shapes.DrawLine(viewport,
							a.ProjectedVector.X,
							a.ProjectedVector.Y,
							b.ProjectedVector.X,
							b.ProjectedVector.Y,
							face.CalculatedColorAAsInt);

			Shapes.DrawLine(viewport,
							b.ProjectedVector.X,
							b.ProjectedVector.Y,
							c.ProjectedVector.X,
							c.ProjectedVector.Y,
							face.CalculatedColorBAsInt);

			Shapes.DrawLine(viewport,
							a.ProjectedVector.X,
							a.ProjectedVector.Y,
							c.ProjectedVector.X,
							c.ProjectedVector.Y,
							face.CalculatedColorCAsInt);
#endif
		}
	}
}
#endif