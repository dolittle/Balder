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
using Balder;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Silverlight.Rendering
{
	public class RenderFace : Face
	{
		public static readonly float DebugNormalLength = 5f;

		public UInt16 Index;

		public Vector TransformedNormal;
		public Vector Position;
		public Vector TransformedPosition;
		public Vector TranslatedPosition;
		public Vector TransformedDebugNormal;
		public Vector TranslatedDebugNormal;
		public TextureCoordinate DiffuseTextureCoordinateA;
		public TextureCoordinate DiffuseTextureCoordinateB;
		public TextureCoordinate DiffuseTextureCoordinateC;

		public Color CalculatedColorA;
		public Color CalculatedColorB;
		public Color CalculatedColorC;


		public RenderFace(int a, int b, int c)
			: base(a,b,c)
		{
			
		}

		public RenderFace(Face face)
			: this(face.A, face.B, face.C)
		{
			Material = face.Material;
			Color = face.Color;
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

		public void Translate(Matrix projectionMatrix, float width, float height)
		{
			TranslatedPosition = Vector.Translate(TransformedPosition, projectionMatrix, width, height);
			TranslatedDebugNormal = Vector.Translate(TransformedDebugNormal, projectionMatrix, width, height);
		}

	}
}