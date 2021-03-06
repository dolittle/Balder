﻿#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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

using System.Collections.Generic;
using Balder.Assets;
using Balder.Execution;
using Ninject;


namespace Balder.Content
{
	/// <summary>
	/// Represents a <see cref="IContentManager"/>
	/// </summary>
	[Singleton]
	public class ContentManager : IContentManager
	{
		public const string DefaultAssetsRoot = "Assets";
		private readonly IKernel _kernel;
		private readonly IContentCache _contentCache;
		private readonly IAssetLoaderService _assetLoaderService;

		/// <summary>
		/// Initializes a new instance of <see cref="ContentManager"/>
		/// </summary>
		/// <param name="kernel"><see cref="IKernel"/> to use for object creation</param>
		/// <param name="contentCache"><see cref="IContentCache"/> to use for caching</param>
		/// <param name="assetLoaderService"><see cref="IAssetLoaderService"/> for resolving asset loaders</param>
		/// <param name="contentCreator"><see cref="IContentCreator"/> for creating new content</param>
		public ContentManager(
			IKernel kernel, 
			IContentCache contentCache, 
			IAssetLoaderService assetLoaderService, 
			IContentCreator contentCreator)
		{
			_kernel = kernel;
			_contentCache = contentCache;
			_assetLoaderService = assetLoaderService;
			AssetsRoot = DefaultAssetsRoot;
			Creator = contentCreator;
		}

#pragma warning disable 1591 // Xml Comments
		public T Load<T>(string assetName)
			where T : IAsset
		{
			var asset = _kernel.Get<T>();
			LoadInto(asset,assetName);
			return asset;
		}

		public void LoadInto<T>(T asset, string assetName)
			where T : IAsset
		{
			

			IEnumerable<IAssetPart> assetParts;
			if( _contentCache.Exists<T>(assetName) )
			{
				var newAssetParts = new List<IAssetPart>();
				assetParts = _contentCache.Get<T>(assetName);
				foreach( var assetPart in assetParts )
				{
					var type = assetPart.GetType();
					var context = assetPart.GetContext();
					var newPart = _kernel.Get(type) as IAssetPart;
					newPart.SetContext(context);
					newPart.InitializeFromAssetPart(assetPart);
					newAssetParts.Add(newPart);

					// Todo: Image needs to have ImageFrame - divide the Asset from its AssetParts
					if( newPart is IAsset )
					{
						((IAsset)newPart).SetAssetParts(new[] { assetPart });
						
					}
				}
				assetParts = newAssetParts;
			} else
			{
				var loader = _assetLoaderService.GetLoader<T>(assetName);
				assetParts = loader.Load(assetName);
				_contentCache.Put<T>(assetName,assetParts);
			}
			
			asset.SetAssetParts(assetParts);
		}


		public T CreateAssetPart<T>() where T : IAssetPart
		{
			var part = _kernel.Get<T>();
			return part;
		}

		public IContentCreator Creator { get; private set; }

		public string AssetsRoot { get; set; }
#pragma warning restore 1591 // Xml Comments
	}
}
