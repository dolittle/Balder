#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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
using Balder.Core.Assets;
using Balder.Core.Content;
using NUnit.Framework;

namespace Balder.Core.Tests.Content
{
	[TestFixture]
	public class ContentCacheTests
	{
		public class MyAssetPart : IAssetPart
		{
			public string Name { get; set; }

			private object _context;
			public object GetContext()
			{
				return _context;
			}

			public void SetContext(object context)
			{
				_context = context;
			}

			public void InitializeFromAssetPart(IAssetPart assetPart)
			{
				
			}
		}

		public class MyAsset : IAsset
		{
			public MyAssetPart[] AssetParts { get; set; }

			public Uri AssetName { get; set; }

			public IAssetPart[] GetAssetParts()
			{
				return AssetParts;
			}

			public void SetAssetParts(IEnumerable<IAssetPart> assetParts)
			{
				AssetParts = (MyAssetPart[]) assetParts;
			}
		}

		[Test]
		public void PuttingContentInCacheShouldReturnSameWhenGetting()
		{
			var contentCache = new ContentCache();
			var asset = new MyAsset();
			var assetParts = new[] {new MyAssetPart()};
			asset.AssetParts = assetParts;
			
			var key = Guid.NewGuid().ToString();
			contentCache.Put<MyAsset>(key,assetParts);
			var actualParts = contentCache.Get<MyAsset>(key);

			Assert.That(actualParts,Is.Not.Null);
			Assert.That(actualParts.ToArray().Length,Is.EqualTo(assetParts.Length));
		}

		[Test]
		public void PuttingContentInCacheShouldReturnTrueWhenAskingIfItExists()
		{
			var contentCache = new ContentCache();
			var asset = new MyAsset();
			var assetParts = new[] { new MyAssetPart() };
			asset.AssetParts = assetParts;
			var key = Guid.NewGuid().ToString();
			contentCache.Put<MyAsset>(key, assetParts);

			var exists = contentCache.Exists<MyAsset>(key);
			Assert.That(exists,Is.True);
		}
	}
}

