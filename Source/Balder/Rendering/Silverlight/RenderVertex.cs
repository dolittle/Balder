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

using System.Collections.Generic;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Rendering.Silverlight
{
	public class RenderVertex : Vertex
	{
		public RenderVertex()
			: this(0, 0, 0)
		{

		}

		public RenderVertex(Vertex vertex)
			: this(vertex.X, vertex.Y, vertex.Z, vertex.NormalX, vertex.NormalY, vertex.NormalZ)
		{

		}

		public RenderVertex(float x, float y, float z)
			: this(x, y, z, 0, 0, 0)
		{

		}

		public RenderVertex(float x, float y, float z, float normalX, float normalY, float normalZ)
			: base(x, y, z, normalX, normalY, normalZ)
		{
			TransformedVector = new Vector(x, y, z);
			TranslatedVector = new Vector(x, y, z);
			TranslatedScreenCoordinates = Vector.Zero;
			SmoothingGroups = new Dictionary<int, SmoothingGroupVertex>();
		}

		public void Transform(Matrix matrix)
		{
			TransformedVector = Vector.Transform(X, Y, Z, matrix);

			// Todo: calculating the rotated normal should only be done when necessary - performance boost!
			TransformedNormal = Vector.TransformNormal(NormalX, NormalY, NormalZ, matrix);

			foreach( var smoothingGroup in SmoothingGroups.Values )
			{
				smoothingGroup.TransformedNormal = Vector.TransformNormal(smoothingGroup.Normal, matrix);
			}
		}


		public void Translate(Matrix projectionMatrix, float width, float height)
		{
			TranslatedVector = Vector.Translate(TransformedVector, projectionMatrix, width, height);
		}

		public void MakeScreenCoordinates()
		{
			TranslatedScreenCoordinates.X = (int)TranslatedVector.X;
			TranslatedScreenCoordinates.Y = (int)TranslatedVector.Y;
			TranslatedScreenCoordinates.Z = (int)TranslatedVector.Z;
		}

		public Vector TransformedVector;
		public Vector TranslatedVector;
		public Vector TransformedNormal;
		public Vector TransformedVectorNormalized;
		public Vector TranslatedScreenCoordinates;
		public float DepthBufferAdjustedZ;

		public Dictionary<int, SmoothingGroupVertex> SmoothingGroups { get; private set; }

		public float U;
		public float V;
	}
}