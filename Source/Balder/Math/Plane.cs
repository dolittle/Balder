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
using System;

namespace Balder.Math
{
	public class Plane
	{
		private Vector _vector1;
		private Vector _vector2;
		private Vector _vector3;
		private Vector _point;

		public Plane()
		{

		}

		public Plane(Vector vector, float distance)
		{
			Normal = vector;
			Normal.Normalize();
			Distance = distance;
		}

		public Vector Normal;
		public float Distance;


		public void SetVectors(Vector vector1, Vector vector2, Vector vector3)
		{
			_vector1 = vector1;
			_vector2 = vector2;
			_vector3 = vector3;

			var aux1 = vector1 - vector2;
			var aux2 = vector3 - vector2;

			Normal = aux2 * aux1;
			Normal.Normalize();
			_point = vector2;
			Distance = -Normal.Dot(_point);
		}


		public void SetNormalAndPoint(Vector normal, Vector point)
		{
			Normal = normal;
			Normal.Normalize();
			_point = point;
			Distance = -Normal.Dot(_point);
		}

		public void SetCoefficients(float a, float b, float c, float d)
		{
			Normal.X = a;
			Normal.Y = b;
			Normal.Z = c;

			var length = Normal.Length;

			Normal.Normalize();

			Distance = d / length;
		}

		public float GetDistanceFromVector(Vector vector)
		{
			return Normal.Dot(vector) + Distance;
		}

		public PlaneIntersectionType Intersects(BoundingBox box)
		{
            var cornersOutside = 0;
            var corners = box.GetCorners();
            for (var cornerIndex = 0; cornerIndex < corners.Length; cornerIndex++)
            {
                var distance = GetDistanceFromVector(corners[cornerIndex]);
                if (distance < 0) cornersOutside++;
            }

            if (cornersOutside == corners.Length) return PlaneIntersectionType.Back;
            if (cornersOutside == 0) return PlaneIntersectionType.Front;

            return PlaneIntersectionType.Intersecting;
		}


		public PlaneIntersectionType Intersects(BoundingSphere sphere)
		{
			float num2 = ((sphere.Center.X * this.Normal.X) + (sphere.Center.Y * this.Normal.Y)) + (sphere.Center.Z * this.Normal.Z);
			float num = num2 + this.Distance;
			if (num > sphere.Radius)
			{
				return PlaneIntersectionType.Front;
			}
			if (num < -sphere.Radius)
			{
				return PlaneIntersectionType.Back;
			}
			return PlaneIntersectionType.Intersecting;
		}

	}
}
