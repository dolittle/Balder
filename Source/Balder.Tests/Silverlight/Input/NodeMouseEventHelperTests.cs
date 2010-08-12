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

using Balder.Display;
using Balder.Execution;
using Balder.Objects.Geometries;
using Balder.Rendering;
using Balder.Input.Silverlight;
using CThru.Silverlight;
using Moq;
using NUnit.Framework;

namespace Balder.Tests.Silverlight.Input
{
	[TestFixture]
	public class NodeMouseEventHelperTests
	{
		private class FakeViewport : Viewport
		{
			public FakeViewport(IRuntimeContext runtimeContext)
				: base(runtimeContext)
			{
				
			}

			public RenderableNode NodeToReturn;
			public override RenderableNode GetNodeAtPosition(int x, int y)
			{
				return NodeToReturn;
			}
		}

		private void CreateMocks(out Game game, out Scene scene, out FakeViewport viewport)
		{
			var runtimeContextMock = new Mock<IRuntimeContext>();
			var nodeRenderingServiceMock = new Mock<INodeRenderingService>();
			
			game = new Game(runtimeContextMock.Object, nodeRenderingServiceMock.Object);
			scene = new Scene(runtimeContextMock.Object, nodeRenderingServiceMock.Object);
			viewport = new FakeViewport(runtimeContextMock.Object) { Scene = scene };
			game.Scene = scene;
		}

		[Test]
		public void MouseOverNodeShouldFireMouseMoveEvent()
		{
			Game game;
			Scene scene;
			FakeViewport viewport;

			CreateMocks(out game, out scene, out viewport);

			var geometry = new Geometry();
			scene.AddNode(geometry);
			viewport.NodeToReturn = geometry;

			var mouseMoved = false;
			geometry.MouseMove += (s, e) => mouseMoved = true;

			var helper = new NodeMouseEventHelper(game, viewport);
			helper.HandleMouseMove(0,0,null);

			Assert.That(mouseMoved,Is.True);
		}

		[Test,SilverlightUnitTest]
		public void FirstMouseOverShouldFireMouseEnterEvent()
		{
			Game game;
			Scene scene;
			FakeViewport viewport;

			CreateMocks(out game, out scene, out viewport);
			var geometry = new Geometry();
			scene.AddNode(geometry);
			viewport.NodeToReturn = geometry;

			var mouseEnter = false;
			geometry.MouseEnter += (s, e) => mouseEnter = true;

			var helper = new NodeMouseEventHelper(game,viewport);
			helper.HandleMouseMove(0, 0, null);

			Assert.That(mouseEnter, Is.True);
		}

		[Test, SilverlightUnitTest]
		public void SubsequentMouseOverShouldNotFireMouseEnterEvent()
		{
			Game game;
			Scene scene;
			FakeViewport viewport;

			CreateMocks(out game, out scene, out viewport);
			var geometry = new Geometry();
			scene.AddNode(geometry);
			viewport.NodeToReturn = geometry;

			var mouseEnter = false;
			geometry.MouseEnter += (s, e) => mouseEnter = true;

			var helper = new NodeMouseEventHelper(game,viewport);
			helper.HandleMouseMove(0, 0, null);
			mouseEnter = false;
			helper.HandleMouseMove(0, 0, null);

			Assert.That(mouseEnter, Is.False);
		}

		[Test, SilverlightUnitTest]
		public void MouseHoveringOverAndThenNotOverShouldFireMouseLeaveEvent()
		{
			Game game;
			Scene scene;
			FakeViewport viewport;

			CreateMocks(out game, out scene, out viewport);
			var geometry = new Geometry();
			scene.AddNode(geometry);
			viewport.NodeToReturn = geometry;

			var mouseLeave = false;
			geometry.MouseLeave += (s, e) => mouseLeave = true;
			var helper = new NodeMouseEventHelper(game, viewport);
			helper.HandleMouseMove(0, 0, null);
			helper.HandleMouseMove(0, 0, null);
			viewport.NodeToReturn = null;
			helper.HandleMouseMove(0, 0, null);
			
			Assert.That(mouseLeave, Is.True);
		}

		[Test, SilverlightUnitTest]
		public void MouseNotHoveringOverAnythingShouldNotFireAnyEvents()
		{
			Game game;
			Scene scene;
			FakeViewport viewport;

			CreateMocks(out game, out scene, out viewport);
			var geometry = new Geometry();
			scene.AddNode(geometry);

			var mouseMoved = false;
			var mouseEnter = false;
			var mouseLeave = false;

			geometry.MouseMove += (s, e) => mouseMoved = true;
			geometry.MouseEnter += (s, e) => mouseEnter = true;
			geometry.MouseLeave += (s, e) => mouseLeave = true;

			var helper = new NodeMouseEventHelper(game, viewport);
			helper.HandleMouseMove(0, 0, null);

			Assert.That(mouseMoved,Is.False);
			Assert.That(mouseEnter,Is.False);
			Assert.That(mouseLeave, Is.False);
		}

		[Test, SilverlightUnitTest]
		public void MouseEnteringAndLeavingAndThenEnteringShouldFireMouseEnter()
		{
			Game game;
			Scene scene;
			FakeViewport viewport;

			CreateMocks(out game, out scene, out viewport);
			var geometry = new Geometry();
			scene.AddNode(geometry);
			viewport.NodeToReturn = geometry;

			var mouseEnter = false;

			geometry.MouseEnter += (s, e) => mouseEnter = true;

			var helper = new NodeMouseEventHelper(game, viewport);
			helper.HandleMouseMove(0, 0, null);
			viewport.NodeToReturn = null;
			helper.HandleMouseMove(0, 0, null);
			mouseEnter = false;
			viewport.NodeToReturn = geometry;
			

			helper.HandleMouseMove(0, 0, null);
			Assert.That(mouseEnter, Is.True);
		}
		
	}
}

