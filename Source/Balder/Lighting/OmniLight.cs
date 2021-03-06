#region License
//
// Author: Petri Wilhelmsen
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

		public override int Calculate(Viewport viewport, Material material, Vector point, Vector normal, out int diffuseResult, out int specularResult)
		{
			var actualAmbient = viewport.Scene.AmbientAsInt;
			var actualDiffuse = DiffuseAsInt; 
			var actualSpecular = SpecularAsInt;

			/*
//////////////////////////////////////////////////////////////
// Example 2.6                                              //
//                                                          //
// This vertex shader simply sets the output registers so   // 
// that all lighting will be calculated per pixel.          //
//////////////////////////////////////////////////////////////
VertexShaderOutputPerPixelDiffuse PerPixelDiffuseVS(
     float3 position : POSITION,
     float3 normal : NORMAL )
{
     VertexShaderOutputPerPixelDiffuse output;

     //generate the world-view-projection matrix
     float4x4 wvp = mul(mul(world, view), projection);
     
     //transform the input position to the output
     output.Position = mul(float4(position, 1.0), wvp);

     output.WorldNormal =  mul(normal, world);
     float4 worldPosition =  mul(float4(position, 1.0), world);
     output.WorldPosition = worldPosition / worldPosition.w;

     //return the output structure
     return output;
}
			 * 
			 //////////////////////////////////////////////////////////////
			// Example 2.9                                              //
			//                                                          //
			// Diffuse + phong in the pixel shader gives the best-      //
			// looking results, but is the most GPU intensive of the    //
			// techniques in this sample, since more pixels are being   //
			// calculated than vertices for our very simple scene.      //
			//////////////////////////////////////////////////////////////
			float4 DiffuseAndPhongPS(PixelShaderInputPerPixelDiffuse input) : COLOR
			{
				 //calculate per-pixel diffuse
				 float3 directionToLight = normalize(lightPosition - input.WorldPosition);
				 float diffuseIntensity = saturate( dot(directionToLight, input.WorldNormal));
				 float4 diffuse = diffuseLightColor * diffuseIntensity;

				 //calculate Phong components per-pixel
				 float3 reflectionVector = normalize(reflect(-directionToLight, input.WorldNormal));
				 float3 directionToCamera = normalize(cameraPosition - input.WorldPosition);
     
				 //calculate specular component
				 float4 specular = specularLightColor * specularIntensity * 
								   pow(saturate(dot(reflectionVector, directionToCamera)), 
									   specularPower);
      
				 //all color components are summed in the pixel shader
				 float4 color = specular  + diffuse + ambientLightColor;
				 color.a = 1.0;
				 return color;
			}
			* 
			 * */
			// Use dotproduct for diffuse lighting. Add point functionality as this now is a directional light.
			// Ambient light
			var ambient = Color.Scale(actualAmbient, StrengthAsFloat);

			// Diffuse light
			var lightDir = _position - point;
			lightDir.Normalize();
			normal.Normalize();

			var dfDot = normal.Dot(lightDir); 
			var diffuse = Color.Scale(Color.Scale(actualDiffuse, dfDot), StrengthAsFloat);

			// Specular highlight
			var reflection = ((2f * normal) * dfDot) - lightDir;
			reflection.Normalize();
			var view = _viewPosition - point;
			view.Normalize();
			var spDot = System.Math.Max(0, reflection.Dot(view));

			var specularPower = 
				MathHelper.Saturate(
					(material.SpecularLevelAsFloat * 
					(float)System.Math.Pow(spDot, material.GlossinessAsFloat)));

			var specular = Color.Scale(Color.Scale(actualSpecular, specularPower), StrengthAsFloat);

			// Compute self shadowing
			var shadow = 4.0f*dfDot; 
			shadow = MathHelper.Saturate(shadow);

			// Compute range for the light
			var attenuation = ((lightDir / _rangeAsFloat).Dot(lightDir / _rangeAsFloat));
			attenuation = MathHelper.Saturate(attenuation);
			attenuation = 1f - attenuation;

			diffuseResult = diffuse;
			specularResult = specular;

			diffuse = Color.Multiply(diffuse, material.DiffuseAsInt);
			specular = Color.Multiply(specular, material.SpecularAsInt);

			// Final result
			diffuse = Color.Scale(diffuse, shadow);
			var color = Color.Scale(
				Color.Additive(
					Color.Additive(
						ambient, diffuse), specular)
				, attenuation);
			

			return color;
		}
	}
}
