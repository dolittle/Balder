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
namespace Balder.Math
{
    public interface IBoundingObject
    {
        bool IsSet { get; }

        bool IsBox { get; }
        bool IsSphere { get; }
        
        void Sphere(float radius);
        void Sphere(Vector center, float radius);
        void Box(Vector min, Vector max);

        Vector Center { get; }
        float Radius { get; }

        BoundingBox BoundingBox { get; }
        BoundingSphere BoundingSphere { get; }

        void SetCenter(float x, float y, float z);

        bool Intersects(BoundingBox box);
        void Intersects(ref BoundingBox box, out bool result);
        PlaneIntersectionType Intersects(Plane plane);
        float? Intersects(Ray ray);
        bool Intersects(BoundingSphere sphere);
        void Intersects(ref BoundingSphere sphere, out bool result);
        ContainmentType Contains(BoundingBox box);
        void Contains(ref BoundingBox box, out ContainmentType result);
        ContainmentType Contains(Vector point);
        void Contains(ref Vector point, out ContainmentType result);
        ContainmentType Contains(BoundingSphere sphere);
        void Contains(ref BoundingSphere sphere, out ContainmentType result);
        IBoundingObject Transform(Matrix matrix);

        void Include(IBoundingObject boundingObject);

    }
}
