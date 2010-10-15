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
#if(SILVERLIGHT)
using System;

namespace Balder.Rendering.Silverlight.Drawing
{
	public class GouraudTextureTriangle : Triangle
	{
		protected override void DrawSpan(int offset)
		{
			var textureWidth = Texture1.Width;
			var textureHeight = Texture1.Height;

			float u;
			float v;
			float z;

			var subPixelX = 1f - (X1 - (int)X1);
			var zz = Z1 + subPixelX * ZInterpolateX;
			var uu = U1 + subPixelX * U1zInterpolateX;
			var vv = V1 + subPixelX * V1zInterpolateX;

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
						u = uu * z;
						v = vv * z;


						var diffuseRed = (diffuseRr >> 8) & 0xff;
						var diffuseGreen = (diffuseGg >> 8) & 0xff;
						var diffuseBlue = (diffuseBb >> 8) & 0xff;

						var specularRed = (specularRr >> 8) & 0xff;
						var specularGreen = (specularGg >> 8) & 0xff;
						var specularBlue = (specularBb >> 8) & 0xff;

						var diffuse = (diffuseRed << 16) | (diffuseGreen << 8) | diffuseBlue;
						var specular = (specularRed << 16) | (specularGreen << 8) | specularBlue;


						var intu = (int)(u) & (textureWidth - 1);
						var intv = (int)(v) & (textureHeight - 1);
						Framebuffer[offset] = 
							Color.Additive(Color.Multiply(
								Color.Blend(Texture1.Pixels[intu, intv], MaterialDiffuseAsInt, Texture1Factor), diffuse), specular) |
						                      Color.AlphaFull;
						DepthBuffer[offset] = bufferZ;
					}
				}

				offset++;

				zz += ZInterpolateX;
				uu += U1zInterpolateX;
				vv += V1zInterpolateX;

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