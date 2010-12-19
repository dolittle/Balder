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
#if(SILVERLIGHT)
using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Balder.Execution;
using Balder.Objects;
using Balder.Rendering;
using Balder.Rendering.Silverlight;
using Ninject;

namespace Balder.Display.Silverlight
{
	public class Display : IDisplay, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private readonly IPlatform _platform;
		private readonly IRuntimeContext _runtimeContext;
		readonly IRenderingManager _renderingManager;
		private WriteableBitmapQueue _bitmapQueue;

		private bool _initialized;

		private UpdateMessage _updateMessage;
		private RenderMessage _renderMessage;
		private PrepareMessage _prepareMessage;
		private bool _forceShow;
		private bool _forceClear;
		private WriteableBitmap _currentFrontBitmap;
		private WriteableBitmap _currentRenderBitmap;

		private UInt32[] _frontDepthBuffer;
		private Grid _container;
		private Image _image;
		private int _height;
		private int _width;

		public Display(IPlatform platform, IRuntimeContext runtimeContext, IRenderingManager renderingManager)
		{
			_platform = platform;
			_runtimeContext = runtimeContext;
			_renderingManager = renderingManager;
			ClearEnabled = true;
		}

		

		public bool ClearEnabled { get; set; }
		public bool Paused { get; set; }
		public bool Halted { get; set; }

		public void Initialize(int width, int height)
		{
			_width = width;
			_height = height;
			_bitmapQueue = new WriteableBitmapQueue(width,height);
			_frontDepthBuffer = new UInt32[width*height];

			BufferContainer.Width = width;
			BufferContainer.Height = height;

			PrepareFrame(null);
			
			BackgroundColor = Color.FromArgb(0xff, 0, 0, 0);
			InitializeRendering();
			_initialized = true;
		}

		public void Uninitialize()
		{
			_renderingManager.Render -= Render;
			_renderingManager.Show -= Show;
			_renderingManager.Clear -= Clear;
			_renderingManager.Swapped -= Swap;
			_renderingManager.Updated -= Update;
			_renderingManager.Prepare -= Prepare;

			_runtimeContext.MessengerContext.SubscriptionsFor<ShowMessage>().RemoveListener(this, ShowMessage);
		}

		private void InitializeRendering()
		{
			_renderingManager.Render += Render;
			_renderingManager.Show += Show;
			_renderingManager.Clear += Clear;
			_renderingManager.Swapped += Swap;
			_renderingManager.Updated += Update;
			_renderingManager.Prepare += Prepare;
			_renderingManager.Start();

			_runtimeContext.MessengerContext.SubscriptionsFor<ShowMessage>().AddListener(this, ShowMessage);
			_runtimeContext.MessengerContext.SubscriptionsFor<PrepareFrameMessage>().AddListener(this, PrepareFrame);

			_renderMessage = new RenderMessage();
			_prepareMessage = new PrepareMessage();
            _updateMessage = new UpdateMessage();
		}


		private void ShowMessage(ShowMessage message)
		{
			_forceShow = true;
			if( ClearEnabled )
			{
				_forceClear = true;
			}
		}


		
		public void InitializeContainer(object container)
		{
			if (container is Grid)
			{
				_container = container as Grid;
				
				_image = new Image
				{
					Stretch = Stretch.None
				};
				_container.Children.Add(_image);
			}
		}

		public void InitializeSkybox(Skybox skybox)
		{
			if (null != _container)
			{
				_container.Children.Insert(0,skybox.SkyboxContext as SkyboxControl);
			}
		}


		public int[] GetCurrentFrame()
		{
			var frame = new int[BufferContainer.Framebuffer.Length];
			Buffer.BlockCopy(BufferContainer.Framebuffer,0,frame,0,frame.Length*4);
			return frame;
		}

		public Color BackgroundColor { get; set; }



		public void PrepareRender()
		{
			if ((_initialized && !Paused || _forceShow) && !Halted)
			{
				_currentRenderBitmap = _bitmapQueue.CurrentRenderBitmap;
					
				if (null != _currentRenderBitmap)
				{
					BufferContainer.Width = _width;
					BufferContainer.Height = _height;
					BufferContainer.Framebuffer = _currentRenderBitmap.Pixels;
					BufferContainer.DepthBuffer = _frontDepthBuffer;
					Array.Clear(_frontDepthBuffer, 0, _frontDepthBuffer.Length);
				}
			}
		}

		private void PrepareFrame(PrepareFrameMessage obj)
		{
		}

		private bool ShouldClear()
		{
			var shouldClear = (_initialized && ClearEnabled && !Paused || _forceClear) && !Halted;
			return shouldClear;
		}

		public void AfterRender()
		{
			if (_initialized && !Halted )
			{
				_bitmapQueue.RenderDone();
			}

		}

		public void Render()
		{
			if (_initialized && !Halted)
			{
				PrepareRender();
				_runtimeContext.MessengerContext.Send(_renderMessage);
				AfterRender();
			}
		}


		public void Swap()
		{
			if (_initialized && !Halted)
			{
			}
		}

		public void Clear()
		{
			var shouldClear = ShouldClear();
			if (shouldClear)
			{
				var clearBitmap = _bitmapQueue.CurrentRenderBitmap;
				if (null != clearBitmap)
				{
					Array.Clear(clearBitmap.Pixels, 0, clearBitmap.Pixels.Length);
				}
				_forceClear = false;
			}
		}


		public void Show()
		{
			if ((_initialized && !Paused || _forceShow) && !Halted)
			{
				if (null != _image)
				{
					_currentFrontBitmap = _bitmapQueue.GetCurrentShowBitmap();
					if (null != _currentFrontBitmap)
					{
						_image.Source = _currentFrontBitmap;
						_currentFrontBitmap.Invalidate();
						_bitmapQueue.ShowDone();
					}
				}
				_forceShow = false;
			}
		}

		public void Prepare()
		{
			if ((_initialized && !Paused || _forceShow) && !Halted)
			{
				_runtimeContext.MessengerContext.Send(_prepareMessage);
			}
		}

		public void Update()
		{
			if (_initialized && !Halted)
			{
				_runtimeContext.MessengerContext.Send(_updateMessage);
			}
		}
	}
}
#endif