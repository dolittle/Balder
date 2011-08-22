#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
using Balder.Debug;
using Balder.Display;
using Balder.Objects;
using Balder.Rendering;
using Balder.View;
using Microsoft.Practices.ServiceLocation;

#if(XAML)
using System.Windows;
using System.Windows.Controls;
#endif

#if(XAML)
using Balder.Input.Silverlight;
#endif

using Ninject;

namespace Balder.Execution
{
	public delegate void GameEventHandler(Game game);

	public class Game : Actor, IHaveRuntimeContext, IHavePropertyContainer
	{
		public IRuntimeContext RuntimeContext { get; private set; }

		public event GameEventHandler Update = (s) => { };
		public event GameEventHandler Initialize = (s) => { };
		public event GameEventHandler LoadContent = (s) => { };

#if(XAML)
		
		private NodeMouseEventHelper _nodeMouseEventHelper;
#endif

		public Game()
			: this(Runtime.Instance.Kernel.Get<IRuntimeContext>(),
			Runtime.Instance.Kernel.Get<INodeRenderingService>())
		{
		}

		public Game(IRuntimeContext runtimeContext, INodeRenderingService nodeRenderingService)
			: base(runtimeContext)
		{
			RuntimeContext = runtimeContext;
			Viewport = new Viewport(runtimeContext); 
			Scene = new Scene(runtimeContext, nodeRenderingService);
			Camera = new Camera();
			Constructed();
			PassiveRenderingMode = PassiveRenderingMode.FullDetail;

			PassiveRendering = Runtime.Instance.Platform.IsInDesignMode;
			runtimeContext.MessengerContext.SubscriptionsFor<UpdateMessage>().AddListener(this, UpdateAction);
		}
		
		public void Uninitialize()
		{
			RuntimeContext.MessengerContext.SubscriptionsFor<UpdateMessage>().RemoveListener(this, UpdateAction);
		}

		private void Constructed()
		{
#if(XAML)
			_programmaticChildren = new Grid();
			Children.Add(_programmaticChildren);

			Loaded += GameLoaded;
#endif
		}

#if(XAML)
		private bool _loaded = false;
		private Grid _programmaticChildren { get; set; }
		private bool _nodeAddedFromXaml;

		internal void AddChildFromProgrammaticApproach(INode node)
		{
			if( _nodeAddedFromXaml )
			{
				return;
			}
			if( node is UIElement )
			{
				_programmaticChildren.Children.Add(node as UIElement);
			}
		}

		internal void RemoveNodeFromProgrammaticApproach(INode node)
		{
			if( node is UIElement )
			{
				if( _programmaticChildren.Children.Contains(node as UIElement))
				{
					_programmaticChildren.Children.Remove(node as UIElement);
				}
			}
		}

		internal void ClearAllProgrammaticNodes()
		{
			_programmaticChildren.Children.Clear();
		}


		public void Unload()
		{
			Runtime.Instance.UnregisterGame(this);
#if(XAML)
			_nodeMouseEventHelper.Dispose();
#endif
		}

		private void GameLoaded(object sender, RoutedEventArgs e)
		{
			if( _loaded )
			{
				return;
			}

			_loaded = true;
			Validate();
			RegisterGame();
			AddNodesToScene();
			InitializeViewport();
#if(XAML)
			_nodeMouseEventHelper = new NodeMouseEventHelper(this, Viewport);
#endif
		}

		private void InitializeViewport()
		{
			Viewport.Width = (int)Width;
			Viewport.Height = (int)Height;
			// Todo: This should be injected - need to figure out how to do this properly!
			Viewport.Display = Display;
		}

		private void RegisterGame()
		{
#if(XAML)
			RuntimeContext.Display.InitializeContainer(this);
#endif
			RuntimeContext.Display.Initialize((int)Width, (int)Height);
			Runtime.Instance.RegisterGame(RuntimeContext.Display, this);
			if( null != Skybox )
			{
				RuntimeContext.Display.InitializeSkybox(Skybox);
			}
		}

		private void Validate()
		{
			if (0 == Width || Width.Equals(double.NaN) ||
				0 == Height || Height.Equals(double.NaN))
			{
				throw new ArgumentException("You need to specify Width and Height");
			}
		}

		private void AddNodesToScene()
		{
			foreach (var element in Children)
			{
				if (element is INode)
				{
					_nodeAddedFromXaml = true;
					Scene.AddNode(element as INode);
					_nodeAddedFromXaml = false;
				}
			}
		}

#endif
		public static readonly Property<Game, Camera> CameraProp = Property<Game, Camera>.Register(g => g.Camera);
		public Camera Camera
		{
			get { return CameraProp.GetValue(this); }
			set
			{
				var previousCamera = Camera;
#if(XAML)
				if (null != previousCamera)
				{
					if (Children.Contains(previousCamera))
					{
						Children.Remove(previousCamera);
					}
				}
				Children.Add(value);
				value.Width = 0;
				value.Height = 0;
				value.Visibility = Visibility.Collapsed;
#endif
				value.RuntimeContext = RuntimeContext;
				CameraProp.SetValue(this, value);
				Viewport.View = value;

			}
		}

		private Scene _scene;
		public Scene Scene
		{
			get { return _scene; }
			set
			{
				_scene = value;
				Viewport.Scene = value;
				value.Game = this;
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

		public Skybox Skybox
		{
			get { return Viewport.Skybox;  }
			set
			{
				Viewport.Skybox = value;
				RuntimeContext.Display.InitializeSkybox(value);
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
				RuntimeContext.PassiveRendering = value;
			}
		}

		private PassiveRenderingMode _passiveRenderingMode;
		public PassiveRenderingMode PassiveRenderingMode
		{
			get { return _passiveRenderingMode; }
			set
			{
				_passiveRenderingMode = value;
				RuntimeContext.PassiveRenderingMode = value;
			}
		}

		public static readonly Property<Game, bool> IsPausedProperty =
			Property<Game, bool>.Register(g => g.IsPaused);
		public bool IsPaused
		{
			get { return IsPausedProperty.GetValue(this); }
			set
			{
				IsPausedProperty.SetValue(this, value);

				// Todo: Figure out a better way to pause/halt everything - hate to have this value floating around everywhere
				Scene.IsPaused = value;
				RuntimeContext.Paused = value;
				if( null != Display )
				{
					Display.Halted = value;
				}
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
			OnUpdateOccured();
			Update(this);
		}

		IPropertyContainer _propertyContainer;
		public IPropertyContainer PropertyContainer
		{
			get
			{
				if (_propertyContainer == null)
					_propertyContainer = new PropertyContainer(this);

				return _propertyContainer;
			}
		}

	}
}