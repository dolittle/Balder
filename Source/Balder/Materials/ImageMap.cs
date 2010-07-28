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
using Balder.Imaging;

namespace Balder.Materials
{
	public class ImageMap : IMap
	{
		private readonly Image _image;

		public ImageMap(Image image)
		{
			_image = image;
		}


		public int[] GetPixelsAs32BppARGB()
		{
			return _image.ImageContext.GetPixelsAs32BppARGB();
		}

		public bool HasPixelChanges
		{
			get { return false; }
		}

		public int Width
		{
			get { return _image.Width; }
		}

		public int Height
		{
			get { return _image.Height; }
		}

		public int WidthBitCount
		{
			get { return _image.WidthBitCount; }
		}

		public int HeightBitCount
		{
			get { return _image.HeightBitCount; }
		}
	}
}