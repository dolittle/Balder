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

using Balder.Display;
using Balder.View;

namespace Balder.Math
{
	public class Frustum
	{
		private readonly Plane[] _planes = new Plane[(int)FrustumLocation.Total];
		private Vector _nearTopLeft, _nearTopRight, _nearBottomLeft, _nearBottomRight, _farTopLeft, _farTopRight, _farBottomLeft, _farBottomRight;
		private float _near;
		private float _far;
		private float _tang;
		private float _nearWidth;
		private float _nearHeight;
		private float _farWidth;
		private float _farHeight;


		public Frustum()
		{
			for (var planeIndex = 0; planeIndex < _planes.Length; planeIndex++)
			{
				_planes[planeIndex] = new Plane();
			}
		}


		public void SetCameraInternals(float fieldOfView, float aspectRatio, float near, float far)
		{
			_near = near;
			_far = far;

			var fieldOfViewRadians = MathHelper.ToRadians(fieldOfView);
			_tang = (float)System.Math.Tan(fieldOfViewRadians*0.5);
			_nearHeight = near * _tang;
			_nearWidth = _nearHeight * aspectRatio;
			_farHeight = far * _tang;
			_farWidth = _farHeight * aspectRatio;
		}


		public void SetCameraDefinition(Viewport viewport, Camera camera)
		{
			SetCameraInternals((float)camera.FieldOfView, viewport.AspectRatio, camera.Near, camera.Far);

			var z = camera.Position-camera.Target;
			z.Normalize();

			var x = camera.Up * z;
			x.Normalize();

			var y = z * x;
			

			var nearClip = camera.Position - (z * _near);

			_nearTopLeft = nearClip + (y * _nearHeight) - (x * _nearWidth);
			_nearTopRight = nearClip + (y * _nearHeight) + (x * _nearWidth);
			_nearBottomLeft = nearClip - (y * _nearHeight) - (x * _nearWidth);
			_nearBottomRight = nearClip - (y * _nearHeight) + (x * _nearWidth);

			var farClip = camera.Position - (z * _far);

			_farTopLeft = farClip + (y * _farHeight) - (x * _farWidth);
			_farTopRight = farClip + (y * _farHeight) + (x * _farWidth);
			_farBottomLeft = farClip - (y * _farHeight) - (x * _farWidth);
			_farBottomRight = farClip - (y * _farHeight) + (x * _farWidth);


			_planes[(int)FrustumLocation.Top].SetVectors(_nearTopRight, _nearTopLeft, _farTopLeft);
			_planes[(int)FrustumLocation.Bottom].SetVectors(_nearBottomLeft, _nearBottomRight, _farBottomRight);
			_planes[(int)FrustumLocation.Left].SetVectors(_nearTopLeft, _nearBottomLeft, _farBottomLeft);
			_planes[(int)FrustumLocation.Right].SetVectors(_nearBottomRight, _nearTopRight, _farBottomRight);
			_planes[(int)FrustumLocation.Near].SetVectors(_nearTopLeft, _nearTopRight, _nearBottomRight);
			_planes[(int)FrustumLocation.Far].SetVectors(_farTopRight, _farTopLeft, _farBottomLeft);
		}



		public FrustumIntersection IsPointInFrustum(Vector vector)
		{
			for( var planeIndex=0; planeIndex<_planes.Length; planeIndex++ )
			{
				var plane = _planes[planeIndex];
				var distance = plane.GetDistanceFromVector(vector);
                if ( distance < 0)
                {
                    return FrustumIntersection.Outside;
                }
			}
            return FrustumIntersection.Inside;

		}

		public FrustumIntersection IsSphereInFrustum(Vector vector, float radius)
		{
            float distance = 0;
			for (var planeIndex = 0; planeIndex < _planes.Length; planeIndex++)
			{
				var plane = _planes[planeIndex];
				distance = plane.GetDistanceFromVector(vector);
                if (distance < -radius)
                {
                    return FrustumIntersection.Outside;
                }
                else if (distance < radius)
                {
                    return FrustumIntersection.Intersect;
                }
            }
            return FrustumIntersection.Inside;
		}
	}
}
