#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
			renderFace.MaterialId = face.MaterialId;

			var aVector = _vertices[renderFace.A].ToVector();
			var bVector = _vertices[renderFace.B].ToVector();
			var cVector = _vertices[renderFace.C].ToVector();

			var v1 = cVector - aVector;
			var v2 = bVector - aVector;

			var cross = v1.Cross(v2);
			cross.Normalize();
			renderFace.Normal = cross;

			_faces[index] = renderFace;
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

        
		public void ResetColorCalculationIfNeeded()
		{
			if (null == _normals) return;
            var recalculateLight = _lightCalculator.HasLightsChanged;

			for (var normalIndex = 0; normalIndex < _normals.Length; normalIndex++)
			{
				var normal = _normals[normalIndex];
                if( recalculateLight )
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
            if (null == _vertices) return;

            var worldView = (world * view);
            var viewToLocal = Matrix.Invert(worldView);

            ResetColorCalculationIfNeeded();
            _lightCalculator.PrepareForNode(node, viewToLocal);

			var color = GetColorFromNode(node);

			BeginVerticesTiming(node);
			CalculateVertices(viewport, view, projection, world);
			EndVerticesTiming(node);

            Material material = null;
            if (node is IHaveMaterial && ((IHaveMaterial)node).Material != null)
                material = ((IHaveMaterial)node).Material;

			BeginRenderingTiming(node);
			SetRenderedFaces(node, RenderFaces(node, material, viewport, view, world, viewToLocal, depthTest));
			SetRenderedLines(node, RenderLines(viewport, color));
			EndRenderingTiming(node);

			if (viewport.DebugInfo.ShowVertices)
				RenderVertices(node, viewport);
		}


		static Color GetColorFromNode(INode node)
		{
			if (node is IHaveColor)
			{
				return ((IHaveColor)node).Color;
			}
			return Colors.Gray;
		}


		void TransformAndTranslateVertices(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			var worldView = (world*view);
			var worldViewProjection = worldView * projection;

			for (var vertexIndex = 0; vertexIndex < _vertices.Length; vertexIndex++)
			{
				var vertex = _vertices[vertexIndex];
                vertex.ProjectAndConvertToScreen(viewport, worldViewProjection);
			}
		}

		static readonly RenderNormal NullNormal = new RenderNormal(new Normal(0,0,0));

		RenderNormal CalculateColorForNormal(int vertexIndex, int normalIndex, Viewport viewport, Material material)
		{
			if (null == _normals || null == _vertices) return NullNormal;

			var normal = _normals[normalIndex];
			var vertex = _vertices[vertexIndex];

			if (!normal.IsColorCalculated)
			{
                normal.CalculatedColorAsInt = 
                    _lightCalculator.Calculate(
                        viewport, 
                        material, 
                        vertex.ToVector(), 
                        normal.ToVector(),
					    out normal.DiffuseColorAsInt, out normal.SpecularColorAsInt);

				normal.CalculatedColor = Color.FromInt(normal.CalculatedColorAsInt);
				normal.DiffuseColor = Color.FromInt(normal.DiffuseColorAsInt);
				normal.SpecularColor = Color.FromInt(normal.SpecularColorAsInt);
				normal.IsColorCalculated = true;
			}
			return normal;
		}

		void CalculateVertexColorsForFace(RenderFace face, Viewport viewport, Material material)
		{
			switch (material.CachedShade)
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
						face.DiffuseColorA = face.ColorA * normal.DiffuseColor;
						face.SpecularColorA = normal.SpecularColor;

                        normal = CalculateColorForNormal(face.B, face.NormalB, viewport, material);
						face.CalculatedColorB = face.ColorB * normal.CalculatedColor;
						face.CalculatedColorBAsInt = normal.CalculatedColorAsInt;
						face.DiffuseColorB = face.ColorB * normal.DiffuseColor;
						face.SpecularColorB = normal.SpecularColor;

                        normal = CalculateColorForNormal(face.C, face.NormalC, viewport, material);
						face.CalculatedColorC = face.ColorC * normal.CalculatedColor;
						face.CalculatedColorCAsInt = normal.CalculatedColorAsInt;
						face.DiffuseColorC = face.ColorC * normal.DiffuseColor;
						face.SpecularColorC = normal.SpecularColor;
					}
					break;
				case MaterialShade.Flat:
					{
						face.ColorAsInt = 
							_lightCalculator.Calculate(viewport, material, face.Center, face.Normal, out face.DiffuseAsInt, out face.SpecularAsInt);
						face.Color = Color.FromInt(face.ColorAsInt);
					}
					break;
			}

            face.AreColorsCalculated = true;
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


		private bool IsLineInView(Viewport viewport, Line line)
		{
			return (_vertices[line.A].ProjectedVector.Z >= Viewport.MinDepth &&
					_vertices[line.B].ProjectedVector.Z >= Viewport.MinDepth);
		}




		int RenderFaces(INode node, Material material, Viewport viewport, Matrix view, Matrix localToWorld, Matrix viewToLocal, bool depthTest)
		{
            if (null == _faces) return 0;

			var faceCount = 0;
            var viewInLocalPosition = new Vector(viewToLocal.M41, viewToLocal.M42, viewToLocal.M43);
            var viewInLocalForward = new Vector(viewToLocal.M31, viewToLocal.M32, viewToLocal.M33);
            viewInLocalForward.Normalize();

            var recalculateLights = _lightCalculator.HasLightsChanged;

			for (var faceIndex = 0; faceIndex < _faces.Length; faceIndex++)
			{
				var face = _faces[faceIndex];
                if (recalculateLights) face.AreColorsCalculated = false;

                if (!face.IsVisible(material, viewport, viewInLocalPosition, _vertices)) continue;

				if (null != _textureCoordinates && _textureCoordinates.Length > 0)
				{
					face.Texture1TextureCoordinateA = _textureCoordinates[face.DiffuseA];
					face.Texture1TextureCoordinateB = _textureCoordinates[face.DiffuseB];
					face.Texture1TextureCoordinateC = _textureCoordinates[face.DiffuseC];
				}
                
                var faceMaterial = GetMaterialForFace(face, node, material);
                face.UpdateMaterialInfo(faceMaterial);

				if (ShouldCalculateVertexColorsForFace(face))
                    CalculateVertexColorsForFace(face, viewport, faceMaterial);
                
				if( face.DrawSolid )
				{
                    face.Texture1 = faceMaterial.DiffuseTexture;
                    face.Texture2 = faceMaterial.ReflectionTexture;
                    face.Texture1Factor = faceMaterial.DiffuseTextureFactor;
                    face.Texture2Factor = faceMaterial.ReflectionTextureFactor;
                    face.Draw(viewport, _vertices, faceMaterial);
				}

				if (face.DrawWireframe)
				{
					if (face.WireframeHasConstantColor)
					{
                        face.CalculatedColorA = faceMaterial.CachedDiffuseWireframe;
                        face.CalculatedColorAAsInt = faceMaterial.CachedDiffuseWireframeAsInt;
                        face.CalculatedColorB = faceMaterial.CachedDiffuseWireframe;
                        face.CalculatedColorBAsInt = faceMaterial.CachedDiffuseWireframeAsInt;
                        face.CalculatedColorC = faceMaterial.CachedDiffuseWireframe;
                        face.CalculatedColorCAsInt = faceMaterial.CachedDiffuseWireframeAsInt;
					}
					DrawFaceAsLine(viewport, face, _vertices);
				}

				faceCount++;
			}
			return faceCount;
		}


		private bool ShouldCalculateVertexColorsForFace(RenderFace face)
		{
			return (face.DrawSolid || (face.DrawWireframe && !face.WireframeHasConstantColor)) && !face.AreColorsCalculated;
		
		}

		private Material GetActualMaterialFromFace(Material material, RenderFace face)
		{
			if (null != material && material.SubMaterials.Count > 0 )
			{
				if( material.SubMaterials.ContainsKey(face.MaterialId) )
				{
					material = material.SubMaterials[face.MaterialId];	
				}
			}
			return material;
		}

		private Material GetMaterialForFace(RenderFace face, INode node, Material material)
		{
			var actualMaterial = GetActualMaterialFromFace(material, face);

			if (null == actualMaterial)
			{
				if (node is IHaveColor)
				{
					actualMaterial = _colorMaterial;
					actualMaterial.Diffuse = ((IHaveColor)node).Color;
				}
				else
				{
					actualMaterial = Material.Default;
				}
			}
			return actualMaterial;
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

				if( line.IsColorSet )
				{
					colorAsInt = line.Color.ToInt();
				}

				var a = _vertices[line.A];
				var b = _vertices[line.B];
				Shapes.DrawLine(viewport,
								a.ProjectedVector.X,
								a.ProjectedVector.Y,
								a.ProjectedVector.Z,
								b.ProjectedVector.X,
								b.ProjectedVector.Y,
								b.ProjectedVector.Z,
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

		#region Statistics
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
		#endregion
	}
}
#endif