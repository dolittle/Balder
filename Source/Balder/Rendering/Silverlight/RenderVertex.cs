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
using Balder.Display;
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
			ProjectedVector = new Vector(x, y, z);
		}

		public void TransformAndProject(Viewport viewport, Matrix worldView, Matrix worldViewProjection)
		{
			ProjectedVector = Vector.Transform(X, Y, Z, worldViewProjection);
			ProjectedVector.X = (((ProjectedVector.X + 1f) * 0.5f) * viewport.Width);
			ProjectedVector.Y = (((-ProjectedVector.Y + 1f) * 0.5f) * viewport.Height);

			var z = (((X * worldView.M13) + (Y * worldView.M23)) + (Z * worldView.M33)) + (worldView.M43);
			z = (z / viewport.View.DepthDivisor) + viewport.View.DepthZero;
			ProjectedVector.Z = z;

			// Todo: calculating the rotated normal should only be done when necessary - performance boost!
			TransformedNormal = Vector.TransformNormal(NormalX, NormalY, NormalZ, worldView);

			TransformedVector = Vector.Transform(X, Y, Z, worldView);
			TransformedVectorNormalized = TransformedVector;
			TransformedVectorNormalized.Normalize();
		}

		public Vector ProjectedVector;
		public Vector TransformedNormal;
		public Vector TransformedVector;
		public Vector TransformedVectorNormalized;

		public float U1;
		public float V1;

		public float U2;
		public float V2;
	}
}
#endif