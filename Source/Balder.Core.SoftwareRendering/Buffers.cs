﻿using System;
#if(SILVERLIGHT)
using System.Windows.Media;
#else
using System.Drawing;
#endif

namespace Balder.Core.SoftwareRendering
{

	public class Buffers<T> : IBuffers
		where T : IFrameBuffer, new()
	{
		private static readonly object ClearingDepthBufferLock = new object();
		private static readonly object DepthBufferLock = new object();

		public const UInt32 DepthBufferMax = UInt32.MaxValue;
		public const UInt32 DepthBufferMin = UInt32.MinValue;
		public static Color BlackBackground = Color.FromArgb(0xff, 0, 0, 0);

		private UInt32[] _clearingDepthBuffer;


		public Buffers(int width, int height)
		{
			Width = width;
			Height = height;
			FrameBuffer = new T();
			FrameBuffer.Initialize(width, height);
			InitializeZBuffer();
		}

		private void InitializeZBuffer()
		{
			var zBufferSize = Width * Height;
			lock (DepthBufferLock)
			{
				DepthBuffer = new UInt32[zBufferSize];
				FrontDepthBuffer = new UInt32[zBufferSize];
			}

			lock (ClearingDepthBufferLock)
			{
				_clearingDepthBuffer = new UInt32[zBufferSize];

				for (var index = 0; index < DepthBuffer.Length; index++)
				{
					_clearingDepthBuffer[index] = DepthBufferMax;
				}
			}

			ClearZBuffer();
			SwapZBuffer();
			ClearZBuffer();
		}



		private void SwapZBuffer()
		{
			lock (DepthBufferLock)
			{
				var front = FrontDepthBuffer;
				var back = DepthBuffer;
				DepthBuffer = front;
				FrontDepthBuffer = back;
			}
		}


		private void ClearZBuffer()
		{
			if (null != _clearingDepthBuffer &&
				null != FrontDepthBuffer)
			{
				lock (ClearingDepthBufferLock)
				{
					_clearingDepthBuffer.CopyTo(FrontDepthBuffer, 0);
				}
			}
		}

		public IFrameBuffer FrameBuffer { get; private set; }
		public UInt32[] DepthBuffer { get; private set; }
		private UInt32[] FrontDepthBuffer { get; set; }

		public int Width { get; private set; }
		public int Height { get; private set; }

		public void Swap()
		{
			FrameBuffer.Swap();
			SwapZBuffer();
		}

		public void Clear()
		{
			FrameBuffer.Clear();
			ClearZBuffer();
		}

		public void Show()
		{
			FrameBuffer.Show();
		}
	}
}
