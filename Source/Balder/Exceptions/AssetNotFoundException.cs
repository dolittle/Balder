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

namespace Balder.Exceptions
{
	/// <summary>
	/// The exception that is thrown when an asset is not found during load
	/// </summary>
	public class AssetNotFoundException : ArgumentException
	{
		/// <summary>
		/// Initializes a new instance of <see cref="AssetNotFoundException">AssetNotFoundException</see>
		/// </summary>
		/// <param name="asset">AssetName that couldn't be found</param>
		public AssetNotFoundException(string asset)
			: base("Asset '"+asset+"' could not be found")
		{
			
		}
	}
}
