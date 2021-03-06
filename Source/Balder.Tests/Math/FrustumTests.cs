﻿#region License
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
using Balder.Math;
using Balder.Rendering;
using Balder.View;
using Balder.Testing;
using Moq;
using NUnit.Framework;

namespace Balder.Tests.Math
{
	[TestFixture]
	public class FrustumTests : TestFixture
	{
		private Frustum _frustum;
		private Camera _camera;

		[TestFixtureSetUp]
		public void Setup()
		{
			var runtimeContextMock = new Mock<IRuntimeContext>();
			var messenger = new MessengerContext();
			runtimeContextMock.Expect(r => r.MessengerContext).Returns(messenger);

				var viewport = new Viewport(runtimeContextMock.Object) { Width = 640, Height = 480 };
			_camera = new Camera() { Target = Vector.Forward, Position = Vector.Zero };
			viewport.View = _camera;
			
			_camera.Update(viewport);
			_frustum = new Frustum();
			_frustum.SetCameraDefinition(viewport, _camera);
		}

		[Test]
		public void VectorInsideShouldNotBeClipped()
		{
			var vectorToTest = new Vector(0f, 0f, (_camera.Far - _camera.Near)/2);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Inside));
		}

		[Test]
		public void VectorAboveTopShouldBeClipped()
		{
			var vectorToTest = new Vector(0f, 1000f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorBelowBottomShouldBeClipped()
		{
			var vectorToTest = new Vector(0f, -1000f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorLeftOfLeftShouldBeClipped()
		{
			var vectorToTest = new Vector(-1000f, 0f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorRightOfRightShouldBeClipped()
		{
			var vectorToTest = new Vector(1000f, 0f, 0.5f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorBehindNearShouldBeClipped()
		{
			var vectorToTest = new Vector(0f, 0f, _camera.Near-10f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void VectorBeyondFarShouldBeClipped()
		{
			var vectorToTest = new Vector(0f, 0f, _camera.Far+1f);
			var result = _frustum.IsPointInFrustum(vectorToTest);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Outside));
		}

		[Test]
		public void SphereInsideShouldNotBeClipped()
		{
			var vectorToTest = new Vector(0f, 0f, (_camera.Far - _camera.Near) / 2);
			var result = _frustum.IsSphereInFrustum(vectorToTest, 5);
			Assert.That(result, Is.EqualTo(FrustumIntersection.Inside));
		}
	}
}
