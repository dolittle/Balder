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
using System.Collections.Generic;
using System.Globalization;

namespace Balder.Math
{
	public class BoundingBox : IEquatable<BoundingBox>
	{
        public Matrix World { get; set; }

		public const int CornerCount = 8;
		public Vector Min;
		public Vector Max;
		public Vector[] GetCorners()
		{
			var corners = new Vector[] { 
                new Vector(Min.X, Max.Y, Max.Z), 
                new Vector(Max.X, Max.Y, Max.Z), 
                new Vector(Max.X, Min.Y, Max.Z), 
                new Vector(Min.X, Min.Y, Max.Z), 
                new Vector(Min.X, Max.Y, Min.Z), 
                new Vector(Max.X, Max.Y, Min.Z), 
                new Vector(Max.X, Min.Y, Min.Z), 
                new Vector(Min.X, Min.Y, Min.Z) 
            };  
            if (World != null && !World.IsIdentity)
                for (var cornerIndex = 0; cornerIndex < corners.Length; cornerIndex++)
                    corners[cornerIndex] = Vector.Transform(corners[cornerIndex], World);

            return corners;
		}


        public BoundingBox(Vector min, Vector max)
		{
			this.Min = min;
			this.Max = max;
		}

        public Vector Center { get { return (Min + Max)/2; } }
        public Vector Size { get { return (Max - Min)/2; } }

		public bool Equals(BoundingBox other)
		{
			return ((this.Min == other.Min) && (this.Max == other.Max));
		}

		public override bool Equals(object obj)
		{
			bool flag = false;
			if (obj is BoundingBox)
			{
				flag = this.Equals((BoundingBox)obj);
			}
			return flag;
		}

		public override int GetHashCode()
		{
			return (this.Min.GetHashCode() + this.Max.GetHashCode());
		}

		public override string ToString()
		{
			return string.Format(CultureInfo.CurrentCulture, "{{Min:{0} Max:{1}}}", new object[] { this.Min.ToString(), this.Max.ToString() });
		}

		public static BoundingBox CreateMerged(BoundingBox original, BoundingBox additional)
		{
            var lowestX = float.MaxValue;
            var lowestY = float.MaxValue;
            var lowestZ = float.MaxValue;
            var highestX = float.MinValue;
            var highestY = float.MinValue;
            var highestZ = float.MinValue;

            var corners = additional.GetCorners();
            foreach (var corner in corners)
            {
                if (corner.X < lowestX) lowestX = corner.X;
                if (corner.Y < lowestY) lowestY = corner.Y;
                if (corner.Z < lowestZ) lowestZ = corner.Z;
                if (corner.X > highestX) highestX = corner.X;
                if (corner.Y > highestY) highestY = corner.Y;
                if (corner.Z > highestZ) highestZ = corner.Z;
            }
            var lowest = new Vector(lowestX, lowestY, lowestZ);
            var highest = new Vector(highestX, highestY, highestZ);

            if (!original.IsSet)
                return new BoundingBox(lowest, highest);


			var min = Vector.Min(original.Min, lowest);
            var max = Vector.Max(original.Max, highest);

            var box = new BoundingBox(min,max);
			return box;
		}

        bool IsSet { get { return !(Min == Vector.Zero && Max == Vector.Zero); } }


		public static BoundingBox CreateFromSphere(BoundingSphere sphere)
		{
			var min = Vector.Zero;
			var max = Vector.Zero;
			min.X = sphere.Center.X - sphere.Radius;
			min.Y = sphere.Center.Y - sphere.Radius;
			min.Z = sphere.Center.Z - sphere.Radius;
			max.X = sphere.Center.X + sphere.Radius;
			max.Y = sphere.Center.Y + sphere.Radius;
			max.Z = sphere.Center.Z + sphere.Radius;
			var box = new BoundingBox(min,max);
			return box;
		}

		public static BoundingBox CreateFromPoints(IEnumerable<Vector> points)
		{
			if (points == null)
			{
				throw new ArgumentNullException();
			}
			var flag = false;
			var vector3 = new Vector(float.MaxValue, float.MaxValue, float.MaxValue);
			var vector2 = new Vector(float.MinValue, float.MinValue, float.MinValue);
			foreach (var vector in points)
			{
				Vector vector4 = vector;
				vector3 = Vector.Min(vector3, vector4);
				vector2 = Vector.Max(vector2, vector4);
				flag = true;
			}
			if (!flag)
			{
				throw new ArgumentException("Bounding box must have more than 0 points");
			}
			return new BoundingBox(vector3, vector2);
		}

