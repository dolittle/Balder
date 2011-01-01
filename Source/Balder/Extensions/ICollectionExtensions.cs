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

namespace Balder.Extensions
{
	/// <summary>
	/// Extension methods for the generic version of ICollection
	/// </summary>
	public static class ICollectionExtensions
	{
		/// <summary>
		/// Add a range to a collection based on an generic IEnumerable with same
		/// type as the ICollection generic parameter
		/// </summary>
		/// <typeparam name="T">Type to add for - will be inferred compiletime</typeparam>
		/// <param name="collection">Collection to add items to</param>
		/// <param name="range">IEnumerable with items to add</param>
		public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> range)
		{
			foreach (var item in range)
			{
				collection.Add(item);
			}
		}
	}
}