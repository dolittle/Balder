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
using Balder.Core.Materials;
using Balder.Core.Math;
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif

namespace Balder.Core.Lighting
{
	/// <summary>
	/// Represents a directional light that has no position in 3D space
	/// </summary>
	public class DirectionalLight : Light
	{
#if(DEFAULT_CONSTRUCTOR)
		public DirectionalLight()
			: this(Runtime.Instance.Kernel.Get<IIdentityManager>())
		{
			
		}
#endif

		public DirectionalLight(IIdentityManager identityManager)
			: base(identityManager)
		{
			SpecularIntensity = 1d;
		}


		/// <summary>
		/// Gets or sets the specular intensity
		/// </summary>
		public double SpecularIntensity
		{
			get; set;
		}

		/// <summary>
		/// Gets or sets the specular power
		/// </summary>
		public double SpecularPower
		{
			get; set;
		}

		/// <summary>
		/// Direction Property
		/// </summary>
		public static readonly Property<DirectionalLight, Coordinate> DirectionProperty =
			Property<DirectionalLight, Coordinate>.Register(l => l.Direction);
		
		/// <summary>
		/// Gets or sets the direction of the light
		/// </summary>
		public Coordinate Direction
		{
			get { return DirectionProperty.GetValue(this); }
			set
			{
				DirectionProperty.SetValue(this, value);
			}
		}

		public override Color Calculate(Viewport viewport, Material material, Vector point, Vector normal)
		{
			var actualDiffuse = Diffuse;

			var lightVector = -(Vector) Direction;
			lightVector.Normalize();

			var ndl = System.Math.Max(0, normal.Dot(lightVector));

			var diffuseLight = ndl*actualDiffuse;

			var reflectionVector = Vector.Reflect(lightVector, normal);
			reflectionVector.Normalize();

			var viewDirection = Vector.Transform(Vector.Forward, viewport.View.ViewMatrix);
			viewDirection.Normalize();

			var specular = SpecularIntensity * (float)System.Math.Pow(MathHelper.Saturate(reflectionVector.Dot(viewDirection)), SpecularPower);

			var color = diffuseLight*(float)specular;
			return color;
		}
	}
}
