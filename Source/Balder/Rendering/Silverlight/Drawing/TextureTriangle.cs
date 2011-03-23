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
	public class TextureTriangle : Triangle
	{
		protected override void DrawSpan(int offset)
		{
			var textureWidth = Texture1.Width;
			var textureHeight = Texture1.Height;

			float u;
			float v;
			float z;

			var subPixelX = 1f - (X1 - X1Int);
			var zz = Z1 + subPixelX * ZInterpolateX;
			var uu = U1 + subPixelX * U1zInterpolateX;
			var vv = V1 + subPixelX * V1zInterpolateX;

			for (var x = X1Int; x < X2Int; x++)
			{
				z = 1f / zz;

				var bufferZ = (UInt32)(1.0f - (z * ZMultiplier));
				if (bufferZ > DepthBuffer[offset] &&
					z >= Near &&
					z < Far
					)
				{
					u = uu * z;
					v = vv * z;

					var intu = (int)(u) & (textureWidth - 1);
					var intv = (int)(v) & (textureHeight - 1);
					Framebuffer[offset] =
						Color.Blend(Texture1.Pixels[intu, intv], MaterialDiffuseAsInt, Texture1Factor) | Color.AlphaFull;
					DepthBuffer[offset] = bufferZ;
				}

				offset++;

				zz += ZInterpolateX;
				uu += U1zInterpolateX;
				vv += V1zInterpolateX;
			}
		}
	}
}
#endif