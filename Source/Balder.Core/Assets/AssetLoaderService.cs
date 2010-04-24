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
using System.IO;
using System.Linq;
using Balder.Core.Execution;
using Ninject.Core;

namespace Balder.Core.Assets
{
	[Singleton]
	public class AssetLoaderService : IAssetLoaderService
	{
		private readonly ITypeDiscoverer _typeDiscoverer;
		private readonly IKernel _kernel;
		private readonly Dictionary<string, IAssetLoader> _assetLoaders;

		public AssetLoaderService(ITypeDiscoverer typeDiscoverer, IKernel kernel)
		{
			_typeDiscoverer = typeDiscoverer;
			_kernel = kernel;
			_assetLoaders = new Dictionary<string, IAssetLoader>();
		}

		public void Initialize()
		{
			var loaders = _typeDiscoverer.FindMultiple<IAssetLoader>();
			foreach( var loaderType in loaders )
			{
				var loader = _kernel.Get(loaderType) as IAssetLoader;
				RegisterLoader(loader);
			}
		}

		private void RegisterLoader(IAssetLoader loader)
		{
			foreach (var extension in loader.FileExtensions)
			{
				_assetLoaders[extension.ToLower()] = loader;
			}
		}

		public IAssetLoader GetLoader<T>(string assetName)
			where T : IAsset
		{
			var extension = Path.GetExtension(assetName).ToLower();
			extension = extension.Substring(1);

			if (!_assetLoaders.ContainsKey(extension))
			{
				throw new ArgumentException("There is no loader for the specified file type '" + extension + "'");
			}

			return _assetLoaders[extension];
		}


		public IAssetLoader[]	AvailableLoaders
		{
			get
			{
				return _assetLoaders.Values.ToArray();
			}
		}
	}
}