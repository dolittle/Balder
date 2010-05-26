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

using System.Reflection;
using System.Windows;
using System.Windows.Media;
using CThru;
using CThru.Silverlight;
using NUnit.Framework;
using TypeMock;
using Typemock.Isolator.VisualBasic;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class NodeTests
	{
		public class SomeNode : RenderableNode
		{
			
		}

		private RenderableNode CreateNode()
		{
			var node = new SomeNode();
			return node;
		}

		[SetUp]
		public void Setup()
		{
			CThruEngine.AddAspectsInAssembly(Assembly.GetExecutingAssembly());

			using( var recorder = RecorderManager.StartRecording())
			{
				VisualStateManager.GoToState(null, string.Empty, false);
				recorder.Return(true);
			}

			using( TheseCalls.WillReturn(true))
			{
				VisualStateManager.GoToState(null, string.Empty, false);
			}

			CThruEngine.StartListening();
		}

		[TearDown]
		public void TearDown()
		{
			CThruEngine.StopListening();
		}


		[Test, SilverlightUnitTest]
		public void SettingColorOnNodeShouldSetColorOnChildren()
		{
			var parent = CreateNode();
			var child = CreateNode();
			parent.Children.Add(child);

			parent.Color = Color.FromSystemColor(Colors.Green);

			Assert.That(child.Color, Is.EqualTo(parent.Color));
		}

		[Test, SilverlightUnitTest]
		public void SettingColorOnNodeShouldSetColorOnEntireHierarchy()
		{
			var parent = CreateNode();
			var child = CreateNode();
			parent.Children.Add(child);
			var childOfChild = CreateNode();
			child.Children.Add(childOfChild);

			parent.Color = Color.FromSystemColor(Colors.Green);

			Assert.That(child.Color, Is.EqualTo(parent.Color));
			Assert.That(childOfChild.Color, Is.EqualTo(parent.Color));
		}

		[Test, SilverlightUnitTest]
		public void AddingAChildShouldAddToItems()
		{
			var parent = CreateNode();
			var child = CreateNode();

			parent.Children.Add(child);

			var contains = parent.Items.Contains(child);
			Assert.That(contains, Is.True);
		}

		[Test, SilverlightUnitTest]
		public void RemovingAChildShouldRemoveItFromItems()
		{
			var parent = CreateNode();
			var child = CreateNode();
			parent.Children.Add(child);

			parent.Children.Remove(child);

			var contains = parent.Items.Contains(child);
			Assert.That(contains, Is.False);
		}

		[Test, SilverlightUnitTest]
		public void ClearingChildrenShouldClearItems()
		{
			var parent = CreateNode();
			var child = CreateNode();
			parent.Children.Add(child);

			parent.Children.Clear();

			Assert.That(parent.Items.Count, Is.EqualTo(0));
		}
	}
}

