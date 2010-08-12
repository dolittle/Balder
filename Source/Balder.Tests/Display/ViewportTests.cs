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
using Balder.Math;
using Balder.Rendering;
using Balder.View;
using Moq;
using NUnit.Framework;

namespace Balder.Tests.Display
{
	[TestFixture]
	public class ViewportTests
	{
		public class SimpleNode : RenderableNode
		{
			public SimpleNode()
			{
				World = Matrix.Identity;
				RenderingWorld = Matrix.Identity;
			}
		}

		private static Viewport GetViewport()
		{
			
			var runtimeContextMock = new Mock<IRuntimeContext>();
			var nodeRenderingServiceMock = new Mock<INodeRenderingService>();
			var viewport = new Viewport(runtimeContextMock.Object);
			viewport.Width = 640;
			viewport.Height = 480;
			var scene = new Scene(runtimeContextMock.Object, nodeRenderingServiceMock.Object);

			var camera = new Camera();
			camera.Position.Set(0, 0, 0);
			camera.Target.Set(0, 0, 1);
			viewport.View = camera;
			camera.Update(viewport);

			viewport.Scene = scene;

			return viewport;
		}

		[Test]
		public void GettingNodeAtCenterWithViewInFrontShouldReturnTheNodeInCenter()
		{
			var viewport = GetViewport();
			viewport.View.Position.Set(0, 0, 0 - 10);

			var simpleNode = new SimpleNode();
			simpleNode.BoundingSphere = new BoundingSphere(new Vector(0,0,0), 10f);
			viewport.Scene.AddNode(simpleNode);

			var foundNode = viewport.GetNodeAtPosition(viewport.Width/2, viewport.Height/2);
			Assert.That(foundNode, Is.Not.Null);
			Assert.That(foundNode, Is.EqualTo(simpleNode));
		}

		[Test]
		public void GettingNodeAtCenterWithViewInFrontAndNoNodeIsInCenterShouldReturnNull()
		{
			var viewport = GetViewport();
			viewport.View.Position.Set(0, 0, 0 - 10);

			var simpleNode = new SimpleNode();
			simpleNode.BoundingSphere = new BoundingSphere(new Vector(0, 0, 0), 10f);
			simpleNode.RenderingWorld = Matrix.CreateTranslation(-100, 0, 0);
			viewport.Scene.AddNode(simpleNode);

			var foundNode = viewport.GetNodeAtPosition(viewport.Width / 2, viewport.Height / 2);
			Assert.That(foundNode, Is.Null);
		}

		[Test]
		public void GettingNodeWithoutAnyRenderableNodeInSceneShouldReturnNull()
		{
			var viewport = GetViewport();
			var foundNode = viewport.GetNodeAtPosition(viewport.Width / 2, viewport.Height / 2);
			Assert.That(foundNode, Is.Null);
		}

		[Test]
		public void GettingNodeAtCenterWithViewMovedToLeftShouldReturnTheNodeInCenter()
		{
			var viewport = GetViewport();
			viewport.View.Position.Set(-100, 0, 0 - 10);

			var simpleNode = new SimpleNode();
			simpleNode.BoundingSphere = new BoundingSphere(new Vector(0, 0, 0), 10f);
			viewport.Scene.AddNode(simpleNode);

			var foundNode = viewport.GetNodeAtPosition(viewport.Width / 2, viewport.Height / 2);
			Assert.That(foundNode, Is.Not.Null);
			Assert.That(foundNode, Is.EqualTo(simpleNode));
		}

		[Test]
		public void GettingNodeAtCenterWithViewInFrontAndMultipleNodesShouldGetTheClosestNode()
		{
			var viewport = GetViewport();
			viewport.View.Position.Set(-100, 0, 0 - 10);

			var farthestNode = new SimpleNode();
			farthestNode.BoundingSphere = new BoundingSphere(new Vector(0, 0, 0), 10f);
			farthestNode.RenderingWorld = Matrix.CreateTranslation(0, 0, 100);
			viewport.Scene.AddNode(farthestNode);

			var nearestNode = new SimpleNode();
			nearestNode.BoundingSphere = new BoundingSphere(new Vector(0, 0, 0), 10f);
			nearestNode.RenderingWorld = Matrix.CreateTranslation(0, 0, 0);
			viewport.Scene.AddNode(nearestNode);

			var foundNode = viewport.GetNodeAtPosition(viewport.Width / 2, viewport.Height / 2);
			Assert.That(foundNode, Is.Not.Null);
			Assert.That(foundNode, Is.EqualTo(nearestNode));
		}
	}
}