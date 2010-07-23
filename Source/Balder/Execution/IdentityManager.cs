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

using System;
using System.Collections.Generic;

namespace Balder.Execution
{
	public class IdentityManager : IIdentityManager
	{
		private class IdentityCache<T>
		{
			private static UInt16 _identityCounter;
			private static readonly Queue<UInt16>	ReleasedIdentities = new Queue<UInt16>();

			static IdentityCache()
			{
				Reset();
			}

			public	static UInt16	AllocateIdentity()
			{
				UInt16 identity;
				if( ReleasedIdentities.Count > 0 )
				{
					identity = ReleasedIdentities.Dequeue();
					return identity;
				}

				identity = _identityCounter;
				_identityCounter++;
				return identity;
			}

			public static void Reset()
			{
				_identityCounter = 1;
			}

			public static void Release(UInt16 identity)
			{
				ReleasedIdentities.Enqueue(identity);
			}
		}


		public void Reset<T>()
		{
			IdentityCache<T>.Reset();
		}

		public UInt16 AllocateIdentity<T>()
		{
			var identity = IdentityCache<T>.AllocateIdentity();
			return identity;
		}

		public void ReleaseIdentity<T>(UInt16 identity)
		{
			IdentityCache<T>.Release(identity);
		}
	}
}