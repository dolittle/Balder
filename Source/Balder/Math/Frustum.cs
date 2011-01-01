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
		private Vector nearTopLeft, nearTopRight, nearBottomLeft, nearBottomRight, farTopLeft, farTopRight, farBottomLeft, farBottomRight;
		private float _near;
		private float _far;
		private float _aspectRatio;
		private float _fieldOfView;
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


		private const double ANG2RAD = 3.14159265358979323846/180.0;

		public void SetCameraInternals(float fieldOfView, float aspectRatio, float near, float far)
		{
			_fieldOfView = fieldOfView;
			_aspectRatio = aspectRatio;
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

			nearTopLeft = nearClip + (y * _nearHeight) - (x * _nearWidth);
			nearTopRight = nearClip + (y * _nearHeight) + (x * _nearWidth);
			nearBottomLeft = nearClip - (y * _nearHeight) - (x * _nearWidth);
			nearBottomRight = nearClip - (y * _nearHeight) + (x * _nearWidth);

			var farClip = camera.Position - (z * _far);

			farTopLeft = farClip + (y * _farHeight) - (x * _farWidth);
			farTopRight = farClip + (y * _farHeight) + (x * _farWidth);
			farBottomLeft = farClip - (y * _farHeight) - (x * _farWidth);
			farBottomRight = farClip - (y * _farHeight) + (x * _farWidth);


			_planes[(int)FrustumLocation.Top].SetVectors(nearTopRight, nearTopLeft, farTopLeft);
			_planes[(int)FrustumLocation.Bottom].SetVectors(nearBottomLeft, nearBottomRight, farBottomRight);
			_planes[(int)FrustumLocation.Left].SetVectors(nearTopLeft, nearBottomLeft, farBottomLeft);
			_planes[(int)FrustumLocation.Right].SetVectors(nearBottomRight, nearTopRight, farBottomRight);
			_planes[(int)FrustumLocation.Near].SetVectors(nearTopLeft, nearTopRight, farBottomRight);
			_planes[(int)FrustumLocation.Far].SetVectors(farTopRight, farTopLeft, farBottomLeft);
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
