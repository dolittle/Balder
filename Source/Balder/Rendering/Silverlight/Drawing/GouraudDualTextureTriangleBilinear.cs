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
#if(SILVERLIGHT)
using System;

namespace Balder.Rendering.Silverlight.Drawing
{
	public class GouraudDualTextureTriangleBilinear : Triangle
	{
		protected override void DrawSpan(int offset)
		{
			var texture1Width = Texture1.Width;
			var texture1Height = Texture1.Height;
			var texture2Width = Texture2.Width;
			var texture2Height = Texture2.Height;

			float u1;
			float v1;
			float u2;
			float v2;
			float z;

			var subPixelX = 1f - (X1 - (int)X1);
			var zz = Z1 + subPixelX * ZInterpolateX;
			var uu1 = U1 + subPixelX * U1zInterpolateX;
			var vv1 = V1 + subPixelX * V1zInterpolateX;

			var uu2 = U2 + subPixelX * U2zInterpolateX;
			var vv2 = V2 + subPixelX * V2zInterpolateX;

			var diffuseRr = (int)((DiffuseRScanline + subPixelX * DiffuseRScanlineInterpolate) * 65535f);
			var diffuseGg = (int)((DiffuseGScanline + subPixelX * DiffuseGScanlineInterpolate) * 65535f);
			var diffuseBb = (int)((DiffuseBScanline + subPixelX * DiffuseBScanlineInterpolate) * 65535f);
			var diffuseAa = (int)((DiffuseAScanline + subPixelX * DiffuseAScanlineInterpolate) * 65535f);
			var diffuseRInterpolate = (int)(DiffuseRScanlineInterpolate * 65535f);
			var diffuseGInterpolate = (int)(DiffuseGScanlineInterpolate * 65535f);
			var diffuseBInterpolate = (int)(DiffuseBScanlineInterpolate * 65535f);
			var diffuseAInterpolate = (int)(DiffuseAScanlineInterpolate * 65535f);

			var specularRr = (int)((SpecularRScanline + subPixelX * SpecularRScanlineInterpolate) * 65535f);
			var specularGg = (int)((SpecularGScanline + subPixelX * SpecularGScanlineInterpolate) * 65535f);
			var specularBb = (int)((SpecularBScanline + subPixelX * SpecularBScanlineInterpolate) * 65535f);
			var specularAa = (int)((SpecularAScanline + subPixelX * SpecularAScanlineInterpolate) * 65535f);
			var specularRInterpolate = (int)(SpecularRScanlineInterpolate * 65535f);
			var specularGInterpolate = (int)(SpecularGScanlineInterpolate * 65535f);
			var specularBInterpolate = (int)(SpecularBScanlineInterpolate * 65535f);
			var specularAInterpolate = (int)(SpecularAScanlineInterpolate * 65535f);

			var x1Int = (int)X1;
			var x2Int = (int)X2;

			for (var x = x1Int; x < x2Int; x++)
			{
				if (x >= 0 && x < BufferContainer.Width)
				{
					z = 1f / zz;
					var bufferZ = (UInt32)(1.0f - (z*ZMultiplier));
					if (bufferZ > DepthBuffer[offset] &&
						z >= Near &&
						z < Far
						)
					{
						u1 = uu1 * z;
						v1 = vv1 * z;
						u2 = uu2 * z;
						v2 = vv2 * z;

						var diffuseRed = (diffuseRr >> 8) & 0xff;
						var diffuseGreen = (diffuseGg >> 8) & 0xff;
						var diffuseBlue = (diffuseBb >> 8) & 0xff;

						var specularRed = (specularRr >> 8) & 0xff;
						var specularGreen = (specularGg >> 8) & 0xff;
						var specularBlue = (specularBb >> 8) & 0xff;

						var diffuse = (diffuseRed << 16) | (diffuseGreen << 8) | diffuseBlue;
						var specular = (specularRed << 16) | (specularGreen << 8) | specularBlue;

						var intu1 = (int)(u1) & (texture1Width - 1);
						var intv1 = (int)(v1) & (texture1Height - 1);
						var intu2 = (int)(u2) & (texture2Width - 1);
						var intv2 = (int)(v2) & (texture2Height - 1);
						Framebuffer[offset] =
							Color.Additive(
								Color.Additive(
									Color.Multiply(
										Color.Blend(Bilerp(Texture1, intu1, intv1, u1, v1), MaterialDiffuseAsInt, Texture1Factor),
										diffuse),
									specular
								),
									Color.Multiply(
										Color.Scale(Bilerp(Texture2, intu2, intv2, u2, v2), Texture2Factor),
										diffuse))
									| Color.AlphaFull;

						DepthBuffer[offset] = bufferZ;
					}
				}

				offset++;

				zz += ZInterpolateX;
				uu1 += U1zInterpolateX;
				vv1 += V1zInterpolateX;

				uu2 += U2zInterpolateX;
				vv2 += V2zInterpolateX;

				diffuseRr += diffuseRInterpolate;
				diffuseGg += diffuseGInterpolate;
				diffuseBb += diffuseBInterpolate;
				diffuseAa += diffuseAInterpolate;

				specularRr += specularRInterpolate;
				specularGg += specularGInterpolate;
				specularBb += specularBInterpolate;
				specularAa += specularAInterpolate;

			}
		}
	}
}
#endif