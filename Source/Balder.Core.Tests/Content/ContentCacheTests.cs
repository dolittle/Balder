#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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
using Balder.Core.Content;
using NUnit.Framework;

namespace Balder.Core.Tests.Content
{
	[TestFixture]
	public class ContentCacheTests
	{
		[Test]
		public void PuttingContentInCacheShouldReturnSameWhenGetting()
		{
			var contentCache = new ContentCache();
			var cacheable = new object();
			var key = Guid.NewGuid().ToString();
			contentCache.Put<object>(key,cacheable);
			var actual = contentCache.Get<object>(key);

			Assert.That(actual,Is.Not.Null);
			Assert.That(actual,Is.EqualTo(cacheable));
		}

		[Test]
		public void PuttingContentInCacheShouldReturnTrueWhenAskingIfItExists()
		{
			var contentCache = new ContentCache();
			var cacheable = new object();
			var key = Guid.NewGuid().ToString();
			contentCache.Put<object>(key, cacheable);

			var exists = contentCache.Exists<object>(key);
			Assert.That(exists,Is.True);
		}
	}
}

