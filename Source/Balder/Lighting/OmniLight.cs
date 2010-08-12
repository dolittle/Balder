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
using Balder.Materials;
using Balder.Math;

namespace Balder.Lighting
{
	/// <summary>
	/// Represents a non directional light that emits light in all directions
	/// </summary>
	public class OmniLight : Light
	{
		/// <summary>
		/// Gets or sets the strength of the light
		/// </summary>
		public double Strength { get; set; }

		/// <summary>
		/// Gets or sets the range of the light
		/// </summary>
		public float Range { get; set; }


		/// <summary>
		/// Creates an instance of OmniLight
		/// </summary>
		public OmniLight()
		{
			Strength = 1f;
			Range = 10.0f;
		}

		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			_position = Position;
			_viewPosition = viewport.View.Position;
			base.BeforeRendering(viewport, view, projection, world);
		}

		private Vector _position;
		private Vector _viewPosition;

		public override Color Calculate(Viewport viewport, Material material, Vector point, Vector normal)
		{
			var actualAmbient = Ambient + material.ActualAmbient;
			var actualDiffuse = Diffuse + material.ActualDiffuse;
			var actualSpecular = material.Specular;

			var strengthAsFloat = (float)Strength;

			// Use dotproduct for diffuse lighting. Add point functionality as this now is a directional light.
			// Ambient light
			var ambient = actualAmbient * strengthAsFloat;

			// Diffuse light
			var lightDir = _position - point;
			lightDir.Normalize();
			normal.Normalize();
			var dfDot = lightDir.Dot(normal);
			dfDot = MathHelper.Saturate(dfDot);
			var diffuse = actualDiffuse * dfDot * strengthAsFloat;

			// Specular highlight
			var reflection = 2f * dfDot * normal - lightDir;
			reflection.Normalize();
			var view = _viewPosition - point;
			view.Normalize();
			var spDot = reflection.Dot(view);
			spDot = MathHelper.Saturate(spDot);
			var specular = actualSpecular * spDot * strengthAsFloat;

			// Compute self shadowing
			var shadow = 4.0f * lightDir.Dot(normal);
			shadow = MathHelper.Saturate(shadow);

			// Compute range for the light
			var attenuation = ((lightDir / Range).Dot(lightDir / Range));
			attenuation = MathHelper.Saturate(attenuation);
			attenuation = 1f - attenuation;

			// Final result
			var colorVector = (ambient + (shadow * diffuse) + specular) * attenuation;

			return colorVector;
		}
	}
}
