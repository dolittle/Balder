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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Balder.Core.Assets;
using Balder.Core.Silverlight.TypeConverters;
using Ninject.Core;

namespace Balder.Core.Imaging
{
#if(SILVERLIGHT)
	[TypeConverter(typeof(UriToImageTypeConverter))]
#endif
	public class Image : IAsset, IAssetPart
	{
		[Inject]
		public IImageContext ImageContext { get; set; }

		private int _width;
		public int Width
		{
			get { return _width; }
			set
			{
				_width = value;
				InitializeWidth();
			}
		}

		private int _height;
		public int Height
		{
			get { return _height; }
			set
			{
				_height = value;
				InitializeHeight();
			}
		}

		public bool IsWidthPowerOfTwo { get; private set; }
		public bool IsHeightPowerOfTwo { get; private set; }

		public int WidthBitCount { get; private set; }
		public int HeightBitCount { get; private set; }

		private void InitializeWidth()
		{
			var log = System.Math.Log(Width) / System.Math.Log(2);

			var logAsInt = (int) log;
			var logDiff = log - (double) logAsInt;
			if( logDiff == 0 )
			{
				IsWidthPowerOfTwo = true;
				WidthBitCount = (int) log;
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

		public string Name { get; set; }
		public object GetContext()
		{
			return ImageContext;
		}

		public void SetContext(object context)
		{
			ImageContext = context as IImageContext;
		}

		public void InitializeFromAssetPart(IAssetPart assetPart)
		{
			AssetName = ((Image)assetPart).AssetName;
		}

		public Uri AssetName { get; set; }

		public IAssetPart[] GetAssetParts()
		{
			return new[] {this};
		}

		public void SetAssetParts(IEnumerable<IAssetPart> assetParts)
		{
			var parts = assetParts.ToArray();
			if( parts.Length > 1 )
			{
				throw new ArgumentException("An image can't have more than one assetpart");
			}
			var image = ((Image) parts[0]);
			ImageContext = image.ImageContext;
			Width = image.Width;
			Height = image.Height;
		}
	}
}