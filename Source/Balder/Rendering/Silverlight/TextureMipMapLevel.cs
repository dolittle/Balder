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
using System.Windows.Media.Imaging;
using Balder.Materials;

namespace Balder.Rendering.Silverlight
{


	public class TextureMipMapLevel
	{
		public int[,] Pixels;
		public WriteableBitmap WriteableBitmap;

		public int Width;
		public int Height;

		public int WidthBitCount;
		public int HeightBitCount;

		public bool IsSemiTransparent;

		private bool IsWidthPowerOfTwo;
		private bool IsHeightPowerOfTwo;


		private bool IsPowerOfTwo(int dimension, out int widthBitCount)
		{
			var log = (float) (System.Math.Log(dimension) / System.Math.Log(2));
			var logAsInt = (int)log;
			var logDiff = log - logAsInt;
			widthBitCount = (int) log;
			return logDiff == 0;
		}

		private void InitializeWidth()
		{
			IsWidthPowerOfTwo = IsPowerOfTwo(Width, out WidthBitCount);
		}

		private void InitializeHeight()
		{
			IsHeightPowerOfTwo = IsPowerOfTwo(Height, out HeightBitCount);
		}


		public void SetFromMap(IMap map)
		{
			Width = map.Width;
			Height = map.Height;
			InitializeWidth();
			InitializeHeight();

			if( !IsWidthPowerOfTwo)
			{
				throw new ArgumentException("Width of texture is not power of two (2,4,8,16,32,64,128,256,512)");
			}

			if (!IsHeightPowerOfTwo)
			{
				throw new ArgumentException("Height of texture is not power of two (2,4,8,16,32,64,128,256,512)");
			}

			var originalPixels = map.GetPixelsAs32BppARGB();
			PrepareWriteableBitmap(originalPixels);
			PreparePixels(originalPixels);
		}

		private void PreparePixels(int[] originalPixels)
		{
			Pixels = new int[Width, Height];

			IsSemiTransparent = false;
			for (var pixelIndex = 0; pixelIndex < originalPixels.Length; pixelIndex++)
			{
				var x = (pixelIndex & Width - 1);
				var y = (pixelIndex >> WidthBitCount) & Height - 1;

				var pixel = originalPixels[pixelIndex];
				Pixels[x, y] = pixel;
			}
		}

		private void PrepareWriteableBitmap(int[] pixels)
		{
			WriteableBitmap = new WriteableBitmap(Width,Height);
			Buffer.BlockCopy(pixels,0,WriteableBitmap.Pixels,0,pixels.Length*4);
		}
	}
}
#endif