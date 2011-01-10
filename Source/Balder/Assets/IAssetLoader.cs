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

namespace Balder.Assets
{
	/// <summary>
	/// Defines a loader that load a specific asset type
	/// </summary>
	public interface IAssetLoader
	{
		/// <summary>
		/// Get the file extensions supported by the loader
		/// </summary>
		string[] FileExtensions { get; }

		/// <summary>
		/// Get the supported asset type for the loader
		/// </summary>
		Type SupportedAssetType { get; }

		/// <summary>
		/// Load assets based on an assetname
		/// </summary>
		/// <param name="assetName">Name of the asset to load</param>
		/// <returns>An array of <see cref="IAssetPart">asset parts</see> loaded</returns>
		IAssetPart[] Load(string assetName); // Todo: Inconcistency - IAsset is using an Uri!
	}
}