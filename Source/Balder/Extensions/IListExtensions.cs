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
using System.Collections;
using System.Collections.Generic;

namespace Balder.Extensions
{
	/// <summary>
	/// Extension methods for IList
	/// </summary>
	public static class IListExtensions
	{
		/// <summary>
		/// Add a range of items to a list
		/// </summary>
		/// <param name="list"><see cref="IList"/> to add to</param>
		/// <param name="range"><see cref="IEnumerable"/> containing the range of items to add</param>
		public static void AddRange(this IList list, IEnumerable range)
		{
			foreach (var item in range)
			{
				list.Add(item);
			}
		}

		/// <summary>
		/// Add a range of items to a generic list
		/// </summary>
		/// <typeparam name="T">Type of item to add - will be inferred by compiler</typeparam>
		/// <param name="list"><see cref="IList{T}"/> to add to</param>
		/// <param name="range"><see cref="IEnumerable{T}"/> containing the range of items to add</param>
		public static void AddRange<T>(this IList<T> list, IEnumerable<T> range)
		{
			foreach( var item in range )
			{
				list.Add(item);
			}
		}
	}
}