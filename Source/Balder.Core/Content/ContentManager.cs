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

using Balder.Core.Assets;
using Balder.Core.Execution;
using Ninject.Core;

#pragma warning disable 1591
namespace Balder.Core.Content
{
	[Singleton]
	public class ContentManager : IContentManager
	{
		public const string DefaultAssetsRoot = "Assets";
		private readonly IObjectFactory _objectFactory;
		private readonly IContentCache _contentCache;
		private readonly IAssetLoaderService _assetLoaderService;

		public ContentManager(IObjectFactory objectFactory, IContentCache contentCache, IAssetLoaderService assetLoaderService)
		{
			_objectFactory = objectFactory;
			_contentCache = contentCache;
			_assetLoaderService = assetLoaderService;
			AssetsRoot = DefaultAssetsRoot;
			Creator = new ContentCreator(objectFactory);
		}

		public T Load<T>(string assetName)
			where T : IAsset
		{
			var asset = _objectFactory.Get<T>();
			LoadInto(asset,assetName);
			return asset;
		}

		public void LoadInto<T>(T asset, string assetName)
			where T : IAsset
		{
			var loader = _assetLoaderService.GetLoader<T>(assetName);
			var assetParts = loader.Load(assetName);
			asset.SetAssetParts(assetParts);
		}


		public T CreateAssetPart<T>() where T : IAssetPart
		{
			var part = _objectFactory.Get<T>();
			return part;
		}

		public ContentCreator Creator { get; private set; }

		public string AssetsRoot { get; set; }
	}
}
#pragma warning restore 1591