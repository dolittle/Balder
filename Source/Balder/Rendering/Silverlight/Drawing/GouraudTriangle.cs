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
	public class GouraudTriangle : Triangle
	{
		protected override void DrawSpan(int offset)
		{
			float z;

			var subPixelX = 1f - (X1 - (int)X1);
			var zz = Z1 + subPixelX * ZInterpolateX;

			var rr = (int)((R1 /*+ subPixelX * RInterpolate1*/) * 255f)<<8;
			var gg = (int)((G1 /*+ subPixelX * GInterpolate1*/) * 255f)<<8;
			var bb = (int)((B1 /*+ subPixelX * BInterpolate1*/) * 255f)<<8;
			var aa = (int)((A1 /*+ subPixelX * AInterpolate1*/) * 255f)<<8;

			var rInterpolate = (int)(RInterpolate1 * 255f);
			var gInterpolate = (int)(GInterpolate1 * 255f);
			var bInterpolate = (int)(BInterpolate1 * 255f);
			var aInterpolate = (int)(AInterpolate1 * 255f);


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
						var red = (rr >> 8) & 0xff;
						var green = (gg >> 8) & 0xff;
						var blue = (bb >> 8) & 0xff;
						//var alpha = (uint)(aStart >> 8) & 0xff;

						var colorAsInt = AlphaFull |
										  (red << 16) |
										  (green << 8) |
										  blue;

						Framebuffer[offset] = colorAsInt;
						DepthBuffer[offset] = bufferZ;
					}
				}

				offset++;
				zz += ZInterpolateX;
				rr += rInterpolate;
				gg += gInterpolate;
				bb += bInterpolate;
				aa += aInterpolate;
			}
		}

		protected virtual void DrawSpan(
			int length,
			float zStart,
			float zAdd,
			int rStart,
			int rAdd,
			int gStart,
			int gAdd,
			int bStart,
			int bAdd,
			int aStart,
			int aAdd,
			uint[] depthBuffer,
			int offset,
			int[] framebuffer)
		{

			for (var x = 0; x <= length; x++)
			{
				var bufferZ = (UInt32)((1.0f - zStart) * (float)UInt32.MaxValue);
				if (bufferZ > depthBuffer[offset] &&
					zStart >= 0f &&
					zStart < 1f
					)
				{
					var red = (uint)(rStart >> 8) & 0xff;
					var green = (uint)(gStart >> 8) & 0xff;
					var blue = (uint)(bStart >> 8) & 0xff;
					//var alpha = (uint)(aStart >> 8) & 0xff;

					uint colorAsInt = 0xff000000 |
									  (red << 16) |
									  (green << 8) |
									  blue;



					framebuffer[offset] = (int)colorAsInt;
					depthBuffer[offset] = bufferZ;
				}

				offset++;
				zStart += zAdd;
				rStart += rAdd;
				gStart += gAdd;
				bStart += bAdd;
				aStart += aAdd;
			}
		}
	}
}
#endif