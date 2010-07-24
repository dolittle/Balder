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
using Balder.Execution;
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Rendering;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Game = Microsoft.Xna.Framework.Game;
using Matrix = Microsoft.Xna.Framework.Matrix;

namespace Balder.Display.WP7
{
    public class Display : IDisplay
    {
        private static IntPtr _windowHandle;
        private static GraphicsDeviceManager _graphicsDeviceManager;
        private static GraphicsAdapter _graphicsAdapter;
        private static PresentationParameters _presentationParameters;
        internal static GraphicsDevice GraphicsDevice;
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
            _presentationParameters.PresentationInterval = PresentInterval.Immediate;
            GraphicsDevice = new GraphicsDevice(_graphicsAdapter, GraphicsProfile.Reach, _presentationParameters);

            game.Dispose();
        }

        public struct MyVertex : IVertexType
        {
            private Vector3 _position;
            private Microsoft.Xna.Framework.Color _color;

            public MyVertex(Vector3 position, Microsoft.Xna.Framework.Color color)
            {
                _position = position;
                _color = color;
            }


            public static VertexElement[] VertexElements = {
                                                               new VertexElement(0, VertexElementFormat.Vector3,VertexElementUsage.Position, 0), 
                                                               new VertexElement(sizeof(float)*3,VertexElementFormat.Color,VertexElementUsage.Color,0), 
                                                           };

            public static VertexDeclaration Declaration = new VertexDeclaration(VertexElements);

            public VertexDeclaration VertexDeclaration
            {
                get { return Declaration; }
            }
        }

        public void Initialize(int width, int height)
        {
            CompositionTarget.Rendering += CompositionTargetRendering;
            _writeableBitmap = new WriteableBitmap(width, height);
            _renderTarget = new RenderTarget2D(GraphicsDevice, width, height, false, SurfaceFormat.Rg32, DepthFormat.None);

            
        }




        private void CompositionTargetRendering(object sender, EventArgs e)
        {
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Microsoft.Xna.Framework.Color.CadetBlue);
            Messenger.DefaultContext.Send(PrepareMessage.Default);
            Messenger.DefaultContext.Send(RenderMessage.Default);

            /*
            var pos = (float)System.Math.Sin(_sin)*5f;

            var vertexBuffer = new VertexBuffer(GraphicsDevice, typeof(MyVertex), 3, BufferUsage.WriteOnly);
            var vertices = new MyVertex[]
                               {
                                   new MyVertex(new Vector3(pos,0,0), new Microsoft.Xna.Framework.Color(0xff,0,0,0xff)), 
                                   new MyVertex(new Vector3(5,0,0), new Microsoft.Xna.Framework.Color(0,0xff,0,0xff)), 
                                   new MyVertex(new Vector3(0,5,0), new Microsoft.Xna.Framework.Color(0,0,0xff,0xff)), 
                               };
            vertexBuffer.SetData(vertices);

            var indices = new short[]
                              {
                                  0,1,2
                              };

            var indexBuffer = new IndexBuffer(GraphicsDevice, IndexElementSize.SixteenBits, indices.Length,
                                              BufferUsage.WriteOnly);
            indexBuffer.SetData(indices);
            _sin += 0.05f;

            var effect = new BasicEffect(GraphicsDevice);
            effect.VertexColorEnabled = true;
            effect.World = Matrix.Identity;
            effect.View = Matrix.CreateLookAt(new Vector3(0, 0, -10), new Vector3(0, 0, 0), Vector3.Up);
            effect.Projection =
                Matrix.CreatePerspectiveFieldOfView(
                    Microsoft.Xna.Framework.MathHelper.PiOver4,
                    GraphicsDevice.Viewport.AspectRatio, 0.1f, 4000f);

            GraphicsDevice.Indices = indexBuffer;
            GraphicsDevice.SetVertexBuffer(vertexBuffer);

            foreach (var pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                
                //_graphicsDevice.DrawPrimitives(PrimitiveType.TriangleStrip, 0, 1);
                
                
                GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, vertices.Length, 0, indices.Length/3);
            }

            */

            GraphicsDevice.SetRenderTarget(null);
            CopyFromRenderTargetToWriteableBitmap();
        }

        public void Uninitialize()
        {
            
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
    }
}
