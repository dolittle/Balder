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
using Balder.Execution;
using NUnit.Framework;

namespace Balder.Tests.Execution
{
	[TestFixture]
	public class MessengerSubscriptionTests
	{
		public class Message
		{
			public int Content { get; set; }
		}

		[Test]
		public void ListenerShouldBeNotifiedDuringNotify()
		{
			var wasCalled = false;
			var subscription = new MessageSubscriptions<Message>();
			subscription.AddListener(this,m => wasCalled = true);
			subscription.Notify(new Message());
			Assert.That(wasCalled,Is.True);
		}

		[Test]
		public void MultipleListenersShouldBeNotifiedDuringNotify()
		{
			var callCount = 0;
			var subscription = new MessageSubscriptions<Message>();
			subscription.AddListener(this, m => callCount++);
			subscription.AddListener(this, m => callCount++);
			subscription.Notify(new Message());
			Assert.That(callCount,Is.EqualTo(2));
		}

		[Test]
		public void ContentShouldBeForwardedToListener()
		{
			var actualContent = 0;
			var expectedContent = 42;
			var subscription = new MessageSubscriptions<Message>();
			subscription.AddListener(this, m => actualContent = m.Content);
			subscription.Notify(new Message {Content = expectedContent});
			Assert.That(actualContent,Is.EqualTo(expectedContent));
		}

		[Test]
		public void RemovingListenerShouldNotCallTheListenerWhenNotifying()
		{
			throw new NotImplementedException();
		}
	}
}