		public bool Intersects(BoundingBox box)
		{
			if ((this.Max.X < box.Min.X) || (this.Min.X > box.Max.X))
			{
				return false;
			}
			if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
			{
				return false;
			}
			return ((this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z));
		}

		public void Intersects(ref BoundingBox box, out bool result)
		{
			result = false;
			if ((((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X)) && ((this.Max.Y >= box.Min.Y) && (this.Min.Y <= box.Max.Y))) && ((this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z)))
			{
				result = true;
			}
		}


		public PlaneIntersectionType Intersects(Plane plane)
		{
			var vector = Vector.Zero;
			var vector2 = Vector.Zero;
			vector2.X = (plane.Normal.X >= 0f) ? this.Min.X : this.Max.X;
			vector2.Y = (plane.Normal.Y >= 0f) ? this.Min.Y : this.Max.Y;
			vector2.Z = (plane.Normal.Z >= 0f) ? this.Min.Z : this.Max.Z;
			vector.X = (plane.Normal.X >= 0f) ? this.Max.X : this.Min.X;
			vector.Y = (plane.Normal.Y >= 0f) ? this.Max.Y : this.Min.Y;
			vector.Z = (plane.Normal.Z >= 0f) ? this.Max.Z : this.Min.Z;
			float num = ((plane.Normal.X * vector2.X) + (plane.Normal.Y * vector2.Y)) + (plane.Normal.Z * vector2.Z);
			if ((num + plane.Distance) > 0f)
			{
				return PlaneIntersectionType.Front;
			}
			num = ((plane.Normal.X * vector.X) + (plane.Normal.Y * vector.Y)) + (plane.Normal.Z * vector.Z);
			if ((num + plane.Distance) < 0f)
			{
				return PlaneIntersectionType.Back;
			}
			return PlaneIntersectionType.Intersecting;
		}

		public void Intersects(ref Plane plane, out PlaneIntersectionType result)
		{
			var vector = Vector.Zero;
			var vector2 = Vector.Zero;
			vector2.X = (plane.Normal.X >= 0f) ? this.Min.X : this.Max.X;
			vector2.Y = (plane.Normal.Y >= 0f) ? this.Min.Y : this.Max.Y;
			vector2.Z = (plane.Normal.Z >= 0f) ? this.Min.Z : this.Max.Z;
			vector.X = (plane.Normal.X >= 0f) ? this.Max.X : this.Min.X;
			vector.Y = (plane.Normal.Y >= 0f) ? this.Max.Y : this.Min.Y;
			vector.Z = (plane.Normal.Z >= 0f) ? this.Max.Z : this.Min.Z;
			float num = ((plane.Normal.X * vector2.X) + (plane.Normal.Y * vector2.Y)) + (plane.Normal.Z * vector2.Z);
			if ((num + plane.Distance) > 0f)
			{
				result = PlaneIntersectionType.Front;
			}
			else
			{
				num = ((plane.Normal.X * vector.X) + (plane.Normal.Y * vector.Y)) + (plane.Normal.Z * vector.Z);
				if ((num + plane.Distance) < 0f)
				{
					result = PlaneIntersectionType.Back;
				}
				else
				{
					result = PlaneIntersectionType.Intersecting;
				}
			}
		}

		public float? Intersects(Ray ray)
		{

			float num = 0f;
			float maxValue = float.MaxValue;
			if (System.Math.Abs(ray.Direction.X) < 1E-06f)
			{
				if ((ray.Position.X < this.Min.X) || (ray.Position.X > this.Max.X))
				{
					return null;
				}
			}
			else
			{
				float num11 = 1f / ray.Direction.X;
				float num8 = (this.Min.X - ray.Position.X) * num11;
				float num7 = (this.Max.X - ray.Position.X) * num11;
				if (num8 > num7)
				{
					float num14 = num8;
					num8 = num7;
					num7 = num14;
				}
				num = MathHelper.Max(num8, num);
				maxValue = MathHelper.Min(num7, maxValue);
				if (num > maxValue)
				{
					return null;
				}
			}
			if (System.Math.Abs(ray.Direction.Y) < 1E-06f)
			{
				if ((ray.Position.Y < this.Min.Y) || (ray.Position.Y > this.Max.Y))
				{
					return null;
				}
			}
			else
			{
				float num10 = 1f / ray.Direction.Y;
				float num6 = (this.Min.Y - ray.Position.Y) * num10;
				float num5 = (this.Max.Y - ray.Position.Y) * num10;
				if (num6 > num5)
				{
					float num13 = num6;
					num6 = num5;
					num5 = num13;
				}
				num = MathHelper.Max(num6, num);
				maxValue = MathHelper.Min(num5, maxValue);
				if (num > maxValue)
				{
					return null;
				}
			}
			if (System.Math.Abs(ray.Direction.Z) < 1E-06f)
			{
				if ((ray.Position.Z < this.Min.Z) || (ray.Position.Z > this.Max.Z))
				{
					return null;
				}
			}
			else
			{
				float num9 = 1f / ray.Direction.Z;
				float num4 = (this.Min.Z - ray.Position.Z) * num9;
				float num3 = (this.Max.Z - ray.Position.Z) * num9;
				if (num4 > num3)
				{
					float num12 = num4;
					num4 = num3;
					num3 = num12;
				}
				num = MathHelper.Max(num4, num);
				maxValue = MathHelper.Min(num3, maxValue);
				if (num > maxValue)
				{
					return null;
				}
			}
			return new float?(num);
		}

