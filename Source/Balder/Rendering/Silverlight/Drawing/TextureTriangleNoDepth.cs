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
namespace Balder.Rendering.Silverlight.Drawing
{
	public class TextureTriangleNoDepth : Triangle
	{
		protected override void DrawSpan(int offset)
		{
			var textureWidth = Texture.Width;
			var textureHeight = Texture.Height;

			float u;
			float v;
			float z;

			var subPixelX = 1f - (X1 - (int)X1);
			var zz = Z1 + subPixelX * ZInterpolationX;
			var uu = U1 + subPixelX * UzInerpolationX;
			var vv = V1 + subPixelX * VzInterpolationX;

			var x1Int = (int)X1;
			var x2Int = (int)X2;

			for (var x = x1Int; x < x2Int; x++)
			{
				if (x >= 0 && x < BufferContainer.Width)
				{
					z = 1f / zz;
					u = uu * z;
					v = vv * z;

					var intu = (int)(u) & (textureWidth - 1);
					var intv = (int)(v) & (textureHeight - 1);
					Framebuffer[offset] = Texture.Pixels[intu, intv] | AlphaFull;
				}

				offset++;

				zz += ZInterpolationX;
				uu += UzInerpolationX;
				vv += VzInterpolationX;
			}
		}

	}
}
#endif