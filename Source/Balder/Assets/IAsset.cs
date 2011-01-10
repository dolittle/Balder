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
using System.Collections.Generic;

namespace Balder.Assets
{
	/// <summary>
	/// Defines an asset
	/// </summary>
	public interface IAsset
	{
		/// <summary>
		/// Gets or sets the asset name in the form of a <see cref="Uri"/>
		/// </summary>
		Uri AssetName { get; set; }

		/// <summary>
		/// Get all <see cref="IAssetPart">asset parts</see> in the asset
		/// </summary>
		/// <returns>An array of <see cref="IAssetPart"/></returns>
		IAssetPart[] GetAssetParts();

		/// <summary>
		/// Set all <see cref="IAssetPart">asset parts</see> for the asset
		/// </summary>
		/// <param name="assetParts"><see cref="IEnumerable{T}"/> of <see cref="IAssetPart"/> to set</param>
		void SetAssetParts(IEnumerable<IAssetPart> assetParts);
	}
}