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
using Balder.Core.Debug;
using Balder.Core.Display;
using Balder.Core.Math;
using Balder.Core.View;

namespace Balder.Core.Execution
{
	public delegate void GameEventHandler(Game game);

	public partial class Game : Actor
	{
		private static readonly EventArgs DefaultEventArgs = new EventArgs();

		public event GameEventHandler Update = (s) => { };
		public event GameEventHandler Initialize = (s) => { };
		public event GameEventHandler LoadContent = (s) => { };

		public Game()
		{
			Viewport = new Viewport { Width = 800, Height = 600 };
			Scene = new Scene();
			Camera = new Camera() { Target = Vector.Forward, Position = Vector.Zero };
			Constructed();
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

		public Viewport Viewport { get; private set; }
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
				HandlePassiveRendering();
			}
		}

		private PassiveRenderingMode _passiveRenderingMode;
		public PassiveRenderingMode PassiveRenderingMode
		{
			get { return _passiveRenderingMode; }
			set
			{
				_passiveRenderingMode = value;
				HandlePassiveRendering();
			}
		}

		private void HandlePassiveRendering()
		{
			if( null == Display )
			{
				return;
			}
			if (_passiveRendering)
			{
				Display.EnablePassiveRendering();
			}
			else
			{
				Display.EnableActiveRendering();
			}
			Display.SetPassiveRenderingMode(_passiveRenderingMode);
		}

		public override void OnInitialize()
		{
			// Todo: Figure out a better way to inject this dependency
			// Plus, this is really fragile - when overridden and base class is not called, this won't work.
			// it affects both Passive Rendering and mouse handling, really bad..
			Viewport.Display = Display;
			HandlePassiveRendering();


			Initialize(this);
		}

		public override void OnLoadContent()
		{
			LoadContent(this);
		}

		public override void OnUpdate()
		{
			Update(this);
		}

		public virtual void OnRender()
		{
			Camera.Update(Viewport);
			Viewport.Render();
		}

		public virtual void OnPrepare()
		{
			Viewport.Prepare();
		}
	}
}