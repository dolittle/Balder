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

using System;
using Balder.Materials;

namespace Balder.Rendering.Silverlight
{
	public class TextureMipMapLevel
	{
		public int[,] Pixels;

		public int Width;
		public int Height;

		public int WidthBitCount;
		public int HeightBitCount;

		public bool IsSemiTransparent;

		private bool IsWidthPowerOfTwo;
		private bool IsHeightPowerOfTwo;

		private void InitializeWidth()
		{
			var log = System.Math.Log(Width) / System.Math.Log(2);

			var logAsInt = (int)log;
			var logDiff = log - (double)logAsInt;
			if (logDiff == 0)
			{
				IsWidthPowerOfTwo = true;
				WidthBitCount = (int)log;
			}
		}

		private void InitializeHeight()
		{
			var log = System.Math.Log(Height) / System.Math.Log(2);

			var logAsInt = (int)log;
			var logDiff = log - (double)logAsInt;
			if (logDiff == 0)
			{
				IsHeightPowerOfTwo = true;
				HeightBitCount = (int)log;
			}
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

			Pixels = new int[Width, Height];

			IsSemiTransparent = false;
			for (var pixelIndex = 0; pixelIndex < originalPixels.Length; pixelIndex++)
			{
				var x = (pixelIndex & map.Width - 1);
				var y = (pixelIndex >> WidthBitCount) & map.Height - 1;

				var pixel = originalPixels[pixelIndex];
				Pixels[x, y] = pixel;
			}
		}
	}
}