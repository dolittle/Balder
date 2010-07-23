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
using System;
using System.Linq.Expressions;
using Balder.Core.Assets;
using Balder.Core.Display;
using Balder.Core.Tests.Fakes;
using Balder.Testing;
using Moq;
using Ninject;
using NUnit.Framework;
using Balder.Core.Execution;

namespace Balder.Core.Tests
{
	[TestFixture]
	public class RuntimeTests : TestFixture
	{
		private static void EventShouldBeCalledForStateDuringRegistration(Expression<Action<Game>> eventExpression, PlatformState state, bool changeStateFirst)
		{
			var eventCalled = false;
			var stateChanged = false;

			var gameMock = new Mock<Game>();
			gameMock.Expect(g => g.OnInitialize());
			gameMock.Expect(g => g.OnBeforeInitialize());
			gameMock.Expect(g => g.OnLoadContent());
			gameMock.Expect(g => g.OnBeforeUpdate());
			gameMock.Expect(g => g.OnUpdate());
			gameMock.Expect(g => g.OnAfterUpdate());
			gameMock.Expect(g => g.OnStopped());
			gameMock.Expect(eventExpression).Callback(

				() =>
				{
					Assert.That(stateChanged, Is.True);
					eventCalled = true;
				});

			var assetLoaderServiceMock = new Mock<IAssetLoaderService>();
			var kernel = new PlatformKernel(typeof(FakePlatform));

			var platform = kernel.Get<IPlatform>() as FakePlatform;
			platform.StateChanged +=
				(p, s) =>
				{
					if (s == state)
					{
						stateChanged = true;
					}
				};

			var runtime = new Runtime(kernel, platform, assetLoaderServiceMock.Object, null);

			if (changeStateFirst)
			{
				platform.ChangeState(state);
			}

			var displayMock = new Mock<IDisplay>();
			runtime.RegisterGame(displayMock.Object, gameMock.Object);

			if (!changeStateFirst)
			{
				platform.ChangeState(state);
			}

			Assert.That(eventCalled, Is.True);
		}

		[Test]
		public void RegisteredGameShouldHaveItsInitializeCalledAfterInitializeStateChangeOccursOnPlatform()
		{
			EventShouldBeCalledForStateDuringRegistration(g => g.OnInitialize(), PlatformState.Initialize, false);
		}

		[Test]
		public void GameRegisteredAfterInitializeStateChangeOccuredOnPlatformShouldHaveItsInitializeEventCalledDirectly()
		{
			EventShouldBeCalledForStateDuringRegistration(g => g.OnInitialize(), PlatformState.Initialize, true);
		}

		[Test]
		public void RegisteredGameShouldHaveItsLoadCalledAfterLoadStateChangeOccursOnPlatform()
		{
			EventShouldBeCalledForStateDuringRegistration(g => g.OnLoadContent(), PlatformState.Load, false);
		}

		[Test]
		public void GameRegisteredAfterLoadStateChangeOccuredOnPlatformShouldHaveItsLoadEventCalledDirectly()
		{
			EventShouldBeCalledForStateDuringRegistration(g => g.OnLoadContent(), PlatformState.Load, true);
		}


		/*
		[Test,SilverlightUnitTest]
		public void ActorsWithinGameShouldHaveItsInitializeCalledAfterGamesInitializeIsCalled()
		{
			Assert.Fail();
		}

		[Test,SilverlightUnitTest]
		public void ActorsRegisteredInGameAfterGameHasStartedRunningShouldHaveItsInitializeCalled()
		{
			Assert.Fail();
		}

		[Test,SilverlightUnitTest]
		public void ActorsRegisteredInGameAfterGameHasStartedRunningShouldHaveItsLoadCalled()
		{
			Assert.Fail();
		}
		 * */
	}
}