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
using Balder.Materials;

namespace Balder.Rendering.Silverlight
{
	public class TextureMipMapLevel
	{
		public int[] OriginalPixels;
		public int[,] Pixels;

		public byte[,] RedComponents;
		public byte[,] GreenComponents;
		public byte[,] BlueComponents;
		public byte[,] AlphaComponents;

		public int[,] RedComponentsAdjustedForPixel;
		public int[,] GreenComponentsAdjustedForPixel;
		public int[,] BlueComponentsAdjustedForPixel;
		public int[,] AlphaComponentsAdjustedForPixel;

		public int Width;
		public int Height;

		public int WidthBitCount;
		public int HeightBitCount;

		public bool IsSemiTransparent;

		public void SetFromMap(IMap map)
		{
			Width = map.Width;
			Height = map.Height;
			WidthBitCount = map.WidthBitCount;
			HeightBitCount = map.HeightBitCount;

			OriginalPixels = map.GetPixelsAs32BppARGB();

			Pixels = new int[Width, Height];
			RedComponents = new byte[Width, Height];
			GreenComponents = new byte[Width, Height];
			BlueComponents = new byte[Width, Height];
			AlphaComponents = new byte[Width, Height];
			GreenComponents = new byte[Width, Height];
			RedComponentsAdjustedForPixel = new int[Width, Height];
			GreenComponentsAdjustedForPixel = new int[Width, Height];
			BlueComponentsAdjustedForPixel = new int[Width, Height];
			AlphaComponentsAdjustedForPixel = new int[Width, Height];

			IsSemiTransparent = false;
			for (var pixelIndex = 0; pixelIndex < OriginalPixels.Length; pixelIndex++)
			{
				var x = (pixelIndex & map.Width - 1);
				var y = (pixelIndex >> map.HeightBitCount) & map.Height - 1;

				var pixel = OriginalPixels[pixelIndex];

				var alpha = ((pixel >> 24) & 0xff);
				var red = ((pixel >> 16) & 0xff);
				var green = ((pixel >> 8) & 0xff);
				var blue = ((pixel) & 0xff);

				Pixels[x, y] = pixel;
				RedComponents[x, y] = (byte)red;
				GreenComponents[x, y] = (byte)green;
				BlueComponents[x, y] = (byte)blue;
				AlphaComponents[x, y] = (byte)alpha;

				RedComponentsAdjustedForPixel[x, y] = (int)(((uint)pixel) & 0x00ff0000);
				GreenComponentsAdjustedForPixel[x, y] = (int)(((uint)pixel) & 0x0000ff00);
				BlueComponentsAdjustedForPixel[x, y] = (int)(((uint)pixel) & 0x000000ff);
				AlphaComponentsAdjustedForPixel[x, y] = (int)(((uint)pixel) & 0xff000000);
			}
		}
	}
}