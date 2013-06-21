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
using Balder.Execution;
using Balder.Math;
using Balder.Rendering;
using Ninject;
using Matrix = Balder.Math.Matrix;

namespace Balder.Debug
{
	[Singleton]
	public class DebugRenderer : IDebugRenderer
	{
		IKernel _kernel;
		DebugShape _boundingSphereDebugShape;
        DebugShape _boundingBoxDebugShape;

		RayDebugShape _rayDebugShape;
		RectangleDebugShape _rectangleDebugShape;

		public DebugRenderer(IKernel kernel)
		{
			_kernel = kernel;
			CreateShapes();
		}

		// Todo: Get rid of this singleton - find a good way for handling same behavior 
		private static object InstanceLockObject = new object();
		private static IDebugRenderer _instance;
		internal static IDebugRenderer Instance
		{
			get
			{
				lock (InstanceLockObject)
				{
					if (null == _instance)
					{
						_instance = Runtime.Instance.Kernel.Get<IDebugRenderer>();
					}
					return _instance;
				}

			}
		}

		private void CreateShapes()
		{
			_boundingSphereDebugShape = _kernel.Get<BoundingSphereDebugShape>();
			_boundingSphereDebugShape.OnInitialize();
            _boundingBoxDebugShape = _kernel.Get<BoundingBoxDebugShape>();
            _boundingBoxDebugShape.OnInitialize();
			_rayDebugShape = _kernel.Get<RayDebugShape>();
			_rayDebugShape.OnInitialize();
			_rectangleDebugShape = _kernel.Get<RectangleDebugShape>();
			_rectangleDebugShape.OnInitialize();
		}

        public void RenderBoundingSphere(BoundingSphere sphere, Viewport viewport, DetailLevel detailLevel, Matrix world, bool topLevel = false)
		{
			var scaleMatrix = Matrix.CreateScale(sphere.Radius);
			var translationMatrix = Matrix.CreateTranslation(sphere.Center) * world;
			var rotateYMatrix = Matrix.CreateRotationY(90);
			var rotateXMatrix = Matrix.CreateRotationX(90);

            _boundingSphereDebugShape.Color = topLevel ? viewport.DebugInfo.TopLevelBoundingObjectsColor : viewport.DebugInfo.BoundingObjectsColor;
			_boundingSphereDebugShape.RenderingWorld = scaleMatrix * translationMatrix;
			_boundingSphereDebugShape.Render(viewport, detailLevel);

			_boundingSphereDebugShape.RenderingWorld = rotateYMatrix * scaleMatrix * translationMatrix;
			_boundingSphereDebugShape.Render(viewport, detailLevel);

			_boundingSphereDebugShape.RenderingWorld = rotateXMatrix * scaleMatrix * translationMatrix;
			_boundingSphereDebugShape.Render(viewport, detailLevel);
		}

        public void RenderBoundingBox(BoundingBox box, Viewport viewport, DetailLevel detailLevel, Matrix world, bool topLevel = false)
        {
            var scaleMatrix = Matrix.CreateScale(box.Size);
            var translationMatrix = Matrix.CreateTranslation(box.Center) * world;

            _boundingBoxDebugShape.Color = topLevel?viewport.DebugInfo.TopLevelBoundingObjectsColor:viewport.DebugInfo.BoundingObjectsColor;
            _boundingBoxDebugShape.RenderingWorld = scaleMatrix * translationMatrix; 
            _boundingBoxDebugShape.Render(viewport, detailLevel);
        }

		public void RenderRectangle(Vector upperLeft, Vector upperRight, Vector lowerLeft, Vector lowerRight, Viewport viewport, Matrix world)
		{
			_rectangleDebugShape.RenderingWorld = world;
			_rectangleDebugShape.SetRectangle(upperLeft, upperRight, lowerLeft, lowerRight);
			_rectangleDebugShape.Color = viewport.DebugInfo.BoundingObjectsColor;
			_rectangleDebugShape.Render(viewport, DetailLevel.Full);
		}


		public void RenderRay(Vector position, Vector direction, Viewport viewport)
		{
			_rayDebugShape.Start = position;
			_rayDebugShape.Direction = direction;
			_rayDebugShape.Color = viewport.DebugInfo.BoundingObjectsColor;
			_rayDebugShape.Render(viewport, DetailLevel.Full);
		}

        public void RenderBoundingObject(IBoundingObject boundingObject, Viewport viewport, DetailLevel detailLevel, Matrix world)
        {
            if (boundingObject.IsTopLevel && viewport.DebugInfo.BoundingObjects == BoundingObjectsDebugInfo.OnlyLowLevel) return;
            if (!boundingObject.IsTopLevel && viewport.DebugInfo.BoundingObjects == BoundingObjectsDebugInfo.OnlyTopLevel) return;

            if (boundingObject.IsSphere) RenderBoundingSphere(boundingObject.BoundingSphere, viewport, detailLevel, world, boundingObject.IsTopLevel);
            if (boundingObject.IsBox) RenderBoundingBox(boundingObject.BoundingBox, viewport, detailLevel, world, boundingObject.IsTopLevel);
        }
    }
}
