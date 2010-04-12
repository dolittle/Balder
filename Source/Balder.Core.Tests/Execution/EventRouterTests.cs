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
using Balder.Core.Execution;
using NUnit.Framework;

namespace Balder.Core.Tests.Execution
{
	public delegate void SomeEventHandler();
	

	[TestFixture]
	public class EventRouterTests
	{
		[Test]
		public void GettingAnEventRouterShouldReturnInstanceWithNameSet()
		{
			var eventName = "SomeEvent";
			var instance = EventRouter.Get(eventName);
			Assert.That(instance,Is.Not.Null);
			Assert.That(instance.Name,Is.EqualTo(eventName));
		}

		[Test]
		public void GettingAnEventRouterTwiceShouldReturnSameInstance()
		{
			var eventName = "SomeEvent";
			var firstInstance = EventRouter.Get(eventName);
			var secondInstance = EventRouter.Get(eventName);
			Assert.That(firstInstance,Is.EqualTo(secondInstance));
		}


		[Test]
		public void RaisingAnEventShouldCallListeners()
		{
			var eventName = "SomeEvent";
			var instance = EventRouter.Get(eventName);

			var called = false;
			Action action = delegate { called = true; };
			instance.AddListener(action);

			SomeEventHandler e = delegate { };

			instance.Raise(e);

			Assert.That(called,Is.True);
		}

		[Test]
		public void RaisingAnEventShouldCallEventHandlers()
		{
			var eventName = "SomeEvent";
			var instance = EventRouter.Get(eventName);

			var called = false;
			SomeEventHandler e = delegate { called = true; };

			instance.Raise(e);

			Assert.That(called, Is.True);
		}

		[Test]
		public void RaisingEventWithArgumentsShouldPassArgumentsToEventHandlers()
		{
			var eventName = "EventWithArgs";
			var instance = EventRouter.Get(eventName);

			object sender = null;
			EventArgs eventArgs = null;
			EventHandler eh = (s, e) =>
			                  	{
			                  		sender = s;
			                  		eventArgs = e;
			                  	};

			var expectedSender = this;
			var expectedEventArgs = new EventArgs();
			instance.Raise(eh,expectedSender, expectedEventArgs);
			Assert.That(sender, Is.Not.Null);
			Assert.That(sender, Is.EqualTo(expectedSender));
			Assert.That(eventArgs, Is.Not.Null);
			Assert.That(eventArgs, Is.EqualTo(expectedEventArgs));
		}

		[Test]
		public void RaisingEventWithArgumentsShouldPassArgumentsToListeners()
		{
			var eventName = "EventWithArgs";
			var instance = EventRouter.Get(eventName);

			object sender = null;
			EventArgs eventArgs = null;
			EventHandler eh = (s, e) =>
			{
				sender = s;
				eventArgs = e;
			};
			instance.AddListener(eh);

			EventHandler emptyEh = (s, e) => { };

			var expectedSender = this;
			var expectedEventArgs = new EventArgs();
			instance.Raise(emptyEh, expectedSender, expectedEventArgs);
			Assert.That(sender, Is.Not.Null);
			Assert.That(sender, Is.EqualTo(expectedSender));
			Assert.That(eventArgs, Is.Not.Null);
			Assert.That(eventArgs, Is.EqualTo(expectedEventArgs));
		}
	}
}

