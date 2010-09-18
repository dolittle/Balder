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
using System.IO;

namespace Balder.Content
{
	/// <summary>
	/// To load files in Balder, you should go through the fileloader. 
	/// Loading of files is a platform specific operation.
	/// </summary>
	public interface IFileLoader
	{
		/// <summary>
		/// Get the stream of a given asset
		/// </summary>
		/// <param name="fileName">Filename for the file you want to load</param>
		/// <returns>A stream that can be used to retrieve the content</returns>
		Stream GetStream(string fileName);

		/// <summary>
		/// Gets the status of a file exists or not
		/// </summary>
		/// <param name="fileName">Filename to check</param>
		/// <returns>True if exists, false if not</returns>
		bool Exists(string fileName);

		/// <summary>
		/// Gets the supported URI schemes
		/// </summary>
		string[] SupportedSchemes { get; }
	}
}