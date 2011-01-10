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
	/// Defines an asset part
	/// </summary>
	public interface IAssetPart
	{
		/// <summary>
		/// Gets or sets the name of the <see cref="IAssetPart"/>
		/// </summary>
		string Name { get; set; }

		/// <summary>
		/// Get context object for the <see cref="IAssetPart"/>
		/// </summary>
		/// <returns>The context object</returns>
		object GetContext();

		/// <summary>
		/// Set the context object for the <see cref="IAssetPart"/>
		/// </summary>
		/// <param name="context"></param>
		void SetContext(object context);

		/// <summary>
		/// Initialize from a different <see cref="IAssetPart"/>
		/// </summary>
		/// <param name="assetPart"><see cref="IAssetPart"/> to initialize from</param>
		void InitializeFromAssetPart(IAssetPart assetPart);
	}
}