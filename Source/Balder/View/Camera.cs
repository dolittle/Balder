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
#if(XAML)
using System.Windows;
#endif
using Balder.Rendering;
using Vector = Balder.Math.Vector;

namespace Balder.View
{
#if(XAML)
	public class Camera : FrameworkElement,
#else
	public class Camera :
#endif
 IView, IHaveRuntimeContext, IHavePropertyContainer
	{
		public const float DefaultFieldOfView = 45f;
		public const float DefaultFar = 4000f;
		public const float DefaultNear = 1.0f;

		int _viewportWidth;
		int _viewportHeight;

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

			Frustum = new Frustum();
		}

		IPropertyContainer _propertyContainer;
		public IPropertyContainer PropertyContainer
		{
			get
			{
				if (_propertyContainer == null)
					_propertyContainer = new PropertyContainer(this);

				return _propertyContainer;
			}
		}

		public Frustum Frustum { get; protected set; }

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

		public virtual Ray GetPickRay(int x, int y)
		{
			var view = ViewMatrix;
			var world = Matrix.Identity;
			var projection = ProjectionMatrix;

			var v = new Vector
			{
				X = (((2.0f * x) / _viewportWidth) - 1) / (projection.M11),
				Y = -(((2.0f * y) / _viewportHeight) - 1) / (projection.M22),
				Z = 1f
			};


			var inverseView = Matrix.Invert(view);

			var ray = new Ray
			{
				Direction = new Vector
				{
					X = (v.X * inverseView.M11) + (v.Y * inverseView.M21) + (v.Z * inverseView.M31),
					Y = (v.X * inverseView.M12) + (v.Y * inverseView.M22) + (v.Z * inverseView.M32),
					Z = (v.X * inverseView.M13) + (v.Y * inverseView.M23) + (v.Z * inverseView.M33),
					W = 1f
				}
			};
			ray.Direction.Normalize();

			ray.Position = new Vector
			{
				X = inverseView.M41,
				Y = inverseView.M42,
				Z = inverseView.M43,
				W = 1f
			};

			ray.Position += (ray.Direction * Near);

			return ray;
		}

		public Vector Unproject(Vector source, Matrix world)
		{
			return Unproject(source, ProjectionMatrix, ViewMatrix, world);
		}

		public Vector Unproject(Vector source, Matrix projection, Matrix view, Matrix world)
		{
			var combinedMatrix = (world * view) * projection;
			var matrix = Matrix.Invert(combinedMatrix);

			source.X = ((source.X / ((float)_viewportWidth)) * 2f) - 1f;
			source.Y = -(((source.Y / ((float)_viewportHeight)) * 2f) - 1f);
			source.Z = (source.Z - Near) / (Far - Near);
			source.W = 1f;
			var vector = Vector.Transform(source, matrix);

			var a = (source.X * matrix.M14) +
						(source.Y * matrix.M24) +
						(source.Z * matrix.M34) +
						(matrix.M44);

			if (!MathHelper.WithinEpsilon(a, 1f))
			{
				vector = vector / (a);
			}

			return vector;

		}

		public void Update(Viewport viewport)
		{
			_viewportWidth = viewport.Width;
			_viewportHeight = viewport.Height;

			ViewMatrix = Matrix.CreateLookAt(Position, Target, Up);
			SetupProjection(viewport);
			UpdateDepthDivisor();
			Frustum.SetCameraDefinition(viewport, this);
		}


		public bool IsInView(Vector vector)
		{
			var inFrustum = Frustum.Intersects(vector);
			return inFrustum == FrustumIntersection.Inside ||
				   inFrustum == FrustumIntersection.Intersect;
		}

		public bool IsInView(Coordinate coordinate)
		{
			var inFrustum = Frustum.Intersects(coordinate);
			return inFrustum == FrustumIntersection.Inside ||
				   inFrustum == FrustumIntersection.Intersect;
		}

        public bool IsInView(IBoundingObject boundingObject)
        {
            var inFrustum = Frustum.Intersects(boundingObject);
			return inFrustum == FrustumIntersection.Inside ||
				   inFrustum == FrustumIntersection.Intersect;
        }
		#endregion

		public IRuntimeContext RuntimeContext { get; internal set; }
	}
}