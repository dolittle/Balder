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

using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;
using Balder.Core.View;
#if(SILVERLIGHT)
using Ninject;
#endif

namespace Balder.Core.Lighting
{
	public class ViewLight : DirectionalLight
	{
		private Vector _direction;
		private Vector _viewDirection;
		private Vector _lightVector;
		private ColorAsFloats _actualDiffuse;
		private Matrix _angleMatrix;

#if(SILVERLIGHT)
		public ViewLight()
			: this(Runtime.Instance.Kernel.Get<IIdentityManager>())
		{
			
		}
#endif

		public ViewLight(IIdentityManager identityManager)
			: base(identityManager)
		{
			PrepareAngleMatrix();
		}


		public static readonly Property<ViewLight, double> XAngleOffsetProperty =
			Property<ViewLight, double>.Register(c => c.XAngleOffset);
		public double XAngleOffset
		{
			get { return XAngleOffsetProperty.GetValue(this); }
			set
			{
				XAngleOffsetProperty.SetValue(this, value);
				PrepareAngleMatrix();
			}
		}

		public static readonly Property<ViewLight, double> YAngleOffsetProperty =
			Property<ViewLight, double>.Register(c => c.YAngleOffset);
		public double YAngleOffset
		{
			get { return YAngleOffsetProperty.GetValue(this); }
			set
			{
				YAngleOffsetProperty.SetValue(this, value);
				PrepareAngleMatrix();
			}
		}

		public static readonly Property<ViewLight, double> ZAngleOffsetProperty =
			Property<ViewLight, double>.Register(c => c.ZAngleOffset);
		public double ZAngleOffset
		{
			get { return ZAngleOffsetProperty.GetValue(this); }
			set
			{
				ZAngleOffsetProperty.SetValue(this, value);
				PrepareAngleMatrix();
			}
		}


		private void PrepareAngleMatrix()
		{
			_angleMatrix = Matrix.CreateRotation((float)XAngleOffset, (float)YAngleOffset, (float)ZAngleOffset);
		}


		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			var combinedMatrix = viewport.View.ViewMatrix * _angleMatrix;

			_direction = Vector.Transform(Vector.Forward, combinedMatrix);
			_direction.Normalize();

			_lightVector = -(Vector)_direction;
			_lightVector.Normalize();

			_viewDirection = Vector.Transform(Vector.Forward, viewport.View.ViewMatrix);
			_viewDirection.Normalize();

			_actualDiffuse = Diffuse.ToColorAsFloats();

			base.BeforeRendering(viewport, view, projection, world);
		}

		public override ColorAsFloats Calculate(Viewport viewport, Vector point, Vector normal)
		{
			var camera = viewport.View as Camera;
			if (null != camera)
			{
				var ndl = System.Math.Max(0, normal.Dot(_lightVector));
				var diffuseLight = ndl * _actualDiffuse;

				var reflectionVector = Vector.Reflect(_lightVector, normal);
				reflectionVector.Normalize();

				var specular = SpecularIntensity * (float)System.Math.Pow(MathHelper.Saturate(reflectionVector.Dot(_viewDirection)), SpecularPower);

				var color = diffuseLight * (float)specular;
				return color;
			}
			return new ColorAsFloats();
		}
	}
}
