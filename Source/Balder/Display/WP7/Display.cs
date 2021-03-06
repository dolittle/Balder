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
#if(WINDOWS_PHONE)
using System;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Balder.Execution;
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Rendering;
using Balder.Rendering.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game = Microsoft.Xna.Framework.Game;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Balder.Display.WP7
{
    public class Display : IDisplay
    {
        private readonly IRuntimeContext _runtimeContext;
        private static IntPtr _windowHandle;
        private static GraphicsDeviceManager _graphicsDeviceManager;
        private static GraphicsAdapter _graphicsAdapter;
        private static PresentationParameters _presentationParameters;
        internal static GraphicsDevice GraphicsDevice;
        private RenderTarget2D _renderTarget;
        private Image _image;
        private WriteableBitmap _writeableBitmap;
        private int _width;
        private int _height;

        public Color BackgroundColor { get; set; }
        public bool ClearEnabled { get; set; }
        public bool Paused { get; set; }
        public bool Halted { get; set; }

    	private static Game _game;

        public Display(IRuntimeContext runtimeContext)
        {
            _runtimeContext = runtimeContext;
        }


        public static void Initialize()
        {
            _game = new Game();
            
            _windowHandle = _game.Window.Handle;
            _graphicsDeviceManager = new GraphicsDeviceManager(_game);

            _graphicsAdapter = GraphicsAdapter.DefaultAdapter;

            _presentationParameters = new PresentationParameters();
            _presentationParameters.BackBufferWidth = _graphicsDeviceManager.PreferredBackBufferWidth;
            _presentationParameters.BackBufferHeight = _graphicsDeviceManager.PreferredBackBufferHeight;
            _presentationParameters.DepthStencilFormat = _graphicsDeviceManager.PreferredDepthStencilFormat;
            _presentationParameters.BackBufferFormat = _graphicsDeviceManager.PreferredBackBufferFormat;
            _presentationParameters.DeviceWindowHandle = _windowHandle;
            _presentationParameters.PresentationInterval = PresentInterval.Immediate;
            _presentationParameters.RenderTargetUsage = RenderTargetUsage.DiscardContents;
            
			GraphicsDevice = new GraphicsDevice(_graphicsAdapter, GraphicsProfile.Reach, _presentationParameters);
        	GraphicsDevice.DepthStencilState = new DepthStencilState
        	                                   	{
        	                                   		DepthBufferEnable = true,
        	                                   		DepthBufferFunction = CompareFunction.Less,
													DepthBufferWriteEnable = true
        	                                   	};
				
            _game.Dispose();
        }

        public void Initialize(int width, int height)
        {
            _width = width;
            _height = height;
            CompositionTarget.Rendering += CompositionTargetRendering;
            _writeableBitmap = new WriteableBitmap(width, height);
            _renderTarget = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Rg32, DepthFormat.Depth16);
        }

		public void Uninitialize()
		{
			CompositionTarget.Rendering -= CompositionTargetRendering;
			GraphicsDevice.Dispose();
			_game.Exit();
		}


        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(BackgroundColor);
            _runtimeContext.MessengerContext.Send(PrepareMessage.Default);
            _runtimeContext.MessengerContext.Send(RenderMessage.Default);
            GraphicsDevice.SetRenderTarget(null);
            CopyFromRenderTargetToWriteableBitmap();
        }


        private void CopyFromRenderTargetToWriteableBitmap()
        {
            _renderTarget.GetData(_writeableBitmap.Pixels);
            _writeableBitmap.Invalidate();
        }


        public void InitializeContainer(object container)
        {
            if (container is Grid)
            {
                _image = new Image
                {
                    Stretch = Stretch.None,
                    Source = _writeableBitmap
                };
                ((Grid)container).Children.Add(_image);
            }
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