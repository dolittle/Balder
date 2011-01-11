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

using Balder.Execution;
using Balder.Utils;

namespace Balder.Content.Silverlight
{
	/// <summary>
	/// Represents a <see cref="IFilePathHelper"/>
	/// </summary>
	public class FilePathHelper : IFilePathHelper
	{
		private readonly IContentManager _contentManager;
		private readonly IPlatform _platform;

		/// <summary>
		/// Initializes an instance of <see cref="FilePathHelper"/>
		/// </summary>
		/// <param name="contentManager"><see cref="IContentManager"/> used during resolving complete filenames</param>
		/// <param name="platform">An instance of the <see cref="IPlatform"/> for getting information about execution context</param>
		public FilePathHelper(IContentManager contentManager, IPlatform platform)
		{
			_contentManager = contentManager;
			_platform = platform;
		}

#pragma warning disable 1591 // Xml Comments
		public string GetFileNameForAsset(string assetName)
		{
			var fullAssemblyName = _platform.EntryAssemblyName;
			var assemblyName = AssemblyHelper.GetAssemblyShortName(fullAssemblyName);
			var filename = string.Empty;

			if( assetName.Contains(";component"))
			{
				filename = assetName;
			} else
			{
				filename = string.Format("/{0};component/{1}/{2}", assemblyName, _contentManager.AssetsRoot, assetName);	
			}

			filename = filename.Replace("\\", "/");
			filename = filename.Replace("//", "/");
			
			return filename;
		}
#pragma warning restore 1591 // Xml Comments
	}
}
