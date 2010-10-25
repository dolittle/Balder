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
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Rendering.Silverlight
{
	public class RenderFace : Face
	{
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

		public RenderFace(int a, int b, int c)
			: base(a,b,c)
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


		private void Prepare(Viewport viewport, RenderVertex[] vertices, Material material)
		{
			var vertexA = vertices[A];
			var vertexB = vertices[B];
			var vertexC = vertices[C];

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
										ref RenderVertex vertexC)
		{
			var point1 = vertexA;
			var point2 = vertexB;
			var point3 = vertexC;

			if (point1.ProjectedVector.Y > point2.ProjectedVector.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;
			}

			if (point1.ProjectedVector.Y > point3.ProjectedVector.Y)
			{
				var p = point1;
				point1 = point3;
				point3 = p;
			}


			if (point2.ProjectedVector.Y > point3.ProjectedVector.Y)
			{
				var p = point2;
				point2 = point3;
				point3 = p;
			}

			vertexA = point1;
			vertexB = point2;
			vertexC = point3;
		}


		public void Draw(Viewport viewport, RenderVertex[] vertices, Material material)
		{
			Prepare(viewport, vertices, material);

			var vertexA = vertices[A];
			var vertexB = vertices[B];
			var vertexC = vertices[C];

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			material.Renderer.Draw(viewport, this, vertexA, vertexB, vertexC);
		}
	}
}
#endif