		public void Intersects(ref Ray ray, out float? result)
		{
			result = 0;
			float num = 0f;
			float maxValue = float.MaxValue;
			if (System.Math.Abs(ray.Direction.X) < 1E-06f)
			{
				if ((ray.Position.X < this.Min.X) || (ray.Position.X > this.Max.X))
				{
					return;
				}
			}
			else
			{
				float num11 = 1f / ray.Direction.X;
				float num8 = (this.Min.X - ray.Position.X) * num11;
				float num7 = (this.Max.X - ray.Position.X) * num11;
				if (num8 > num7)
				{
					float num14 = num8;
					num8 = num7;
					num7 = num14;
				}
				num = MathHelper.Max(num8, num);
				maxValue = MathHelper.Min(num7, maxValue);
				if (num > maxValue)
				{
					return;
				}
			}
			if (System.Math.Abs(ray.Direction.Y) < 1E-06f)
			{
				if ((ray.Position.Y < this.Min.Y) || (ray.Position.Y > this.Max.Y))
				{
					return;
				}
			}
			else
			{
				float num10 = 1f / ray.Direction.Y;
				float num6 = (this.Min.Y - ray.Position.Y) * num10;
				float num5 = (this.Max.Y - ray.Position.Y) * num10;
				if (num6 > num5)
				{
					float num13 = num6;
					num6 = num5;
					num5 = num13;
				}
				num = MathHelper.Max(num6, num);
				maxValue = MathHelper.Min(num5, maxValue);
				if (num > maxValue)
				{
					return;
				}
			}
			if (System.Math.Abs(ray.Direction.Z) < 1E-06f)
			{
				if ((ray.Position.Z < this.Min.Z) || (ray.Position.Z > this.Max.Z))
				{
					return;
				}
			}
			else
			{
				float num9 = 1f / ray.Direction.Z;
				float num4 = (this.Min.Z - ray.Position.Z) * num9;
				float num3 = (this.Max.Z - ray.Position.Z) * num9;
				if (num4 > num3)
				{
					float num12 = num4;
					num4 = num3;
					num3 = num12;
				}
				num = MathHelper.Max(num4, num);
				maxValue = MathHelper.Min(num3, maxValue);
				if (num > maxValue)
				{
					return;
				}
			}
			result = new float?(num);
		}

		public bool Intersects(BoundingSphere sphere)
		{
			var vector = Vector.Clamp(sphere.Center, this.Min, this.Max);
			var num = Vector.DistanceSquared(sphere.Center, vector);
			return (num <= (sphere.Radius * sphere.Radius));
		}

		public void Intersects(ref BoundingSphere sphere, out bool result)
		{
			var vector = Vector.Clamp(sphere.Center, this.Min, this.Max);
			var num = Vector.DistanceSquared(sphere.Center, vector);
			result = num <= (sphere.Radius * sphere.Radius);
		}

		public ContainmentType Contains(BoundingBox box)
		{
			if ((this.Max.X < box.Min.X) || (this.Min.X > box.Max.X))
			{
				return ContainmentType.Disjoint;
			}
			if ((this.Max.Y < box.Min.Y) || (this.Min.Y > box.Max.Y))
			{
				return ContainmentType.Disjoint;
			}
			if ((this.Max.Z < box.Min.Z) || (this.Min.Z > box.Max.Z))
			{
				return ContainmentType.Disjoint;
			}
			if ((((this.Min.X <= box.Min.X) && (box.Max.X <= this.Max.X)) && ((this.Min.Y <= box.Min.Y) && (box.Max.Y <= this.Max.Y))) && ((this.Min.Z <= box.Min.Z) && (box.Max.Z <= this.Max.Z)))
			{
				return ContainmentType.Contains;
			}
			return ContainmentType.Intersects;
		}

