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
	public class MessengerTests : TestFixture
	{
		[Test]
		public void DefaultContextShouldExist()
		{
			Assert.That(Messenger.DefaultContext, Is.Not.Null);
		}

		[Test]
		public void SpecifyingContextShouldReturnAContext()
		{
			var context = Messenger.Context(new object());
			Assert.That(context,Is.Not.Null);
		}

		[Test]
		public void GettingContextForSameObjectTwiceShouldReturnSameContext()
		{
			var obj = new object();
			var firstContext = Messenger.Context(obj);
			var secondContext = Messenger.Context(obj);
			Assert.That(secondContext, Is.SameAs(firstContext));
		}
		
		[Test]
		public void GettingContextForObjectShouldNotReturnDefaultContext()
		{
			var obj = new object();
			var context = Messenger.Context(obj);
			Assert.That(context, Is.Not.SameAs(Messenger.DefaultContext));
		}

	}
}

