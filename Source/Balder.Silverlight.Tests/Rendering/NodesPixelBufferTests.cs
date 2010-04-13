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

using Balder.Core;
using Balder.Core.Materials;
using Balder.Silverlight.Rendering;
using CThru.Silverlight;
using Moq;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Rendering
{
	[TestFixture]
	public class NodesPixelBufferTests
	{
		[Test, SilverlightUnitTest]
		public void GettingNodeIdentifierForTheFirstTimeShouldReturnOneAsIdentifier()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);
			var nodeMock = new Mock<Node>();
			manager.Reset();

			var identifier = manager.GetNodeIdentifier(nodeMock.Object);

			Assert.That(identifier, Is.EqualTo(1));
		}

		[Test, SilverlightUnitTest]
		public void GettingNodeIdentifierMultipleTimesForSameNodeShouldReturnSameIdentifier()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);
			var nodeMock = new Mock<Node>();
			manager.Reset();

			var identifier = manager.GetNodeIdentifier(nodeMock.Object);
			var secondIdentifier = manager.GetNodeIdentifier(nodeMock.Object);

			Assert.That(secondIdentifier, Is.EqualTo(identifier));
		}

		[Test, SilverlightUnitTest]
		public void GettingNodeIdentifierForMultipleNodesShouldReturnUniqueIdentifiers()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);
			var firstNodeMock = new Mock<Node>();
			var secondNodeMock = new Mock<Node>();
			manager.Reset();

			var firstIdentifier = manager.GetNodeIdentifier(firstNodeMock.Object);
			var secondIdentifier = manager.GetNodeIdentifier(secondNodeMock.Object);

			Assert.That(secondIdentifier, Is.Not.EqualTo(firstIdentifier));
		}

		[Test, SilverlightUnitTest]
		public void ResettingShouldRestartIdentifierCounter()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);
			var firstNodeMock = new Mock<Node>();
			var secondNodeMock = new Mock<Node>();
			var thirdNodeMock = new Mock<Node>();
			manager.Reset();

			manager.GetNodeIdentifier(firstNodeMock.Object);
			manager.GetNodeIdentifier(secondNodeMock.Object);

			manager.Reset();

			var identifier = manager.GetNodeIdentifier(thirdNodeMock.Object);
			Assert.That(identifier, Is.EqualTo(1));
		}

		[Test, SilverlightUnitTest]
		public void SettingNodeAtPositionShouldReturnSameNodeWhenGetting()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);

			var firstNodeMock = new Mock<Node>();
			var secondNodeMock = new Mock<Node>();

			manager.SetNodeAtPosition(firstNodeMock.Object, 0, 0);
			manager.SetNodeAtPosition(secondNodeMock.Object, 0, 1);

			var node = manager.GetNodeAtPosition(0, 0);

			Assert.That(node, Is.EqualTo(firstNodeMock.Object));
		}

		[Test, SilverlightUnitTest]
		public void GettingNodeAtPositionAfterNewFrameAndNodeNotInFrameShouldReturnNull()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);

			var nodeMock = new Mock<Node>();
			manager.SetNodeAtPosition(nodeMock.Object, 0, 0);
			manager.NewFrame();

			var node = manager.GetNodeAtPosition(0, 0);
			Assert.That(node, Is.Null);
		}


		[Test, SilverlightUnitTest]
		public void GettingNodeAtPositionOutsideTheScreenShouldReturnNull()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);

			var nodeMock = new Mock<Node>();
			manager.SetNodeAtPosition(nodeMock.Object, 0, 0);

			var node = manager.GetNodeAtPosition(-1, -1);
			Assert.That(node, Is.Null);
			node = manager.GetNodeAtPosition(641, 481);
			Assert.That(node, Is.Null);
		}

		[Test, SilverlightUnitTest]
		public void SettingNodeAtPositionOutsideTheScreenShouldNotCauseAnException()
		{
			try
			{
				var manager = new NodesPixelBuffer();
				manager.Initialize(640, 480);

				var nodeMock = new Mock<Node>();

				var hashCode = nodeMock.Object.GetHashCode();

				manager.SetNodeAtPosition(nodeMock.Object, -1, -1);
				manager.SetNodeAtPosition(nodeMock.Object, 641, 481);
			}
			catch
			{
				Assert.Fail();
			}
		}

		[Test, SilverlightUnitTest]
		public void SettingNodeWithMaterialShouldReturnSameMaterialWhenGetting()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);
			var material = new Material();

			var firstNodeMock = new Mock<Node>();
			var secondNodeMock = new Mock<Node>();

			manager.SetNodeAtPosition(firstNodeMock.Object, material, 0, 0);
			manager.SetNodeAtPosition(secondNodeMock.Object, 0, 1);

			var actualMaterial = manager.GetMaterialAtPosition(0, 0);

			Assert.That(actualMaterial, Is.EqualTo(material));
		}

		[Test, SilverlightUnitTest]
		public void GettingMaterialAtPositionOutsideTheScreenShouldReturnNull()
		{
			var manager = new NodesPixelBuffer();
			manager.Initialize(640, 480);

			var material = new Material();
			var nodeMock = new Mock<Node>();
			manager.SetNodeAtPosition(nodeMock.Object, material, 0, 0);

			var actualMaterial = manager.GetMaterialAtPosition(-1, -1);
			Assert.That(actualMaterial, Is.Null);
			material = manager.GetMaterialAtPosition(641, 481);
			Assert.That(actualMaterial, Is.Null);
		}

	}
}