		public void Contains(ref BoundingBox box, out ContainmentType result)
		{
			result = ContainmentType.Disjoint;
			if ((((this.Max.X >= box.Min.X) && (this.Min.X <= box.Max.X)) && ((this.Max.Y >= box.Min.Y) && (this.Min.Y <= box.Max.Y))) && ((this.Max.Z >= box.Min.Z) && (this.Min.Z <= box.Max.Z)))
			{
				result = ((((this.Min.X <= box.Min.X) && (box.Max.X <= this.Max.X)) && ((this.Min.Y <= box.Min.Y) && (box.Max.Y <= this.Max.Y))) && ((this.Min.Z <= box.Min.Z) && (box.Max.Z <= this.Max.Z))) ? ContainmentType.Contains : ContainmentType.Intersects;
			}
		}


		public ContainmentType Contains(Vector point)
		{
			if ((((this.Min.X <= point.X) && (point.X <= this.Max.X)) && ((this.Min.Y <= point.Y) && (point.Y <= this.Max.Y))) && ((this.Min.Z <= point.Z) && (point.Z <= this.Max.Z)))
			{
				return ContainmentType.Contains;
			}
			return ContainmentType.Disjoint;
		}

		public void Contains(ref Vector point, out ContainmentType result)
		{
			result = ((((this.Min.X <= point.X) && (point.X <= this.Max.X)) && ((this.Min.Y <= point.Y) && (point.Y <= this.Max.Y))) && ((this.Min.Z <= point.Z) && (point.Z <= this.Max.Z))) ? ContainmentType.Contains : ContainmentType.Disjoint;
		}

		public ContainmentType Contains(BoundingSphere sphere)
		{
			var vector = Vector.Clamp(sphere.Center, this.Min, this.Max);
			var num2 = Vector.DistanceSquared(sphere.Center, vector);
			float radius = sphere.Radius;
			if (num2 > (radius * radius))
			{
				return ContainmentType.Disjoint;
			}
			if (((((this.Min.X + radius) <= sphere.Center.X) && (sphere.Center.X <= (this.Max.X - radius))) && (((this.Max.X - this.Min.X) > radius) && ((this.Min.Y + radius) <= sphere.Center.Y))) && (((sphere.Center.Y <= (this.Max.Y - radius)) && ((this.Max.Y - this.Min.Y) > radius)) && ((((this.Min.Z + radius) <= sphere.Center.Z) && (sphere.Center.Z <= (this.Max.Z - radius))) && ((this.Max.X - this.Min.X) > radius))))
			{
				return ContainmentType.Contains;
			}
			return ContainmentType.Intersects;
		}

		public void Contains(ref BoundingSphere sphere, out ContainmentType result)
		{
			var vector = Vector.Clamp(sphere.Center, this.Min, this.Max);
			var num2 = Vector.DistanceSquared(sphere.Center, vector);
			float radius = sphere.Radius;
			if (num2 > (radius * radius))
			{
				result = ContainmentType.Disjoint;
			}
			else
			{
				result = (((((this.Min.X + radius) <= sphere.Center.X) && (sphere.Center.X <= (this.Max.X - radius))) && (((this.Max.X - this.Min.X) > radius) && ((this.Min.Y + radius) <= sphere.Center.Y))) && (((sphere.Center.Y <= (this.Max.Y - radius)) && ((this.Max.Y - this.Min.Y) > radius)) && ((((this.Min.Z + radius) <= sphere.Center.Z) && (sphere.Center.Z <= (this.Max.Z - radius))) && ((this.Max.X - this.Min.X) > radius)))) ? ContainmentType.Contains : ContainmentType.Intersects;
			}
		}

		internal Vector SupportMapping(ref Vector v)
		{
			var result = Vector.Zero;
			result.X = (v.X >= 0f) ? this.Max.X : this.Min.X;
			result.Y = (v.Y >= 0f) ? this.Max.Y : this.Min.Y;
			result.Z = (v.Z >= 0f) ? this.Max.Z : this.Min.Z;
			return result;
		}

        public BoundingBox Transform(Matrix matrix)
        {
            var boundingBox = new BoundingBox(Min, Max);
            boundingBox.World = matrix;
            return boundingBox;
        }
	}
}
