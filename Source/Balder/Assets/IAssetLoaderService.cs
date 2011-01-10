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

namespace Balder.Assets
{
	/// <summary>
	/// Defines a service for loading assets
	/// </summary>
	public interface IAssetLoaderService
	{
		/// <summary>
		/// Initialize the service
		/// </summary>
		void Initialize();

		/// <summary>
		/// Get a loader based upon type and the name of the asset
		/// </summary>
		/// <typeparam name="T">Type of the <see cref="IAsset">asset</see> to load</typeparam>
		/// <param name="assetName">Name of the <see cref="IAsset">asset</see> to load</param>
		/// <returns>The <see cref="IAssetLoader"/> for the type and file, null if none was registered</returns>
		IAssetLoader GetLoader<T>(string assetName) where T : IAsset; // Todo: inconsistency - IAsset uses URI

		/// <summary>
		/// Get all available <see cref="IAssetLoader">asset loaders</see>
		/// </summary>
		IAssetLoader[] AvailableLoaders { get; }
	}
}