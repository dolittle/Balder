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

using Balder;
using Balder.Materials;
using Balder.Silverlight.Rendering;
using CThru.Silverlight;
using Moq;
using NUnit.Framework;

namespace Balder.Silverlight.Tests.Rendering
{
	[TestFixture]
	public class MetaDataPixelBufferTests
	{
		[Test, SilverlightUnitTest]
		public void GettingNodeIdentifierForTheFirstTimeShouldReturnOneAsIdentifier()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);
			var nodeMock = new Mock<INode>();
			manager.Reset();

			var identifier = manager.GetIdentifier(nodeMock.Object);

			Assert.That(identifier, Is.EqualTo(1));
		}

		[Test, SilverlightUnitTest]
		public void GettingNodeIdentifierMultipleTimesForSameNodeShouldReturnSameIdentifier()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);
			var nodeMock = new Mock<INode>();
			manager.Reset();

			var identifier = manager.GetIdentifier(nodeMock.Object);
			var secondIdentifier = manager.GetIdentifier(nodeMock.Object);

			Assert.That(secondIdentifier, Is.EqualTo(identifier));
		}

		[Test, SilverlightUnitTest]
		public void GettingNodeIdentifierForMultipleNodesShouldReturnUniqueIdentifiers()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);
			var firstNodeMock = new Mock<INode>();
			firstNodeMock.Expect(n => n.Id).Returns(1);
			var secondNodeMock = new Mock<INode>();
			secondNodeMock.Expect(n => n.Id).Returns(2);
			manager.Reset();

			var firstIdentifier = manager.GetIdentifier(firstNodeMock.Object);
			var secondIdentifier = manager.GetIdentifier(secondNodeMock.Object);

			Assert.That(secondIdentifier, Is.Not.EqualTo(firstIdentifier));
		}

		[Test, SilverlightUnitTest]
		public void ResettingShouldRestartIdentifierCounter()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);
			var firstNodeMock = new Mock<INode>();
			var secondNodeMock = new Mock<INode>();
			var thirdNodeMock = new Mock<INode>();
			manager.Reset();

			manager.GetIdentifier(firstNodeMock.Object);
			manager.GetIdentifier(secondNodeMock.Object);

			manager.Reset();

			var identifier = manager.GetIdentifier(thirdNodeMock.Object);
			Assert.That(identifier, Is.EqualTo(1));
		}

		[Test, SilverlightUnitTest]
		public void SettingNodeAtPositionShouldReturnSameNodeWhenGetting()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);

			var firstNodeMock = new Mock<INode>();
			var secondNodeMock = new Mock<INode>();

			manager.SetNodeAtPosition(firstNodeMock.Object, 0, 0);
			manager.SetNodeAtPosition(secondNodeMock.Object, 0, 1);

			var node = manager.GetNodeAtPosition(0, 0);

			Assert.That(node, Is.EqualTo(firstNodeMock.Object));
		}

		[Test, SilverlightUnitTest]
		public void GettingNodeAtPositionAfterNewFrameAndNodeNotInFrameShouldReturnNull()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);

			var nodeMock = new Mock<INode>();
			manager.SetNodeAtPosition(nodeMock.Object, 0, 0);
			manager.NewFrame();

			var node = manager.GetNodeAtPosition(0, 0);
			Assert.That(node, Is.Null);
		}


		[Test, SilverlightUnitTest]
		public void GettingNodeAtPositionOutsideTheScreenShouldReturnNull()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);

			var nodeMock = new Mock<INode>();
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
				var manager = new MetaDataPixelBuffer();
				manager.Initialize(640, 480);
				var nodeMock = new Mock<INode>();

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
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);
			var materialMock = new Mock<Material>();

			var firstNodeMock = new Mock<INode>();
			var secondNodeMock = new Mock<INode>();

			manager.SetNodeAtPosition(firstNodeMock.Object, materialMock.Object, 0, 0);
			manager.SetNodeAtPosition(secondNodeMock.Object, 0, 1);

			var actualMaterial = manager.GetMaterialAtPosition(0, 0);

			Assert.That(actualMaterial, Is.EqualTo(materialMock.Object));
		}

		[Test, SilverlightUnitTest]
		public void GettingMaterialAtPositionOutsideTheScreenShouldReturnNull()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);

			var materialMock = new Mock<Material>();
			var nodeMock = new Mock<INode>();
			manager.SetNodeAtPosition(nodeMock.Object, materialMock.Object, 0, 0);

			var actualMaterial = manager.GetMaterialAtPosition(-1, -1);
			Assert.That(actualMaterial, Is.Null);
			actualMaterial = manager.GetMaterialAtPosition(641, 481);
			Assert.That(actualMaterial, Is.Null);
		}

		[Test,SilverlightUnitTest]
		public void SettingNodeWithFaceShouldReturnSameFaceWhenGetting()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);

			var face = new RenderFace(0, 1, 2) { Index = 1 };

			var firstNodeMock = new Mock<INode>();
			var secondNodeMock = new Mock<INode>();

			manager.SetNodeAtPosition(firstNodeMock.Object, face, 0, 0);
			manager.SetNodeAtPosition(secondNodeMock.Object, 0, 1);

			var actualFace = manager.GetRenderFaceAtPosition(0, 0);

			Assert.That(actualFace, Is.EqualTo(face));
		}

		[Test, SilverlightUnitTest]
		public void GettingRenderFaceAtPositionOutsideTheScreenShouldReturnNull()
		{
			var manager = new MetaDataPixelBuffer();
			manager.Initialize(640, 480);

			var face = new RenderFace(0, 1, 2) { Index = 1 };
			var nodeMock = new Mock<INode>();
			manager.SetNodeAtPosition(nodeMock.Object, face, 0, 0);
			var actualFace = manager.GetRenderFaceAtPosition(-1, -1);
			Assert.That(actualFace, Is.Null);
			actualFace = manager.GetRenderFaceAtPosition(641, 481);
			Assert.That(actualFace, Is.Null);
		}
	}
}

