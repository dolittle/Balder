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
	/// Represents a <see cref="IContentCache"/>
	/// </summary>
	public class ContentCache : IContentCache
	{
		static class TypeCache<T>
			where T : IAsset
		{
			public static Dictionary<object, IEnumerable<IAssetPart>> Cache { get; private set; }

			static TypeCache()
			{
				Cache = new Dictionary<object, IEnumerable<IAssetPart>>();
			}
			
		}

#pragma warning disable 1591 // Xml Comments
		public bool Exists<T>(object key)
			where T:IAsset
		{
			return TypeCache<T>.Cache.ContainsKey(key);
		}

		public IEnumerable<IAssetPart> Get<T>(object key)
			where T : IAsset
		{
			if( Exists<T>(key))
			{
				return TypeCache<T>.Cache[key];
			}
			return null;
		}

		public void Put<T>(object key, IEnumerable<IAssetPart> content)
			where T : IAsset
		{
			TypeCache<T>.Cache[key] = content;
		}
#pragma warning restore 1591 // Xml Comments
	}

}