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
		private float _strengthAsFloat;
		private Vector _direction;

		public DirectionalLight()
		{
			Strength = 1;
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
				_direction = value;
				_direction.Normalize();
			}
		}

		/// <summary>
		/// Gets or sets the strength of the light
		/// </summary>
		public double Strength { get; set; }

		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			_strengthAsFloat = (float)Strength;
			base.BeforeRendering(viewport, view, projection, world);
		}

		public override int Calculate(Viewport viewport, Material material, Vector point, Vector normal)
		{
			var actualAmbient = Color.Multiply(AmbientAsInt, material.AmbientAsInt);
			var actualDiffuse = Color.Multiply(DiffuseAsInt, material.DiffuseAsInt);
			var actualSpecular = material.SpecularAsInt;

			var ambient = Color.Scale(actualAmbient, _strengthAsFloat);

			var dfDot = System.Math.Max(0, _direction.Dot(normal));
			var diffuse = Color.Scale(Color.Scale(actualDiffuse, dfDot), _strengthAsFloat);

			var reflectionVector = Vector.Reflect(_direction, normal);
			reflectionVector.Normalize();

			var viewDirection = Vector.Transform(Vector.Forward, viewport.View.ViewMatrix);
			viewDirection.Normalize();

			var shadow = 4.0f * _direction.Dot(normal);
			shadow = MathHelper.Saturate(shadow);

			var specularPower = material.ShineStrengthAsFloat* 
				(float)System.Math.Pow(
					MathHelper.Saturate(reflectionVector.Dot(viewDirection)), 
						material.ShineAsFloat);
			var specular = Color.Scale(Color.Scale(actualSpecular, specularPower), _strengthAsFloat);


			var color = 
				Color.Additive(
					Color.Additive(ambient, 
						Color.Scale(diffuse,shadow)), specular);
			return color;
		}
	}
}
