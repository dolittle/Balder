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
using NUnit.Framework;

namespace Balder.Core.Tests.Execution
{
	[TestFixture]
	public class MessengerContextTests
	{
		public class Message
		{
			public int Content { get; set; }
		}

		public class SecondMessage
		{
			
		}

		[Test]
		public void SendingShouldCallListener()
		{
			var wasCalled = false;
			var context = new MessengerContext();
			context.ListenTo<Message>(m => wasCalled = true);
			context.Send(new Message());
			Assert.That(wasCalled,Is.True);
		}

		[Test]
		public void SendingShouldCallAllListeners()
		{
			var callCount = 0;
			var context = new MessengerContext();
			context.ListenTo<Message>(m => callCount++);
			context.ListenTo<Message>(m => callCount++);
			context.Send(new Message());
			Assert.That(callCount, Is.EqualTo(2));
		}

		[Test]
		public void ContentShouldBeForwardedToListener()
		{
			var actualContent = 0;
			var expectedContent = 42;
			var context = new MessengerContext();
			context.ListenTo<Message>(m => actualContent = m.Content);
			context.Send(new Message { Content = expectedContent });
			Assert.That(actualContent, Is.EqualTo(expectedContent));
		}

		[Test]
		public void ListeningToOneMessageTypeAndSendingAnotherShouldNotMakeTheListenerGetCalled()
		{
			var wasCalled = false;
			var context = new MessengerContext();
			context.ListenTo<Message>(m => wasCalled = true);
			context.Send(new SecondMessage());
			Assert.That(wasCalled, Is.False);
		}
	}
}