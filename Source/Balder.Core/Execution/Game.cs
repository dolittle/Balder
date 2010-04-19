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
using Balder.Core.Debug;
using Balder.Core.Display;
using Balder.Core.Math;
using Balder.Core.Rendering;
using Balder.Core.View;

namespace Balder.Core.Execution
{
	public delegate void GameEventHandler(Game game);

	public partial class Game : Actor
	{
		private readonly RuntimeContext _runtimeContext;

		public event GameEventHandler Update = (s) => { };
		public event GameEventHandler Initialize = (s) => { };
		public event GameEventHandler LoadContent = (s) => { };

		public Game()
			: this(Runtime.Instance.Kernel.Get<RuntimeContext>())
		{
		}

		public Game(RuntimeContext runtimeContext)
		{
			_runtimeContext = runtimeContext;
			Viewport = new Viewport { Width = 800, Height = 600 };
			Scene = new Scene();
			Camera = new Camera() { Target = Vector.Forward, Position = Vector.Zero };
			Constructed();

			Messenger.DefaultContext.SubscriptionsFor<UpdateMessage>().AddListener(this, UpdateAction);
		}

		public void Uninitialize()
		{
			Messenger.DefaultContext.SubscriptionsFor<UpdateMessage>().RemoveListener(this, UpdateAction);
		}

		partial void Constructed();

		private Scene _scene;
		public Scene Scene
		{
			get { return _scene; }
			set
			{
				_scene = value;
				Viewport.Scene = value;
			}
		}

		private Viewport _viewport;
		public Viewport Viewport
		{
			get { return _viewport; }
			private set
			{
				_viewport = value;
				// Todo: This should be injected - need to figure out how to do this properly!
				Viewport.Display = Display;
			}
		}

		public DebugInfo DebugInfo
		{
			get { return Viewport.DebugInfo; }
			set { Viewport.DebugInfo = value; }
		}

		private bool _passiveRendering;
		public bool PassiveRendering
		{
			get { return _passiveRendering; }
			set
			{
				_passiveRendering = value;
				_runtimeContext.PassiveRendering = true;
			}
		}

		private PassiveRenderingMode _passiveRenderingMode;
		public PassiveRenderingMode PassiveRenderingMode
		{
			get { return _passiveRenderingMode; }
			set
			{
				_passiveRenderingMode = value;
				_runtimeContext.PassiveRenderingMode = value;
			}
		}


		public override void OnLoadContent()
		{
			LoadContent(this);
		}

		public override void OnInitialize()
		{
			Initialize(this);
			base.OnInitialize();
		}

		private void UpdateAction(UpdateMessage updateMessage)
		{
			Update(this);
		}
	}
}