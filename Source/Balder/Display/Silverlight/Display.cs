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
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Rendering;
using Balder.Rendering.Silverlight;

namespace Balder.Display.Silverlight
{
	public class Display : IDisplay, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private readonly IPlatform _platform;
		private WriteableBitmapQueue _bitmapQueue;

		private bool _initialized;

		private UpdateMessage _updateMessage;
		private RenderMessage _renderMessage;
		private PrepareMessage _prepareMessage;
		private bool _forceShow;
		private bool _forceClear;

		public Display(IPlatform platform)
		{
			_platform = platform;
			ClearEnabled = true;
		}

		

		public bool ClearEnabled { get; set; }
		public bool Paused { get; set; }
		public bool Halted { get; set; }

		public void Initialize(int width, int height)
		{
			_bitmapQueue = new WriteableBitmapQueue(width,height);
			_frontDepthBuffer = new UInt32[width*height];

			BufferContainer.Width = width;
			BufferContainer.Height = height;
			BufferContainer.RedPosition = 2;
			BufferContainer.GreenPosition = 0;
			BufferContainer.BluePosition = 1;
			BufferContainer.AlphaPosition = 3;
			BufferContainer.Stride = width;

			PrepareFrame(null);
			
			BackgroundColor = Color.FromArgb(0xff, 0, 0, 0);
			InitializeRendering();
			_initialized = true;
		}

		public void Uninitialize()
		{
			RenderingManager.Instance.Render -= Render;
			RenderingManager.Instance.Show -= Show;
			RenderingManager.Instance.Clear -= Clear;
			RenderingManager.Instance.Swapped -= Swap;
			RenderingManager.Instance.Updated -= Update;
			RenderingManager.Instance.Prepare -= Prepare;
			Messenger.DefaultContext.SubscriptionsFor<ShowMessage>().RemoveListener(this, ShowMessage);
		}

		private void InitializeRendering()
		{
			RenderingManager.Instance.Render += Render;
			RenderingManager.Instance.Show += Show;
			RenderingManager.Instance.Clear += Clear;
			RenderingManager.Instance.Swapped += Swap;
			RenderingManager.Instance.Updated += Update;
			RenderingManager.Instance.Prepare += Prepare;
			RenderingManager.Instance.Start();

			Messenger.DefaultContext.SubscriptionsFor<ShowMessage>().AddListener(this,ShowMessage);
			Messenger.DefaultContext.SubscriptionsFor<PrepareFrameMessage>().AddListener(this,PrepareFrame);

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


		public void PrepareRender()
		{
			if (_initialized )
			{
				_currentRenderBitmap = _bitmapQueue.CurrentRenderBitmap;
					
				if (null != _currentRenderBitmap)
				{
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
			return (_initialized && ClearEnabled && !Paused || _forceClear) && !Halted;
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
				Messenger.DefaultContext.Send(_renderMessage);
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
			if (ShouldClear())
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
			Messenger.DefaultContext.Send(_prepareMessage);
		}

		public void Update()
		{
			if (_initialized && !Halted)
			{
				Messenger.DefaultContext.Send(_updateMessage);
			}
		}
	}
}
#endif