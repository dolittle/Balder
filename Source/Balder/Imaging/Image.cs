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
using System.Linq;
using Balder.Assets;
using Balder.Execution;
#if(SILVERLIGHT)
using System.ComponentModel;
using Balder.Silverlight.TypeConverters;
using Ninject;
#endif

namespace Balder.Imaging
{
#if(SILVERLIGHT)
	[TypeConverter(typeof(UriToImageTypeConverter))]
#endif
	public class Image : IAsset, IAssetPart
	{
#if(SILVERLIGHT)
		public Image()
			: this(Runtime.Instance.Kernel.Get<IImageContext>())
		{
			
		}
#endif

		public Image(IImageContext imageContext)
		{
			ImageContext = imageContext;
		}

		
		public IImageContext ImageContext { get; set; }

		public int Width { get; set; }
		public int Height { get; set; }

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