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
    public class BoundingObject : IBoundingObject
    {
        public Vector Center { get { return BoundingSphere != null ? BoundingSphere.Center : Vector.Zero; } }
        public float Radius { get { return BoundingSphere != null ? BoundingSphere.Radius : 0; } }
        public BoundingBox BoundingBox { get; private set; }
        public BoundingSphere BoundingSphere { get; private set; }

        public bool IsSet { get { return BoundingBox != null || (BoundingSphere != null && BoundingSphere.IsSet); } }
        public bool IsBox { get { return BoundingBox != null; } }
        public bool IsSphere { get { return BoundingSphere != null; } }

        public void Sphere(float radius)
        {
            Sphere(Vector.Zero, radius);
        }

        public void Sphere(Vector center, float radius)
        {
            BoundingBox = null;
            BoundingSphere = new BoundingSphere(center, radius);
        }


        public void Box(Vector min, Vector max)
        {
            BoundingBox = new BoundingBox(min, max);
        }

        public bool Intersects(BoundingBox box)
        {
            if (IsSphere) return BoundingSphere.Intersects(box);
            if (IsBox) return BoundingSphere.Intersects(box);
            return false;
        }

        public void Intersects(ref BoundingBox box, out bool result)
        {
            if (IsSphere) BoundingSphere.Intersects(ref box, out result);
            if (IsBox) BoundingSphere.Intersects(ref box, out result);
            result = false;
        }

        public PlaneIntersectionType Intersects(Plane plane)
        {
            if (IsSphere) return BoundingSphere.Intersects(plane);
            if (IsBox) return BoundingSphere.Intersects(plane);
            return PlaneIntersectionType.None;
        }

        public float? Intersects(Ray ray)
        {
            if (IsSphere) return BoundingSphere.Intersects(ray);
            if (IsBox) return BoundingBox.Intersects(ray);
            return null;
        }

        public bool Intersects(BoundingSphere sphere)
        {
            if (IsSphere) return BoundingSphere.Intersects(sphere);
            if (IsBox) return BoundingBox.Intersects(sphere);
            return false;
        }

        public void Intersects(ref BoundingSphere sphere, out bool result)
        {
            if (IsSphere) BoundingSphere.Intersects(ref sphere, out result);
            if (IsBox) BoundingBox.Intersects(ref sphere, out result);
            result = false;
        }

        public ContainmentType Contains(BoundingBox box)
        {
            if (IsSphere) return BoundingSphere.Contains(box);
            if (IsBox) return BoundingBox.Contains(box);
            return ContainmentType.DoesNotContain;
        }

        public void Contains(ref BoundingBox box, out ContainmentType result)
        {
            if (IsSphere) BoundingSphere.Contains(ref box, out result);
            if (IsBox) BoundingBox.Contains(ref box, out result);
            result = ContainmentType.DoesNotContain;
        }

        public ContainmentType Contains(Vector point)
        {
            if (IsSphere) return BoundingSphere.Contains(point);
            if (IsBox) return BoundingBox.Contains(point);
            return ContainmentType.DoesNotContain;
        }

        public void Contains(ref Vector point, out ContainmentType result)
        {
            if (IsSphere) BoundingSphere.Contains(ref point, out result);
            if (IsBox) BoundingBox.Contains(ref point, out result);
            result = ContainmentType.DoesNotContain;
        }

        public ContainmentType Contains(BoundingSphere sphere)
        {
            if (IsSphere) return BoundingSphere.Contains(sphere);
            if (IsBox) return BoundingBox.Contains(sphere);
            return ContainmentType.DoesNotContain;
        }

        public void Contains(ref BoundingSphere sphere, out ContainmentType result)
        {
            if (IsSphere) BoundingSphere.Contains(ref sphere, out result);
            if (IsBox) BoundingBox.Contains(ref sphere, out result);
            result = ContainmentType.DoesNotContain;
        }

        public IBoundingObject Transform(Matrix matrix)
        {
            var boundingObject = new BoundingObject();
            if (IsSphere) boundingObject.BoundingSphere = BoundingSphere.Transform(matrix);
            if (IsBox) boundingObject.BoundingBox = BoundingBox.Transform(matrix);
            return boundingObject;
        }

        public void Include(IBoundingObject boundingObject)
        {
            if ((IsSphere || !IsSet) && boundingObject.IsSphere) BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere ?? new BoundingSphere(Vector.Zero, 0), boundingObject.BoundingSphere);
            if ((IsBox || !IsSet) && boundingObject.IsBox) BoundingBox = BoundingBox.CreateMerged(BoundingBox ?? new BoundingBox(Vector.Zero, Vector.Zero), boundingObject.BoundingBox);
        }

        public void SetCenter(float x, float y, float z)
        {
            if (IsSphere) BoundingSphere.Center.Set(x, y, z);
        }
    }
}
