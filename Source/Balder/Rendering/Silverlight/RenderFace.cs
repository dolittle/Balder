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
using System.Collections.Generic;
using Balder.Display;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Rendering.Silverlight
{
	public class RenderFace : Face
	{
		private static readonly RenderVertex VertexA = new RenderVertex();
		private static readonly RenderVertex VertexB = new RenderVertex();
		private static readonly RenderVertex VertexC = new RenderVertex();
		private static readonly RenderVertex VertexD = new RenderVertex();

		public static readonly float DebugNormalLength = 5f;

		public Material Material;

		public UInt16 Index;

		public Vector TransformedNormal;
		public Vector Position;
		public Vector TransformedPosition;
		public Vector TranslatedPosition;
		public Vector TransformedDebugNormal;
		public Vector TranslatedDebugNormal;

		public TextureCoordinate Texture1TextureCoordinateA;
		public TextureCoordinate Texture1TextureCoordinateB;
		public TextureCoordinate Texture1TextureCoordinateC;

		public TextureCoordinate Texture2TextureCoordinateA;
		public TextureCoordinate Texture2TextureCoordinateB;
		public TextureCoordinate Texture2TextureCoordinateC;

		public TextureCoordinate LightMapTextureCoordinateA;
		public TextureCoordinate LightMapTextureCoordinateB;
		public TextureCoordinate LightMapTextureCoordinateC;

		public TextureCoordinate BumpMapTextureCoordinateA;
		public TextureCoordinate BumpMapTextureCoordinateB;
		public TextureCoordinate BumpMapTextureCoordinateC;

		public int ColorAAsInt;
		public int ColorBAsInt;
		public int ColorCAsInt;

		public Color CalculatedColorA;
		public Color CalculatedColorB;
		public Color CalculatedColorC;

		public int CalculatedColorAAsInt;
		public int CalculatedColorBAsInt;
		public int CalculatedColorCAsInt;

		public Color Color;
		public int ColorAsInt;

		public int MaterialAmbientAsInt;
		public int MaterialDiffuseAsInt;

		public int DiffuseAsInt;
		public int SpecularAsInt;

		public Color DiffuseColorA;
		public Color DiffuseColorB;
		public Color DiffuseColorC;

		public Color SpecularColorA;
		public Color SpecularColorB;
		public Color SpecularColorC;

		public Texture Texture1;
		public int Texture1Factor;

		public Texture Texture2;
		public int Texture2Factor;

		public Texture LightMap;

		public Texture BumpMap;

		public bool DrawSolid;
		public bool DrawWireframe;
		public bool WireframeHasConstantColor;

		public RenderFace(int a, int b, int c)
			: base(a, b, c)
		{

		}

		public RenderFace(Face face)
			: this(face.A, face.B, face.C)
		{
			//Material = face.Material;
			ColorA = face.ColorA;
			ColorB = face.ColorB;
			ColorC = face.ColorC;
			ColorAAsInt = ColorA.ToInt();
			ColorBAsInt = ColorB.ToInt();
			ColorCAsInt = ColorC.ToInt();
			DiffuseA = face.DiffuseA;
			DiffuseB = face.DiffuseB;
			DiffuseC = face.DiffuseC;
			SmoothingGroup = face.SmoothingGroup;
			Normal = face.Normal;

			CalculatedColorA = Colors.Black;
			CalculatedColorB = Colors.Black;
			CalculatedColorC = Colors.Black;
		}

		public bool IsVisible(Viewport viewport, RenderVertex[] vertices)
		{
			var dot = Vector.Dot(TransformedPosition, TransformedNormal);
			var visible = dot < 0;

			if (null != Material)
			{
				visible |= Material.CachedDoubleSided;
			}


			return visible && IsInView(viewport, vertices);
		}

		private bool IsInView(Viewport viewport, RenderVertex[] vertices)
		{
			var visible = true;

			var nearClipped = (vertices[A].ProjectedVector.Z < viewport.View.Near &&
			                   vertices[B].ProjectedVector.Z < viewport.View.Near &&
			                   vertices[C].ProjectedVector.Z < viewport.View.Near);
			if (nearClipped)
				return false;

		
			visible &= (vertices[A].ProjectedVector.X < viewport.Width || 
						vertices[B].ProjectedVector.X < viewport.Width ||
						vertices[C].ProjectedVector.X < viewport.Width);

			
			visible &= (vertices[A].ProjectedVector.X > 0 || 
						vertices[B].ProjectedVector.X > 0 || 
						vertices[C].ProjectedVector.X > 0);

			visible &= (vertices[A].ProjectedVector.Y < viewport.Height ||
						vertices[B].ProjectedVector.Y < viewport.Height ||
						vertices[C].ProjectedVector.Y < viewport.Height);

			visible &= (vertices[A].ProjectedVector.Y > 0 ||
						vertices[B].ProjectedVector.Y > 0 ||
						vertices[C].ProjectedVector.Y > 0);

			return visible;
		}



		public void TransformNormal(Matrix matrix)
		{
			TransformedNormal = Vector.TransformNormal(Normal, matrix);
		}

		public void Transform(Matrix matrix)
		{
			TransformedPosition = Vector.Transform(Position, matrix);
		}

		public void Transform(Matrix world, Matrix view)
		{
			TransformedNormal = Vector.TransformNormal(Normal, world);
			TransformedNormal = Vector.TransformNormal(TransformedNormal, view);
			TransformedPosition = Vector.Transform(Position, world, view);

			TransformedDebugNormal = TransformedPosition + (TransformedNormal); //*DebugNormalLength);
		}


		private void Prepare(RenderVertex vertexA, RenderVertex vertexB, RenderVertex vertexC)
		{
			if (null != Texture1TextureCoordinateA)
			{
				vertexA.U1 = Texture1TextureCoordinateA.U;
				vertexA.V1 = Texture1TextureCoordinateA.V;
			}
			if (null != Texture1TextureCoordinateB)
			{
				vertexB.U1 = Texture1TextureCoordinateB.U;
				vertexB.V1 = Texture1TextureCoordinateB.V;
			}
			if (null != Texture1TextureCoordinateC)
			{
				vertexC.U1 = Texture1TextureCoordinateC.U;
				vertexC.V1 = Texture1TextureCoordinateC.V;
			}

			if (null != Texture2TextureCoordinateA)
			{
				vertexA.U2 = Texture2TextureCoordinateA.U;
				vertexA.V2 = Texture2TextureCoordinateA.V;
			}
			if (null != Texture2TextureCoordinateB)
			{
				vertexB.U2 = Texture2TextureCoordinateB.U;
				vertexB.V2 = Texture2TextureCoordinateB.V;
			}
			if (null != Texture2TextureCoordinateC)
			{
				vertexC.U2 = Texture2TextureCoordinateC.U;
				vertexC.V2 = Texture2TextureCoordinateC.V;
			}

			vertexA.U2 = vertexA.U1;
			vertexA.V2 = vertexA.V1;

			vertexB.U2 = vertexB.U1;
			vertexB.V2 = vertexB.V1;

			vertexC.U2 = vertexC.U1;
			vertexC.V2 = vertexC.V1;

			vertexA.CalculatedColor = CalculatedColorA;
			vertexA.DiffuseColor = DiffuseColorA;
			vertexA.SpecularColor = SpecularColorA;


			vertexB.CalculatedColor = CalculatedColorB;
			vertexB.DiffuseColor = DiffuseColorB;
			vertexB.SpecularColor = SpecularColorB;

			vertexC.CalculatedColor = CalculatedColorC;
			vertexC.DiffuseColor = DiffuseColorC;
			vertexC.SpecularColor = SpecularColorC;
		}


		protected void GetSortedPoints(ref RenderVertex vertexA,
										ref RenderVertex vertexB,
										ref RenderVertex vertexC,
										Func<Vector, float> getComponent)
		{
			var point1 = vertexA;
			var point2 = vertexB;
			var point3 = vertexC;


			if (getComponent(point1.ProjectedVector) > getComponent(point2.ProjectedVector))
			{
				var p = point1;
				point1 = point2;
				point2 = p;
			}

			if (getComponent(point1.ProjectedVector) > getComponent(point3.ProjectedVector))
			{
				var p = point1;
				point1 = point3;
				point3 = p;
			}


			if (getComponent(point2.ProjectedVector) > getComponent(point3.ProjectedVector))
			{
				var p = point2;
				point2 = point3;
				point3 = p;
			}

			vertexA = point1;
			vertexB = point2;
			vertexC = point3;
		}

		private static readonly List<RenderVertex> _sortList = new List<RenderVertex>();


		protected void GetSortedPoints(ref RenderVertex vertexA,
										ref RenderVertex vertexB,
										ref RenderVertex vertexC,
										ref RenderVertex vertexD,
										Func<Vector, float> getComponent)
		{
			_sortList.Clear();
			_sortList.Add(vertexA);
			_sortList.Add(vertexB);
			_sortList.Add(vertexC);
			_sortList.Add(vertexD);
			_sortList.Sort((a, b) => getComponent(a.ProjectedVector).CompareTo(getComponent(b.ProjectedVector)));

			vertexA = _sortList[0];
			vertexB = _sortList[1];
			vertexC = _sortList[2];
			vertexD = _sortList[3];
		}

		protected bool IsClippedAgainstNear(Viewport viewport, RenderVertex a, RenderVertex b, RenderVertex c)
		{
			var clipped =
				a.ProjectedVector.Z < viewport.View.Near ||
				b.ProjectedVector.Z < viewport.View.Near ||
				c.ProjectedVector.Z < viewport.View.Near;

			return clipped;
		}


		public void Draw(Viewport viewport, RenderVertex[] vertices, Material material)
		{
			var vertexA = vertices[A];
			var vertexB = vertices[B];
			var vertexC = vertices[C];
			Prepare(vertexA, vertexB, vertexC);

			Draw(viewport, vertexA, vertexB, vertexC, material);
		}

		private Color ClipColor(Color colorA, Color colorB, float length, float distance)
		{
			var colorARed = ((float)colorA.Red) / 255f;
			var colorAGreen = ((float)colorA.Green) / 255f;
			var colorABlue = ((float)colorA.Blue) / 255f;
			var colorAAlpha = ((float)colorA.Alpha) / 255f;

			var colorBRed = ((float)colorB.Red) / 255f;
			var colorBGreen = ((float)colorB.Green) / 255f;
			var colorBBlue = ((float)colorB.Blue) / 255f;
			var colorBAlpha = ((float)colorB.Alpha) / 255f;

			var redDelta = colorBRed - colorARed;
			var greenDelta = colorBGreen - colorAGreen;
			var blueDelta = colorBBlue - colorABlue;
			var alphaDelta = colorBAlpha - colorAAlpha;

			colorARed += ((redDelta / length) * distance);
			colorAGreen += ((greenDelta / length) * distance);
			colorABlue += ((blueDelta / length) * distance);
			colorAAlpha += ((alphaDelta / length) * distance);

			var color = new Color(
				(byte) (colorARed*255f),
				(byte) (colorAGreen*255f),
				(byte) (colorABlue*255f),
				(byte) (colorAAlpha*255f)
				);
			return color;
		}

		private void ClipLine(Viewport viewport, RenderVertex vertexA, RenderVertex vertexB)
		{
			var distance = viewport.View.Near - vertexA.ProjectedVector.Z;
			var delta = vertexB.TransformedVector - vertexA.TransformedVector;
			var deltaU1 = vertexB.U1 - vertexA.U1;
			var deltaV1 = vertexB.V1 - vertexA.V1;
			var deltaU2 = vertexB.U2 - vertexA.U2;
			var deltaV2 = vertexB.V2 - vertexA.V2;
			var length = delta.Z;
				//System.Math.Max(System.Math.Max(delta.X, delta.Y), delta.Z);

			var xAdd = (delta.X / length) * distance;
			var yAdd = (delta.Y / length) * distance;
			var zAdd = (delta.Z / length) * distance;
			var u1Add = (deltaU1 / length) * distance;
			var v1Add = (deltaV1 / length) * distance;
			var u2Add = (deltaU2 / length) * distance;
			var v2Add = (deltaV2 / length) * distance;

			vertexA.TransformedVector = new Vector(
					vertexA.TransformedVector.X + xAdd,
					vertexA.TransformedVector.Y + yAdd,
					viewport.View.Near
					//vertexA.TransformedVector.Z + zAdd
				);
			vertexA.ProjectedVector = Vector.Transform(vertexA.TransformedVector, viewport.View.ProjectionMatrix);
			vertexA.ConvertToScreenCoordinates(viewport);

			vertexA.U1 += u1Add;
			vertexA.V1 += v1Add;
			vertexA.U2 += u2Add;
			vertexA.V2 += v2Add;

			vertexA.CalculatedColor = ClipColor(vertexA.CalculatedColor, vertexB.CalculatedColor, length, distance);
			vertexA.DiffuseColor = ClipColor(vertexA.DiffuseColor, vertexB.DiffuseColor, length, distance);
			vertexA.SpecularColor = ClipColor(vertexA.SpecularColor, vertexB.SpecularColor, length, distance);
		}


		private void Draw(Viewport viewport, RenderVertex vertexA, RenderVertex vertexB, RenderVertex vertexC, Material material)
		{
			vertexA.CopyTo(VertexA);
			vertexB.CopyTo(VertexB);
			vertexC.CopyTo(VertexC);

			vertexA = VertexA;
			vertexB = VertexB;
			vertexC = VertexC;

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC, (v) => v.Y);
			if (IsClippedAgainstNear(viewport, vertexA, vertexB, vertexC))
			{
				GetSortedPoints(ref vertexA, ref vertexB, ref vertexC, (v) => v.Z);

				if (vertexB.ProjectedVector.Z < viewport.View.Near)
				{
					ClipLine(viewport, vertexA, vertexC);
					ClipLine(viewport, vertexB, vertexC);

					GetSortedPoints(ref vertexA, ref vertexB, ref vertexC, (v) => v.Y);
					Draw(viewport, this, material, vertexA, vertexB, vertexC);
				}
				else
				{
					
					var vertexD = VertexD;
					vertexA.CopyTo(vertexD);

					ClipLine(viewport, vertexA, vertexB);
					ClipLine(viewport, vertexD, vertexC);
					
					var originalA = vertexA;
					var originalC = vertexC;

					GetSortedPoints(ref vertexA, ref vertexB, ref vertexC, (v) => v.Y);
					material.Renderer.Draw(viewport, this, vertexA, vertexB, vertexC);
					
					vertexA = originalA;
					vertexC = originalC;
					
					GetSortedPoints(ref vertexA, ref vertexD, ref vertexC, (v) => v.Y);
					Draw(viewport, this, material, vertexA, vertexD, vertexC);
				}
			}
			else
			{
				Draw(viewport, this, material, vertexA, vertexB, vertexC);
			}
		}

		private void Draw(Viewport viewport, RenderFace face, Material material, RenderVertex vertexA, RenderVertex vertexB, RenderVertex vertexC)
		{
			material.Renderer.Draw(viewport, face, vertexA, vertexB, vertexC);
		}


		public void CopyTo(RenderFace target)
		{
			target.ColorAsInt = ColorAsInt;
			target.DiffuseAsInt = DiffuseAsInt;
			target.SpecularAsInt = SpecularAsInt;
			target.MaterialDiffuseAsInt = MaterialDiffuseAsInt;
			target.Texture1 = Texture1;
			target.Texture1Factor = Texture1Factor;
			target.Texture2 = Texture2;
			target.Texture2Factor = Texture2Factor;
			target.LightMap = LightMap;
			target.BumpMap = BumpMap;
			target.Material = Material;
		}
	}
}
#endif