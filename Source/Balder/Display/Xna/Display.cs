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
#if(XNA)
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
#if(XAML)
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Media;
#endif


namespace Balder.Display.Xna
{
	public class Display : IDisplay
    {
    	public static XnaWindow Window;
		internal static GraphicsDevice GraphicsDevice;
		private readonly IRuntimeContext _runtimeContext;

        private static IntPtr _windowHandle;
        private static GraphicsAdapter _graphicsAdapter;
        private static PresentationParameters _presentationParameters;

        public Color BackgroundColor { get; set; }
        public bool ClearEnabled { get; set; }
        public bool Paused { get; set; }
        public bool Halted { get; set; }

#if(XAML)
		Grid _container;
		Image _image;

		DispatcherTimer _timer;

		WriteableBitmap _writeableBitmap;
		RenderTarget2D _renderTarget;
		int[] _pixels;
#endif

        public Display(IRuntimeContext runtimeContext)
        {
            _runtimeContext = runtimeContext;
			Initialize();
        }


		void Initialize()
		{
			_graphicsAdapter = GraphicsAdapter.DefaultAdapter;
			_presentationParameters = new PresentationParameters();

			Window = new XnaWindow();
#if(!XAML)
        	Window.Render += WindowRender;
#endif
			_windowHandle = Window.Handle;

			_presentationParameters.BackBufferWidth = 800;
			_presentationParameters.BackBufferHeight = 600;
			_presentationParameters.DepthStencilFormat = DepthFormat.Depth24;
			_presentationParameters.BackBufferFormat = SurfaceFormat.Color;
			_presentationParameters.DeviceWindowHandle = _windowHandle;
			_presentationParameters.PresentationInterval = PresentInterval.Immediate;
			_presentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;

			_presentationParameters.IsFullScreen = false;

			GraphicsDevice = new GraphicsDevice(_graphicsAdapter, GraphicsProfile.Reach, _presentationParameters);

			GraphicsDevice.DepthStencilState = new DepthStencilState
			{
				DepthBufferEnable = true,
				DepthBufferFunction = CompareFunction.Less,
				DepthBufferWriteEnable = true
			};

			GraphicsDevice.Viewport = new Microsoft.Xna.Framework.Graphics.Viewport(0, 0, 800, 600);

			//GraphicsDevice.BlendState = BlendState.Opaque;

			_spriteBatch = new SpriteBatch(GraphicsDevice);
			var texture = File.OpenRead(@"C:\Projects\Balder\Source\Balder.Silverlight.SampleBrowser\Assets\BalderLogo.png");
			_texture = Texture2D.FromStream(GraphicsDevice, texture);

#if(XAML)
			_writeableBitmap = new WriteableBitmap(800, 600, 72, 72, PixelFormats.Bgr32, BitmapPalettes.WebPalette);
			_renderTarget = new RenderTarget2D(GraphicsDevice, 800, 600, false, SurfaceFormat.Rg32, DepthFormat.Depth16);
			_pixels = new int[800*600];
#endif
			
		}

#if(XAML)
		public void InitializeContainer(object container)
		{
			if (container is Grid)
			{
				_container = container as Grid;

				_image = new Image
				{
					Stretch = Stretch.None,
					Source = _writeableBitmap
				};
				
				_container.Children.Add(_image);

				CompositionTarget.Rendering += Render;
			}
		}

		static Int32Rect _fullScreenRect = new Int32Rect(0, 0, 800, 600);

		private void CopyFromRenderTargetToWriteableBitmap()
		{
			_renderTarget.GetData(_pixels);
			_writeableBitmap.Lock();
			_writeableBitmap.WritePixels(_fullScreenRect, _pixels, 800*4,0);
			_writeableBitmap.AddDirtyRect(_fullScreenRect);
			_writeableBitmap.Unlock();
		}


#endif

    	private SpriteBatch _spriteBatch;
    	private Texture2D _texture;

    	private double sin;

#if(XAML)
		void Render(object sender, EventArgs e)
#else
		private void WindowRender(object sender, PaintEventArgs e)
#endif
		{
			//if (ClearEnabled)
			{
				GraphicsDevice.Clear(
					ClearOptions.Target | ClearOptions.DepthBuffer,
					Microsoft.Xna.Framework.Color.AliceBlue,
					//new Microsoft.Xna.Framework.Color(BackgroundColor.Red, BackgroundColor.Green, BackgroundColor.Blue, 0xff),
					1f,
					0);
			}

			var size = (int) ((System.Math.Sin(sin)*128) + 128);
			
			_spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
			_spriteBatch.Draw(_texture, new Rectangle(0,0,size,size), new Microsoft.Xna.Framework.Color(0xff,0xff,0xff,0xff));
			_spriteBatch.End();
			
			sin += 0.05d;
			
			_runtimeContext.MessengerContext.Send(PrepareMessage.Default);
			_runtimeContext.MessengerContext.Send(RenderMessage.Default);

			GraphicsDevice.Present();

#if(XAML)
			CopyFromRenderTargetToWriteableBitmap();
#endif
		}

        public void Initialize(int width, int height)
        {
		}

		public void Uninitialize()
		{
			GraphicsDevice.Dispose();
		}

        public INode GetNodeAtPosition(int xPosition, int yPosition)
        {
            return null;
        }

        public Material GetMaterialAtPosition(int xPosition, int yPosition)
        {
            return null;
        }

        public Face GetFaceAtPosition(int xPosition, int yPosition)
        {
            return null;
        }

        public int GetFaceIndexAtPosition(int xPosition, int yPosition)
        {
            return -1;
        }

        public int[] GetCurrentFrame()
        {
            return null;
        }


        public void InitializeSkybox(Objects.Skybox skybox)
        {
            
        }
    }
}
#endif