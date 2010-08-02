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
using System.Globalization;

namespace Balder.Math
{
	public struct Ray : IEquatable<Ray>
	{
		public Vector Position;
		public Vector Direction;

		public Ray(Vector position, Vector direction)
			: this()
		{
			Position = position;
			Direction = direction;
		}

		public bool Equals(Ray other)
		{
			return (((((Position.X == other.Position.X) && (Position.Y == other.Position.Y)) && ((Position.Z == other.Position.Z) && (Direction.X == other.Direction.X))) && (Direction.Y == other.Direction.Y)) && (Direction.Z == other.Direction.Z));
		}

		public override bool Equals(object obj)
		{
			bool flag = false;
			if ((obj != null) && (obj is Ray))
			{
				flag = Equals((Ray)obj);
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (Position.GetHashCode() + Direction.GetHashCode());
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{{Position:{0} Direction:{1}}}", new object[] { Position.ToString(), Direction.ToString() });
		}

		public float? Intersects(BoundingBox box)
		{
			return box.Intersects(this);
		}


		public float? Intersects(Plane plane)
		{
			float num2 = ((plane.Normal.X * Direction.X) + (plane.Normal.Y * Direction.Y)) + (plane.Normal.Z * Direction.Z);
			if (System.Math.Abs(num2) < 1E-05f)
			{
				return null;
			}
			float num3 = ((plane.Normal.X * Position.X) + (plane.Normal.Y * Position.Y)) + (plane.Normal.Z * Position.Z);
			float num = (-plane.Distance - num3) / num2;
			if (num < 0f)
			{
				if (num < -1E-05f)
				{
					return null;
				}
				num = 0f;
			}
			return new float?(num);
		}

		public float? Intersects(BoundingSphere sphere)
		{
			var distance = sphere.Center - Position;
			var lengthSquared = distance.LengthSquared();
			var radiusSquared = sphere.Radius * sphere.Radius;
			if (lengthSquared <= radiusSquared)
			{
				return 0f;
			}
			var dotProduct = Vector.Dot(distance, Direction);
			if (dotProduct < 0f)
			{
				return null;
			}

			var result = lengthSquared - (dotProduct * dotProduct);
			if (result > radiusSquared)
			{
				return null;
			}
			var actual = (float)System.Math.Sqrt((double)radiusSquared - result);
			return actual;
		}


		public float? IntersectsTriangle(Vector vector1, Vector vector2, Vector vector3)
		{
			float u, v;
			return IntersectsTriangle(vector1, vector2, vector3, out u, out v);
		}
		

		public float? IntersectsTriangle(Vector vector1, Vector vector2, Vector vector3, out float triangleU, out float triangleV)
		{
			var edge1 = vector2 - vector1;
			var edge2 = vector3 - vector1;
			var directionCrossEdge2 = Vector.Cross(Direction, edge2);
			var determinant = Vector.Dot(edge1, directionCrossEdge2);

			triangleU = 0;
			triangleV = 0;

			// BackFace Culling
			if (determinant >= 0) 
			{
				return null;
			}

			var inverseDeterminant = 1.0f / determinant;
			var distanceVector = Position - vector1;

			triangleU = Vector.Dot(distanceVector, directionCrossEdge2);
			triangleU *= inverseDeterminant;
			

			if (triangleU < 0 || triangleU > 1)
			{
				return null;
			}

			var distanceCrossEdge1 = Vector.Cross(distanceVector, edge1);
			triangleV = Vector.Dot(Direction, distanceCrossEdge1);
			triangleV *= inverseDeterminant;

			if (triangleV < 0 || triangleV > 1)
			{
				return null;
			}

			var rayDistance = Vector.Dot(edge2, distanceCrossEdge1);
			rayDistance *= inverseDeterminant;
			return rayDistance;
		}

		public static bool operator ==(Ray a, Ray b)
		{
			return (((((a.Position.X == b.Position.X) && (a.Position.Y == b.Position.Y)) && ((a.Position.Z == b.Position.Z) && (a.Direction.X == b.Direction.X))) && (a.Direction.Y == b.Direction.Y)) && (a.Direction.Z == b.Direction.Z));
		}

		public static bool operator !=(Ray a, Ray b)
		{
			if ((((a.Position.X == b.Position.X) && (a.Position.Y == b.Position.Y)) && ((a.Position.Z == b.Position.Z) && (a.Direction.X == b.Direction.X))) && (a.Direction.Y == b.Direction.Y))
			{
				return (a.Direction.Z != b.Direction.Z);
			}
			return true;
		}
	}
}
