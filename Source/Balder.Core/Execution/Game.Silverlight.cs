using System;
using System.Windows;
using System.Windows.Media;
using Balder.Core.Display;
using Balder.Core.Silverlight.Input;
using Balder.Core.View;

namespace Balder.Core.Execution
{
	public partial class Game
	{
		private IDisplay _display;
		private bool _loaded = false;
		private NodeMouseEventHelper _nodeMouseEventHelper;

		partial void Constructed()
		{
			Loaded += GameLoaded;
		}


		public void Unload()
		{
			Runtime.Instance.UnregisterGame(this);
			_nodeMouseEventHelper.Dispose();
		}

		public static readonly Property<Game, Camera> CameraProp = Property<Game, Camera>.Register(g => g.Camera);
		public Camera Camera
		{
			get { return CameraProp.GetValue(this); }
			set
			{
				var previousCamera = Camera;
				if (null != previousCamera)
				{
					if (Children.Contains(previousCamera))
					{
						Children.Remove(previousCamera);
					}
				}
				CameraProp.SetValue(this, value);
				Viewport.View = value;

				value.Width = 0;
				value.Height = 0;
				value.Visibility = Visibility.Collapsed;

				Children.Add(value);
			}
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
			_nodeMouseEventHelper = new NodeMouseEventHelper(this, Viewport);
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
			_display = Runtime.Instance.Platform.DisplayDevice.CreateDisplay();
			_display.Initialize((int)Width, (int)Height);
			Runtime.Instance.RegisterGame(_display, this);
			_display.InitializeContainer(this);
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
					Scene.AddNode(element as INode);
				}
			}
		}

	}
}
