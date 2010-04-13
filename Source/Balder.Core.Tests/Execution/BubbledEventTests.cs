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
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;
using NUnit.Framework;

namespace Balder.Core.Tests.Execution
{
	public class SimpleClassWithBubbledEvent : INode
	{
		public static readonly BubbledEvent<SimpleClassWithBubbledEvent, BubbledEventHandler> SimpleEvent =
			BubbledEvent<SimpleClassWithBubbledEvent, BubbledEventHandler>.Register(s => s.Event);

		public event BubbledEventHandler Event;


		public INode Parent { get; set; }
		public Matrix ActualWorld { get; private set; }
		public Matrix RenderingWorld { get; set; }
		public Scene Scene { get; set; }
		public void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{

		}
	}

	public class HierarchicalClass : INode
	{
		public static readonly BubbledEvent<HierarchicalClass, BubbledEventHandler> SomeEventBubbledEvent =
			BubbledEvent<HierarchicalClass, BubbledEventHandler>.Register(s => s.SomeEvent);

		public event BubbledEventHandler SomeEvent;

		public INode Parent { get; set; }
		public Matrix ActualWorld { get; private set; }
		public Matrix RenderingWorld { get; set; }
		public Scene Scene { get; set; }
		public void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{

		}
	}




	[TestFixture]
	public class BubbledEventTests
	{
		[Test]
		public void RaisingBubbledEventShouldCallHandlersAdded()
		{
			var simpleClass = new SimpleClassWithBubbledEvent();

			var called = false;
			simpleClass.Event += (s, e) => called = true;
			SimpleClassWithBubbledEvent.SimpleEvent.Raise(simpleClass, null, null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void RaisingBubbledEventOnChildShouldRaiseItOnParent()
		{
			var parent = new HierarchicalClass();
			var child = new HierarchicalClass { Parent = parent };

			var called = false;
			parent.SomeEvent += (s, e) => called = true;
			HierarchicalClass.SomeEventBubbledEvent.Raise(child, null, null);

			Assert.That(called, Is.True);
		}

		[Test]
		public void RaisingBubbledEventShouldPassArguments()
		{
			var simpleClass = new SimpleClassWithBubbledEvent();

			INode sender = null;
			BubbledEventArgs eventArgs = null;
			simpleClass.Event += (s, e) =>
									{
										sender = s;
										eventArgs = e;
									};

			var expectedSender = simpleClass;
			var expectedEventArgs = new BubbledEventArgs();
			SimpleClassWithBubbledEvent.SimpleEvent.Raise(simpleClass, expectedSender, expectedEventArgs);
			Assert.That(sender, Is.EqualTo(expectedSender));
			Assert.That(eventArgs, Is.EqualTo(expectedEventArgs));
		}

		[Test]
		public void RaisingBubbledEventWithBubbledEventArgsShouldSetOriginalSource()
		{
			var simpleClass = new SimpleClassWithBubbledEvent();

			INode source = null;
			simpleClass.Event += (s, e) => source = e.OriginalSource;

			SimpleClassWithBubbledEvent.SimpleEvent.Raise(simpleClass,simpleClass,new BubbledEventArgs());
			Assert.That(source,Is.EqualTo(simpleClass));
		}

		[Test]
		public void RaisingBubbledEventWithBubbledEventArgsShouldSetOriginalSourceToChildAndSenderToParent()
		{
			var parent = new HierarchicalClass();
			var child = new HierarchicalClass {Parent = parent};

			INode sender = null;
			INode originalSource = null;

			parent.SomeEvent += (s, e) =>
			                    	{
			                    		sender = s;
			                    		originalSource = e.OriginalSource;
			                    	};
			
			HierarchicalClass.SomeEventBubbledEvent.Raise(child,child,new BubbledEventArgs());

			Assert.That(sender,Is.EqualTo(parent));
			Assert.That(originalSource, Is.EqualTo(child));
		}

		[Test]
		public void SettingHandledToTrueForBubblingEventShouldNotRaiseEventOnParent()
		{
			var parent = new HierarchicalClass();
			var child = new HierarchicalClass { Parent = parent };

			var called = false;
			child.SomeEvent += (s, e) => e.Handled = true;
			parent.SomeEvent += (s, e) => called = true;

			HierarchicalClass.SomeEventBubbledEvent.Raise(child, child, new BubbledEventArgs());

			Assert.That(called,Is.False);
		}

	}
}

