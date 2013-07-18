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
    public static class PlaneIntersectionTypeExtensions
    {
        public static FrustumIntersection ToFrustumIntersection(this PlaneIntersectionType intersection)
        {
            switch (intersection)
            {
                case PlaneIntersectionType.Back:
                    return FrustumIntersection.Outside;
                case PlaneIntersectionType.Front:
                    return FrustumIntersection.Inside;
                case PlaneIntersectionType.Intersecting:
                    return FrustumIntersection.Intersect;
            }

            return FrustumIntersection.Inside;
        }
    }
}
