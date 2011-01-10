#region License
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

using System;
using Balder.Execution;
using Ninject;

namespace Balder.Content
{
	/// <summary>
	/// Represents a <see cref="IFileLoaderManager"/>
	/// </summary>	
	public class FileLoaderManager : IFileLoaderManager
	{
		private readonly IFileLoader _defaultFileLoader;
		private readonly ITypeDiscoverer _typeDiscoverer;
		private readonly IKernel _kernel;
		private IFileLoader[] _fileLoaders;

		/// <summary>
		/// Initializes a new instance of <see cref="FileLoaderManager"/>
		/// </summary>
		/// <param name="defaultFileLoader">Default <see cref="IFileLoader"/> to use</param>
		/// <param name="typeDiscoverer"><see cref="ITypeDiscoverer"/> to use for discovering types</param>
		/// <param name="kernel"><see cref="IKernel"/> to use for object creation</param>
		public FileLoaderManager(
			IFileLoader defaultFileLoader, 
			ITypeDiscoverer typeDiscoverer, 
			IKernel kernel)
		{
			_defaultFileLoader = defaultFileLoader;
			_typeDiscoverer = typeDiscoverer;
			_kernel = kernel;
			DiscoverFileLoaders();
		}

		private void DiscoverFileLoaders()
		{
			var fileLoaderTypes = _typeDiscoverer.FindMultiple<IFileLoader>();

			_fileLoaders = new IFileLoader[fileLoaderTypes.Length];
			for( var fileLoaderTypeIndex=0; fileLoaderTypeIndex<fileLoaderTypes.Length; fileLoaderTypeIndex++)
			{
				_fileLoaders[fileLoaderTypeIndex] = _kernel.Get(fileLoaderTypes[fileLoaderTypeIndex]) as IFileLoader;
			}
		}


#pragma warning disable 1591
		public IFileLoader GetFileLoader(string fileName)
		{
			if (fileName.Contains("://"))
			{
				var uri = new Uri(fileName);
				foreach (var fileLoader in _fileLoaders)
				{
					if (null == fileLoader.SupportedSchemes)
					{
						continue;
					}
					foreach (var scheme in fileLoader.SupportedSchemes)
					{
						if (uri.Scheme.ToLower().Equals(scheme.ToLower()))
						{
							return fileLoader;
						}
					}
				}
			}

			return _defaultFileLoader;
		}
#pragma warning restore 1591
	}
}