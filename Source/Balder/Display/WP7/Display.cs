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
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Balder.Materials;
using Balder.Objects.Geometries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Display.WP7
{
    public class Display : IDisplay
    {
        private static IntPtr _windowHandle;
        private static GraphicsDeviceManager _graphicsDeviceManager;
        private static GraphicsAdapter _graphicsAdapter;
        private static PresentationParameters _presentationParameters;
        private static GraphicsDevice _graphicsDevice;
        private RenderTarget2D _renderTarget;
        private Image _image;
        private WriteableBitmap _writeableBitmap;

        public Color BackgroundColor { get; set; }
        public bool ClearEnabled { get; set; }
        public bool Paused { get; set; }
        public bool Halted { get; set; }


        public static void Initialize()
        {

            var game = new Game();

            _windowHandle = game.Window.Handle;
            _graphicsDeviceManager = new GraphicsDeviceManager(game);

            _graphicsAdapter = GraphicsAdapter.DefaultAdapter;

            _presentationParameters = new PresentationParameters();
            _presentationParameters.BackBufferWidth = _graphicsDeviceManager.PreferredBackBufferWidth;
            _presentationParameters.BackBufferHeight = _graphicsDeviceManager.PreferredBackBufferHeight;
            _presentationParameters.DepthStencilFormat = _graphicsDeviceManager.PreferredDepthStencilFormat;
            _presentationParameters.BackBufferFormat = _graphicsDeviceManager.PreferredBackBufferFormat;
            _presentationParameters.DeviceWindowHandle = _windowHandle;
            _graphicsDevice = new GraphicsDevice(_graphicsAdapter, GraphicsProfile.Reach, _presentationParameters);

            game.Dispose();
        }

        public void Initialize(int width, int height)
        {
            _renderTarget = new RenderTarget2D(_graphicsDevice, width, height, false, SurfaceFormat.Rg32, DepthFormat.None);

            _graphicsDevice.SetRenderTarget(_renderTarget);
            _graphicsDevice.Clear(Microsoft.Xna.Framework.Color.CadetBlue);
            _graphicsDevice.SetRenderTarget(null);
            
            _writeableBitmap = new WriteableBitmap(width,height);
            CopyFromRenderTargetToWriteableBitmap();

            
        }

        public void Uninitialize()
        {
            
        }


        private void CopyFromRenderTargetToWriteableBitmap()
        {
            _renderTarget.GetData(_writeableBitmap.Pixels);

            /*
            for (var pixel = 0; pixel < _writeableBitmap.Pixels.Length; pixel++)
            {
                _writeableBitmap.Pixels[pixel] = -1;
            }*/

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
    }
}
