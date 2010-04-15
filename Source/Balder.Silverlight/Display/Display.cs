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
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Balder.Core;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Silverlight.Rendering;
using Color = Balder.Core.Color;

namespace Balder.Silverlight.Display
{
	public class Display : IDisplay, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private readonly IPlatform _platform;
		private WriteableBitmapQueue _bitmapQueue;

		private bool _initialized;

		public Display(IPlatform platform, INodesPixelBuffer nodesPixelBuffer)
		{
			_platform = platform;
			NodesPixelBuffer = nodesPixelBuffer;
		}

		public void Initialize(int width, int height)
		{
			_bitmapQueue = new WriteableBitmapQueue(width,height);
			_frontDepthBuffer = new UInt32[width*height];

			NodesPixelBuffer.Initialize(width,height);

			BufferContainer.Width = width;
			BufferContainer.Height = height;
			BufferContainer.RedPosition = 2;
			BufferContainer.GreenPosition = 0;
			BufferContainer.BluePosition = 1;
			BufferContainer.AlphaPosition = 3;
			BufferContainer.Stride = width;
			_initialized = true;
			BackgroundColor = Color.FromArgb(0xff, 0, 0, 0);
		}

		private Image _image;
		public void InitializeContainer(object container)
		{
			if (container is Grid)
			{
				_image = new Image
				{
					Stretch = Stretch.None
				};
				((Grid)container).Children.Add(_image);
			}
		}

		public void EnablePassiveRendering()
		{
			RenderingManager.Instance.EnablePassiveRendering();
		}

		public void EnableActiveRendering()
		{
			RenderingManager.Instance.EnableActiveRendering();
		}

		public void SetPassiveRenderingMode(PassiveRenderingMode renderingMode)
		{
			
		}

		public void SignalRendering()
		{
			RenderingManager.Instance.SignalRendering();
		}

		public INode GetNodeAtPosition(int xPosition, int yPosition)
		{
			var node = NodesPixelBuffer.GetNodeAtPosition(xPosition, yPosition);
			return node;
		}

		public int[] GetCurrentFrame()
		{
			var frame = new int[BufferContainer.Framebuffer.Length];
			Buffer.BlockCopy(BufferContainer.Framebuffer,0,frame,0,frame.Length*4);
			return frame;
		}

		public Color BackgroundColor { get; set; }

		private WriteableBitmap _currentFrontBitmap;
		private WriteableBitmap _currentRenderBitmap;
		
		private UInt32[] _frontDepthBuffer;

		
		public INodesPixelBuffer NodesPixelBuffer { get; private set; }
		

		public void PrepareRender()
		{
			if (_initialized)
			{
				_currentRenderBitmap = _bitmapQueue.CurrentRenderBitmap;
					
				if (null != _currentRenderBitmap)
				{
					BufferContainer.Framebuffer = _currentRenderBitmap.Pixels;
					BufferContainer.DepthBuffer = _frontDepthBuffer;
					BufferContainer.NodeBuffer = NodesPixelBuffer.RenderingBuffer;
					
					Array.Clear(_frontDepthBuffer, 0, _frontDepthBuffer.Length);
				}
			}
		}

		public void AfterRender()
		{
			if (_initialized)
			{
				_bitmapQueue.RenderDone();
			}

		}

		public void Render()
		{
			
		}


		public void Swap()
		{
			if (_initialized)
			{
			}
		}

		public void Clear()
		{
			if (_initialized)
			{
				var clearBitmap = _bitmapQueue.CurrentRenderBitmap;
				if (null != clearBitmap)
				{
					Array.Clear(clearBitmap.Pixels, 0, clearBitmap.Pixels.Length);
				}
			}
		}


		public void Show()
		{
			if (_initialized)
			{
				if (null != _image)
				{
					_currentFrontBitmap = _bitmapQueue.GetCurrentShowBitmap();
					if (null != _currentFrontBitmap)
					{
						_image.Source = _currentFrontBitmap;
						_currentFrontBitmap.Invalidate();
						_bitmapQueue.ShowDone();

						NodesPixelBuffer.NewFrame();
					}
				}
			}
		}

		public void Prepare()
		{
			
		}

		public void Update()
		{
			if (_initialized)
			{
			}
		}
	}
}