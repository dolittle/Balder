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
#if(SILVERLIGHT)
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Objects;
using Balder.Rendering;
using Balder.Rendering.Silverlight5;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Silverlight;

namespace Balder.Display.Silverlight5
{
	public class Display : IDisplay
	{
		private readonly IRuntimeContext _runtimeContext;
		private readonly IRenderingManager _renderingManager;
		private UpdateMessage _updateMessage;
		private RenderMessage _renderMessage;
		private PrepareMessage _prepareMessage;

		private Grid _container;
		private DrawingSurface _drawingSurface;
		private Microsoft.Xna.Framework.Color _actualBackgroundColor;

		public static GraphicsDevice GraphicsDevice { get { return GraphicsDeviceManager.Current.GraphicsDevice; } } 

		public Display(IRuntimeContext runtimeContext, IRenderingManager renderingManager)
		{
			_runtimeContext = runtimeContext;
			_renderingManager = renderingManager;

			_renderMessage = new RenderMessage();
			_prepareMessage = new PrepareMessage();
			_updateMessage = new UpdateMessage();

		}


		private Color _backgroundColor;
		public Color BackgroundColor
		{
			get { return _backgroundColor; }
			set
			{
				_backgroundColor = value;
				_actualBackgroundColor = new Microsoft.Xna.Framework.Color(value.Red, value.Green, value.Blue, value.Alpha);
			}
		}

		public bool ClearEnabled { get; set; }
		public bool Paused { get; set; }
		public bool Halted { get; set; }

		public void Initialize(int width, int height)
		{
			if (_container != null)
			{
				_drawingSurface = new DrawingSurface();
				_drawingSurface.Width = width;
				_drawingSurface.Height = height;
				_drawingSurface.Draw += Draw;
				_container.Background = new SolidColorBrush(new System.Windows.Media.Color {A = 0x1, R = 0, G = 0, B = 0});
				_container.Children.Add(_drawingSurface);

				CompositionTarget.Rendering += CompositionTargetRendering;

				_renderingManager.Initialize();
			}
		}


		void CompositionTargetRendering(object sender, System.EventArgs e)
		{
			_runtimeContext.MessengerContext.Send(_prepareMessage);
			_runtimeContext.MessengerContext.Send(_renderMessage);
			_runtimeContext.MessengerContext.Send(_updateMessage);
		}

		public void Uninitialize()
		{
		}

		public void InitializeContainer(object container)
		{
			if (container is Grid)
			{
				_container = container as Grid;
			}
		}

		void Draw(object sender, DrawEventArgs e)
		{
			e.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, _actualBackgroundColor, 1.0f, 0);
			_renderingManager.Render(e.GraphicsDevice);
			e.InvalidateSurface();
		}

		public void InitializeSkybox(Skybox skybox)
		{
		}

		public int[] GetCurrentFrame()
		{
			return null;
		}
	}
}
#endif