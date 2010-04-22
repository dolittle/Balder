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
using Balder.Core.Assets;
using Balder.Core.Content;
using Balder.Core.Tests.Stubs;
using Moq;
using NUnit.Framework;

namespace Balder.Core.Tests.Content
{
	[TestFixture]
	public class ContentManagerTests
	{
		public class MyAsset : IAsset
		{
			public bool LoadCalled = false;
			public void Load(string assetName)
			{
				LoadCalled = true;
			}
		}

		public class MyAssetPart : IAssetPart
		{
			public string Name { get; set; }
			public object CacheKey { get; set; }
			public object GetContext()
			{
				return Context;
			}

			public void SetContext(object obj)
			{
				Context = obj;
			}

			public object Context { get; set; }
		}

		[Test]
		public void LoadingAssetShouldCallLoadOnAsset()
		{
			var objectFactoryStub = new ObjectFactoryStub();
			var cacheMock = new Mock<IContentCache>();
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var contentManager = new ContentManager(objectFactoryStub, cacheMock.Object, assetLoaderServiceMock.Object);
			var asset = contentManager.Load<MyAsset>("something");
			Assert.That(asset,Is.Not.Null);
			Assert.That(asset.LoadCalled,Is.True);
		}

		[Test]
		public void LoadingAssetPartShouldReturnAnAssetPart()
		{
			var objectFactoryStub = new ObjectFactoryStub();
			var cacheMock = new Mock<IContentCache>();
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var contentManager = new ContentManager(objectFactoryStub, cacheMock.Object, assetLoaderServiceMock.Object);
			var assetPart = contentManager.LoadPart<MyAssetPart>("something");

			Assert.That(assetPart,Is.Not.Null);
		}

		[Test]
		public void LoadingAssetPartForTheFirstTimeShouldLoadIt()
		{
			var objectFactoryStub = new ObjectFactoryStub();
			var cacheMock = new Mock<IContentCache>();
			cacheMock.Expect(c => c.Exists<MyAssetPart>(It.IsAny<object>())).Returns(false);
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			assetLoaderServiceMock.Expect(a => a.GetLoader<MyAssetPart>(It.IsAny<string>()));
			var contentManager = new ContentManager(objectFactoryStub, cacheMock.Object, assetLoaderServiceMock.Object);
			var assetPart = contentManager.LoadPart<MyAssetPart>("something");
			assetLoaderServiceMock.VerifyAll();
		}

		[Test]
		public void LoadingAssetPartForTheFirstTimeShouldAddContextToCache()
		{
			var objectFactoryStub = new ObjectFactoryStub();
			var cacheMock = new Mock<IContentCache>();
			cacheMock.Expect(c => c.Put<object>(It.IsAny<object>(), It.IsAny<object>()));

			var assetLoaderMock = new Mock<AssetLoader<MyAssetPart>>();
			assetLoaderMock.Expect(a => a.Load(It.IsAny<string>())).Returns(new [] {new MyAssetPart()});
			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			assetLoaderServiceMock.Expect(a => a.GetLoader<MyAssetPart>(It.IsAny<string>()));
			var contentManager = new ContentManager(objectFactoryStub, cacheMock.Object, assetLoaderServiceMock.Object);
			var assetPart = contentManager.LoadPart<MyAssetPart>("something");
			cacheMock.VerifyAll();
		}
	}
}

