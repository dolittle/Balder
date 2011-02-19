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
#if(SILVERLIGHT)
using System;
using System.IO;
using System.Windows;

namespace Balder.Content.Silverlight
{
	/// <summary>
	/// Represents a <see cref="IFileLoader"/> for loading files from within the current Silverlight application
	/// </summary>
	public class FileLoader : IFileLoader
	{
		private readonly IContentManager _contentManager;
		private readonly FilePathHelper _filePathHelper;

		/// <summary>
		/// Initializes a new instance of a <see cref="FileLoader"/>
		/// </summary>
		/// <param name="contentManager"><see cref="IContentManager"/> to use for managing the content</param>
		/// <param name="filePathHelper"><see cref="FilePathHelper"/> </param>
		public FileLoader(IContentManager contentManager, FilePathHelper filePathHelper)
		{
			_contentManager = contentManager;
			_filePathHelper = filePathHelper;
		}


#pragma warning disable 1591 // Xml Comments
		public Stream GetStream(string assetName)
		{
			var filename = _filePathHelper.GetFileNameForAsset(assetName);

			// Todo: add check to see if file exists
			var resourceStream = Application.GetResourceStream(new Uri(filename, UriKind.Relative));
			
			return resourceStream.Stream;
		}

		public bool Exists(string fileName)
		{
			throw new NotImplementedException();
		}

		public string[] SupportedSchemes
		{
			get { throw new NotImplementedException(); }
		}
#pragma warning restore 1591 // Xml Comments
	}
}
#endif