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
using System.Collections.Generic;
using Balder.Assets;

namespace Balder.Content
{
	/// <summary>
	/// Defines a caching mechanism for content being created or loaded
	/// </summary>
	public interface IContentCache
	{
		/// <summary>
		/// Check if content is in the cache based upon a key
		/// </summary>
		/// <typeparam name="T">Type of asset to check</typeparam>
		/// <param name="key">Key of the content</param>
		/// <returns>True if content is in the cache, false if not</returns>
		bool Exists<T>(object key) where T : IAsset;

		/// <summary>
		/// Get content from the cache based upon the key
		/// </summary>
		/// <typeparam name="T">Type of asset the content belongs to</typeparam>
		/// <param name="key">Key of the content</param>
		/// <returns>The asset parts in the cache</returns>
		IEnumerable<IAssetPart> Get<T>(object key) where T : IAsset;

		/// <summary>
		/// Put content into the cache based upon a key
		/// </summary>
		/// <typeparam name="T">Type of asset the content belongs to</typeparam>
		/// <param name="key">Key of the content</param>
		/// <param name="parts">AssetParts to put in the cache</param>
		void Put<T>(object key, IEnumerable<IAssetPart> parts) where T : IAsset;
	}
}