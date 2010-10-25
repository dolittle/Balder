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
using Balder.Display;
using Balder.Execution;
using Balder.Math;
#if(SILVERLIGHT)
using System.Windows;
#endif
using Balder.Rendering;


namespace Balder.View
{
#if(SILVERLIGHT)
	public class Camera : FrameworkElement,
#else
	public class Camera :
#endif
 IView, IHaveRuntimeContext
	{
		public const float DefaultFieldOfView = 45f;
		public const float DefaultFar = 4000f;
		public const float DefaultNear = 1.0f;

		private readonly Frustum _frustum;

		public Camera()
		{
			Position = Vector.Zero;
			Target = Vector.Forward;
			Up = Vector.Up;
			Near = DefaultNear;
			Far = DefaultFar;
			FieldOfView = DefaultFieldOfView;
			ProjectionMatrix = Matrix.Identity;
			UpdateDepthDivisor();
			ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);

			_frustum = new Frustum();
		}


		public virtual Matrix ViewMatrix { get; protected set; }
		public virtual Matrix ProjectionMatrix { get; protected set; }

		public static readonly Property<Camera, Coordinate> PositionProp = Property<Camera, Coordinate>.Register(c => c.Position);
		public Coordinate Position
		{
			get { return PositionProp.GetValue(this); }
			set { PositionProp.SetValue(this, value); }
		}


		public static readonly Property<Camera, Coordinate> TargetProp = Property<Camera, Coordinate>.Register(c => c.Target);
		public Coordinate Target
		{
			get { return TargetProp.GetValue(this); }
			set { TargetProp.SetValue(this, value); }
		}

		public Vector Up { get; set; }
		public Vector Forward
		{
			get { return Target - Position; }
		}
		public float Near { get; set; }
		public float Far { get; set; }

		public static readonly Property<Camera, double> FieldOfViewProperty =
			Property<Camera, double>.Register(c => c.FieldOfView);
		public double FieldOfView
		{
			get { return FieldOfViewProperty.GetValue(this); }
			set { FieldOfViewProperty.SetValue(this, value); }
		}

		public float DepthZero { get; private set; }
		public float DepthDivisor { get; private set; }
		public float DepthMultiplier { get; private set; }


		#region Private Methods

		/// <summary>
		/// Calculates the projection matrix
		/// </summary>
		protected virtual void SetupProjection(Viewport viewport)
		{
			var projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
				MathHelper.ToRadians((float)FieldOfView),
				viewport.AspectRatio,
				Near,
				Far);
			//var screenTranslationMatrix = Matrix.CreateScreenTranslation(viewport.Width, viewport.Height);
			ProjectionMatrix = projectionMatrix; // *screenTranslationMatrix;
		}

		private void UpdateDepthDivisor()
		{
			DepthDivisor = (Far - Near);
			DepthMultiplier = 1f / DepthDivisor;
			DepthZero = Near / DepthDivisor;
		}
		#endregion

		#region Public Methods

		public void Update(Viewport viewport)
		{
			ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
			SetupProjection(viewport);
			UpdateDepthDivisor();
			_frustum.SetCameraDefinition(viewport, this);
		}


		public bool IsInView(Vector vector)
		{
			var inFrustum = _frustum.IsPointInFrustum(vector);
			return inFrustum == FrustumIntersection.Inside ||
				   inFrustum == FrustumIntersection.Intersect;
		}

		public bool IsInView(Coordinate coordinate)
		{
			var inFrustum = _frustum.IsPointInFrustum(coordinate);
			return inFrustum == FrustumIntersection.Inside ||
				   inFrustum == FrustumIntersection.Intersect;
		}

		public bool IsInView(BoundingSphere boundingSphere)
		{
			var inFrustum = _frustum.IsSphereInFrustum(boundingSphere.Center, boundingSphere.Radius);
			return inFrustum == FrustumIntersection.Inside ||
				   inFrustum == FrustumIntersection.Intersect;
		}

		public bool IsInView(Vector position, float radius)
		{
			var inFrustum = _frustum.IsSphereInFrustum(position, radius);
			return inFrustum == FrustumIntersection.Inside ||
				   inFrustum == FrustumIntersection.Intersect;
		}

		#endregion

		public IRuntimeContext RuntimeContext { get; internal set; }
	}
}