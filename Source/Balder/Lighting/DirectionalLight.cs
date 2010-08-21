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

using Balder.Display;
using Balder.Execution;
using Balder.Materials;
using Balder.Math;

namespace Balder.Lighting
{
	/// <summary>
	/// Represents a directional light that has no position in 3D space
	/// </summary>
	public class DirectionalLight : Light
	{
		private Vector _direction;

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
				_direction = value;
				_direction.Normalize();

				//_direction = -_direction;

			}
		}

		public override int Calculate(Viewport viewport, Material material, Vector point, Vector normal, out int diffuseResult, out int specularResult)
		{
			diffuseResult = 0;
			specularResult = 0;
			var actualAmbient = Color.Multiply(AmbientAsInt, material.AmbientAsInt);
			var actualDiffuse = Color.Multiply(DiffuseAsInt, material.DiffuseAsInt);
			var actualSpecular = material.SpecularAsInt;

			var ambient = Color.Scale(actualAmbient, StrengthAsFloat);

					
			var dfDot = System.Math.Max(0, -_direction.Dot(normal));
			var diffuse = Color.Scale(Color.Scale(actualDiffuse, dfDot), StrengthAsFloat);

			var reflectionVector = Vector.Reflect(_direction, normal);
			reflectionVector.Normalize();

			var viewDirection = Vector.Transform(Vector.Forward, viewport.View.ViewMatrix);
			viewDirection.Normalize();

			var shadow = 4.0f*dfDot; 
			shadow = MathHelper.Saturate(shadow);

			var specularPower = MathHelper.Saturate(material.SpecularLevelAsFloat* 
				(float)System.Math.Pow(
					MathHelper.Saturate(reflectionVector.Dot(viewDirection)), 
						material.GlossinessAsFloat));
			var specular = Color.Scale(Color.Scale(actualSpecular, specularPower), StrengthAsFloat);

			diffuse = Color.Scale(diffuse, shadow);
			var color = 
				Color.Additive(
					Color.Additive(ambient, 
						diffuse), specular);
			return color;
		}
	}
}
