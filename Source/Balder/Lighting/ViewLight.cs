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

namespace Balder.Lighting
{
	public class ViewLight : DirectionalLight
	{
		private Matrix _angleMatrix;


		public ViewLight()
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
			var transformed = Vector.Transform(Vector.Forward, combinedMatrix);
			Direction.Set(transformed);

			base.BeforeRendering(viewport, view, projection, world);
		}
	
	}
}
