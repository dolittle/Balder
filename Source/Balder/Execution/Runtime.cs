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
using System.Collections.Generic;
using Balder.Assets;
using Balder.Collections;
using Balder.Content;
using Balder.Debug;
using Balder.Display;
#if(SILVERLIGHT)
using Balder.Execution.Silverlight;
#endif
using Ninject;

namespace Balder.Execution
{
	[Singleton]
	public class Runtime : IRuntime
	{
		private static IRuntime _instance;
		private static readonly object InstanceLockObject = new object();

		private readonly Dictionary<IDisplay, ActorCollection> _gamesPerDisplay;

		private bool _hasPlatformInitialized;
		private bool _hasPlatformLoaded;
		private bool _hasPlatformRun;

		public Runtime(IKernel kernel, 
					   IPlatform platform, 
					   IAssetLoaderService assetLoaderService, 
					   IContentManager contentManager)
		{
			Kernel = kernel;
			Platform = platform;
			_gamesPerDisplay = new Dictionary<IDisplay, ActorCollection>();
			assetLoaderService.Initialize();
			ContentManager = contentManager;
			InitializePlatformEventHandlers();
		}

		public static IRuntime Instance
		{
			get
			{
				lock (InstanceLockObject)
				{
					if (null == _instance)
					{
						_instance = GetRuntime();
					}
					return _instance;
				}
			}
		}

		private static IRuntime GetRuntime()
		{
			var kernel = new PlatformKernel(typeof(Platform));
			return kernel.Get<IRuntime>();
		}


		public IKernel Kernel { get; private set; }

		public IPlatform Platform { get; private set; }
		public IContentManager ContentManager { get; private set; }

		public DebugInfo DebugInfo { get; set; }


		public T CreateGame<T>() where T : Game
		{
			var game = Kernel.Get<T>();
			return game;
		}

		public Game CreateGame(Type type)
		{
			var game = Kernel.Get(type) as Game;
			return game;
		}


		public void RegisterGame(IDisplay display, Game game)
		{
			// Todo: Display can't be singleton - in case of multiple games in one application
			Kernel.Bind<IDisplay>().ToConstant(display);
			Kernel.Inject(game);
			WireUpGame(display, game);
			var actorCollection = GetGameCollectionForDisplay(display);
			actorCollection.Add(game);
			HandleEventsForActor(game);
		}

		public void UnregisterGame(Game game)
		{
			// Todo : This should be handled by removing runtime context when it is a 
			// fully qualified member of the system
			game.Viewport.Uninitialize();
			game.Uninitialize();

			var displaysToRemove = new List<IDisplay>();
			lock (_gamesPerDisplay)
			{
				
				foreach (var display in _gamesPerDisplay.Keys)
				{
					var gameFound = false;
					var actorCollection = _gamesPerDisplay[display];
					foreach (var actor in actorCollection)
					{
						if (actor is Game && actor.Equals(game))
						{
							gameFound = true;
							break;
						}
					}
					if (gameFound)
					{
						actorCollection.Remove(game);
						if (actorCollection.Count == 0)
						{
							displaysToRemove.Add(display);
						}
					}
				}
			}

			foreach (var display in displaysToRemove)
			{
				display.Uninitialize();
				_gamesPerDisplay.Remove(display);
				Platform.DisplayDevice.RemoveDisplay(display);
			}
		}


		private void WireUpGame(IDisplay display, Game objectToWire)
		{
			/*
			var scope = Kernel.CreateScope();
			var displayActivationContext = new DisplayActivationContext(display, objectToWire.GetType(), scope);
			Kernel.Inject(objectToWire, displayActivationContext);*/
		}

		private ActorCollection GetGameCollectionForDisplay(IDisplay display)
		{
			lock (_gamesPerDisplay)
			{

				ActorCollection actorCollection = null;
				if (_gamesPerDisplay.ContainsKey(display))
				{
					actorCollection = _gamesPerDisplay[display];
				}
				else
				{
					actorCollection = new ActorCollection();
					_gamesPerDisplay[display] = actorCollection;
				}
				return actorCollection;
			}
		}




		private void InitializePlatformEventHandlers()
		{
			Platform.StateChanged += PlatformStateChanged;
		}


		private void HandleEventsForGames()
		{
			lock (_gamesPerDisplay)
			{
				foreach (var games in _gamesPerDisplay.Values)
				{
					foreach (var game in games)
					{
						HandleEventsForActor(game);
					}
				}
			}
		}

		private void HandleEventsForActor<T>(T actor) where T : Actor
		{
			if (!actor.HasInitialized && HasPlatformInitialized)
			{
				actor.ChangeState(ActorState.Initialize);
			}
			if (!actor.HasLoaded && HasPlatformLoaded)
			{
				actor.ChangeState(ActorState.Load);
				actor.ChangeState(ActorState.Run);
			}
			if (!actor.HasUpdated &&
				HasPlatformRun &&
				actor.State == ActorState.Run)
			{
				actor.OnUpdate();
			}
		}

		private bool IsPlatformInStateOrLater(PlatformState state, ref bool field)
		{
			if (field)
			{
				return true;
			}
			if (Platform.CurrentState >= state)
			{
				return true;
			}
			return false;
		}

		private bool HasPlatformLoaded { get { return IsPlatformInStateOrLater(PlatformState.Load, ref _hasPlatformLoaded); } }
		private bool HasPlatformInitialized { get { return IsPlatformInStateOrLater(PlatformState.Initialize, ref _hasPlatformInitialized); } }
		private bool HasPlatformRun { get { return IsPlatformInStateOrLater(PlatformState.Run, ref _hasPlatformRun); } }


		private void PlatformStateChanged(IPlatform platform, PlatformState state)
		{
			switch (state)
			{
				case PlatformState.Initialize:
					{
						_hasPlatformInitialized = true;
					}
					break;
				case PlatformState.Load:
					{
						_hasPlatformLoaded = true;
					}
					break;
				case PlatformState.Run:
					{
						_hasPlatformRun = true;
					}
					break;
			}
			HandleEventsForGames();
		}

	}
}