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

			var rr = (int)((RScanline + subPixelX * RScanlineInterpolate) * 65535f);
			var gg = (int)((GScanline + subPixelX * GScanlineInterpolate) * 65535f);
			var bb = (int)((BScanline + subPixelX * BScanlineInterpolate) * 65535f);
			var aa = (int)((AScanline + subPixelX * AScanlineInterpolate) * 65535f);
			var rInterpolate = (int)(RScanlineInterpolate * 65535f);
			var gInterpolate = (int)(GScanlineInterpolate * 65535f);
			var bInterpolate = (int)(BScanlineInterpolate * 65535f);
			var aInterpolate = (int)(AScanlineInterpolate * 65535f);

			var x1Int = (int)X1;
			var x2Int = (int)X2;

			for (var x = x1Int; x < x2Int; x++)
			{
				if (x >= 0 && x < BufferContainer.Width)
				{
					z = 1f / zz;
					var bufferZ = (UInt32)((1.0f - z) * (float)UInt32.MaxValue);
					if (bufferZ > DepthBuffer[offset] &&
						z >= 0f &&
						z < 1f
						)
					{
						u1 = uu1 * z;
						v1 = vv1 * z;
						u2 = uu2 * z;
						v2 = vv2 * z;

						var red = (rr >> 8) & 0xff;
						var green = (gg >> 8) & 0xff;
						var blue = (bb >> 8) & 0xff;

						//var alpha = (uint)(aStart >> 8) & 0xff;

						var colorAsInt = (red << 16) |
										  (green << 8) |
										  blue;


						var intu1 = (int)(u1) & (texture1Width - 1);
						var intv1 = (int)(v1) & (texture1Height - 1);
						var intu2 = (int)(u2) & (texture2Width - 1);
						var intv2 = (int)(v2) & (texture2Height - 1);
						Framebuffer[offset] =
							Color.Additive(
							Color.Multiply(
									Color.Scale(Bilerp(Texture1, intu1, intv1, u1, v1), Texture1Factor),
									
									colorAsInt
									), 
									Color.Multiply(Color.Scale(Bilerp(Texture2,intu2, intv2, u2, v2), Texture2Factor), colorAsInt)) | Color.AlphaFull;

						DepthBuffer[offset] = bufferZ;
					}
				}

				offset++;

				zz += ZInterpolateX;
				uu1 += U1zInterpolateX;
				vv1 += V1zInterpolateX;

				uu2 += U2zInterpolateX;
				vv2 += V2zInterpolateX;

				rr += rInterpolate;
				gg += gInterpolate;
				bb += bInterpolate;
				aa += aInterpolate;

			}
		}
	}
}
#endif