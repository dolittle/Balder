#region License
//
// Author: Petri Wilhelmsen
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
	/// Represents a non directional light that emits light in all directions
	/// </summary>
	public class OmniLight : Light
	{
		public static readonly Property<OmniLight, double> RangeProperty =
			Property<OmniLight, double>.Register(l => l.Range);

		/// <summary>
		/// Gets or sets the range of the light
		/// </summary>
		public double Range
		{
			get { return RangeProperty.GetValue(this); }
			set
			{
				RangeProperty.SetValue(this,value);
				_rangeAsFloat = (float) value;
			}
		}

		/// <summary>
		/// Creates an instance of OmniLight
		/// </summary>
		public OmniLight()
		{
			Range = 10;
		}

		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			_position = Position;
			_viewPosition = viewport.View.Position;
			base.BeforeRendering(viewport, view, projection, world);
		}

		private Vector _position;
		private Vector _viewPosition;
		private float _rangeAsFloat;

		public override int Calculate(Viewport viewport, Material material, Vector point, Vector normal)
		{
			var actualAmbient = Color.Multiply(AmbientAsInt, material.AmbientAsInt);
			var actualDiffuse = Color.Multiply(DiffuseAsInt, material.DiffuseAsInt);
			var actualSpecular = material.SpecularAsInt;


			// Use dotproduct for diffuse lighting. Add point functionality as this now is a directional light.
			// Ambient light
			var ambient = Color.Scale(actualAmbient, StrengthAsFloat);

			// Diffuse light
			var lightDir = _position - point;
			lightDir.Normalize();
			normal.Normalize();

			var dfDot = System.Math.Max(0, lightDir.Dot(normal));
			var diffuse = Color.Scale(Color.Scale(actualDiffuse, dfDot), StrengthAsFloat);

			// Specular highlight
			var reflection = 2f * dfDot * normal - lightDir;
			reflection.Normalize();
			var view = _viewPosition - point;
			view.Normalize();
			var spDot = reflection.Dot(view);
			spDot = MathHelper.Saturate(spDot);

			var specularPower = material.ShineStrengthAsFloat * (float)System.Math.Pow(spDot, material.ShineAsFloat);
			var specular = Color.Scale(Color.Scale(actualSpecular, specularPower), StrengthAsFloat);


			// Compute self shadowing
			var shadow = 4.0f * lightDir.Dot(normal);
			shadow = MathHelper.Saturate(shadow);

			// Compute range for the light
			var attenuation = ((lightDir / _rangeAsFloat).Dot(lightDir / _rangeAsFloat));
			attenuation = MathHelper.Saturate(attenuation);
			attenuation = 1f - attenuation;

			// Final result

			var color = Color.Scale(
				Color.Additive(
					Color.Additive(
						ambient, Color.Scale(diffuse, shadow)), specular)
				, attenuation);

			return color;
		}
	}
}
