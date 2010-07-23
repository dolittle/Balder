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

using Balder.Core.Execution;
using Balder.Testing;
using NUnit.Framework;

namespace Balder.Core.Tests.Execution
{
	[TestFixture]
	public class IdentityManagerTests : TestFixture
	{
		[Test]
		public void GettingFirstIdentityForTypeShouldReturnOne()
		{
			var identityManager = new IdentityManager();
			identityManager.Reset<IdentityManagerTests>();
			var identity = identityManager.AllocateIdentity<IdentityManagerTests>();
			Assert.That(identity,Is.EqualTo(1));
		}

		[Test]
		public void GettingFirstIdentityForTwoDifferentTypesShouldReturnOneOnBoth()
		{
			var identityManager = new IdentityManager();
			identityManager.Reset<IdentityManagerTests>();
			identityManager.Reset<string>();
			var identity = identityManager.AllocateIdentity<IdentityManagerTests>();
			Assert.That(identity, Is.EqualTo(1));
			identity = identityManager.AllocateIdentity<string>();
			Assert.That(identity, Is.EqualTo(1));
		}

		[Test]
		public void GettingIdentityTwiceForSameTypeShouldReturnDifferentIdentities()
		{
			var identityManager = new IdentityManager();
			identityManager.Reset<IdentityManagerTests>();
			var identity = identityManager.AllocateIdentity<IdentityManagerTests>();
			var secondIdentity = identityManager.AllocateIdentity<IdentityManagerTests>();
			Assert.That(secondIdentity, Is.Not.EqualTo(identity));
		}

		[Test]
		public void ReleasingIdentityShouldReturnReleasedIdentityWhenGettingNewIdentityForSameType()
		{
			var identityManager = new IdentityManager();
			identityManager.Reset<IdentityManagerTests>();
			var identity = identityManager.AllocateIdentity<IdentityManagerTests>();
			var secondIdentity = identityManager.AllocateIdentity<IdentityManagerTests>();
			var thirdIdentity = identityManager.AllocateIdentity<IdentityManagerTests>();
			identityManager.ReleaseIdentity<IdentityManagerTests>(secondIdentity);
			var releasedIdentity = identityManager.AllocateIdentity<IdentityManagerTests>();
			Assert.That(releasedIdentity,Is.EqualTo(secondIdentity));
		}
	}
}