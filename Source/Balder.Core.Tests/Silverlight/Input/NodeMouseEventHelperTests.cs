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

using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Objects.Geometries;
using Balder.Core.Silverlight.Input;
using CThru.Silverlight;
using NUnit.Framework;

namespace Balder.Core.Tests.Silverlight.Input
{
	[TestFixture]
	public class NodeMouseEventHelperTests
	{
		private class FakeViewport : Viewport
		{
			public RenderableNode NodeToReturn;
			public override RenderableNode GetNodeAtScreenCoordinate(int x, int y)
			{
				return NodeToReturn;
			}
		}

		[Test,SilverlightUnitTest]
		public void MouseOverNodeShouldFireMouseMoveEvent()
		{
			var game = new Game();
			var scene = new Scene();
			var viewport = new FakeViewport { Scene = scene };
			game.Scene = scene;

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
			var game = new Game();
			var scene = new Scene();
			var viewport = new FakeViewport { Scene = scene };
			game.Scene = scene;
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
			var game = new Game();
			var scene = new Scene();
			var viewport = new FakeViewport { Scene = scene };
			game.Scene = scene;
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
			var game = new Game();
			var scene = new Scene();
			var viewport = new FakeViewport { Scene = scene };
			game.Scene = scene;
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
			var game = new Game();
			var scene = new Scene();
			var viewport = new FakeViewport { Scene = scene };
			game.Scene = scene;
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
			var game = new Game();
			var scene = new Scene();
			var viewport = new FakeViewport { Scene = scene };
			game.Scene = scene;
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

