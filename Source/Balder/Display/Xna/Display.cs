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
using System.Drawing;
using System.Windows.Forms;
using Balder.Execution;
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Rendering;
using Balder.Rendering.Xna;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Balder.Display.Xna
{
    public class Display : IDisplay
    {
    	public static Form Window;
		internal static GraphicsDevice GraphicsDevice;
		private readonly IRuntimeContext _runtimeContext;

        private static IntPtr _windowHandle;
        private static GraphicsAdapter _graphicsAdapter;
        private static PresentationParameters _presentationParameters;
        private int _width;
        private int _height;

        public Color BackgroundColor { get; set; }
        public bool ClearEnabled { get; set; }
        public bool Paused { get; set; }
        public bool Halted { get; set; }

        public Display(IRuntimeContext runtimeContext)
        {
            _runtimeContext = runtimeContext;
			Initialize();
        }


        private void Initialize()
		{
			_graphicsAdapter = GraphicsAdapter.DefaultAdapter;
			_presentationParameters = new PresentationParameters();
			
			Window = new Form();
			Window.Size = new Size(800,600);
			Window.Paint += WindowPaint;

        	_windowHandle = Window.Handle;

        	_presentationParameters.BackBufferWidth = 800;
        	_presentationParameters.BackBufferHeight = 600;
        	_presentationParameters.DepthStencilFormat = DepthFormat.Depth16;
        	_presentationParameters.BackBufferFormat = SurfaceFormat.Color;
            _presentationParameters.DeviceWindowHandle = _windowHandle;
            _presentationParameters.PresentationInterval = PresentInterval.Immediate;
        	_presentationParameters.IsFullScreen = false;

			GraphicsDevice = new GraphicsDevice(_graphicsAdapter, GraphicsProfile.Reach, _presentationParameters);
			
        	GraphicsDevice.DepthStencilState = new DepthStencilState
        	                                   	{
        	                                   		DepthBufferEnable = true,	
        	                                   		DepthBufferFunction = CompareFunction.Less,
													DepthBufferWriteEnable = true
        	                                   	};
		}

		private void WindowPaint(object sender, PaintEventArgs e)
		{
			GraphicsDevice.Clear(
				ClearOptions.Target | ClearOptions.DepthBuffer,
				new Microsoft.Xna.Framework.Color(0xff, 0, 0, 0xff),
				1f,
				0);
			
			_runtimeContext.MessengerContext.Send(PrepareMessage.Default);
			_runtimeContext.MessengerContext.Send(RenderMessage.Default);
			

			GraphicsDevice.Present();
		}

        public void Initialize(int width, int height)
        {
            _width = width;
            _height = height;
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