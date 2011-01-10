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
using Balder.Assets;

namespace Balder.Content
{
	/// <summary>
	/// Defines a manager for content
	/// </summary>
	public interface IContentManager
	{
		/// <summary>
		/// Load an asset based on the assetname.
		/// </summary>
		/// <typeparam name="T">Type of asset to load</typeparam>
		/// <param name="assetName">Name of asset - usually a file reference</param>
		/// <returns>Loaded asset</returns>
		T Load<T>(string assetName) where T : IAsset;

		/// <summary>
		/// Load content into an asset
		/// </summary>
		/// <typeparam name="T">Type of asset to load into</typeparam>
		/// <param name="asset">Asset to load into</param>
		/// <param name="assetName">Name of asset - usually a file reference</param>
		void LoadInto<T>(T asset, string assetName) where T : IAsset;

		/// <summary>
		/// Create an asset part - assets can have many parts
		/// </summary>
		/// <typeparam name="T">Type of assetpart to create</typeparam>
		/// <returns>Created assetpart</returns>
		T CreateAssetPart<T>() where T : IAssetPart;

		/// <summary>
		/// Get the ContentCreator
		/// </summary>
		IContentCreator Creator { get; }

		/// <summary>
		/// Get or set root of assets - typically a folder relative to current directory or a specific folder
		/// </summary>
		string AssetsRoot { get; set; }
	}
}