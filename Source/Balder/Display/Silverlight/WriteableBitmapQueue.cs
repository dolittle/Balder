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
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace Balder.Display.Silverlight
{
	public class WriteableBitmapQueue
	{
		private readonly Queue<WriteableBitmap> _renderQueue;
		private readonly Stack<WriteableBitmap> _showStack;
		private readonly int _width;
		private readonly int _height;

		public WriteableBitmapQueue(int width, int height)
		{
			_width = width;
			_height = height;
			_renderQueue = new Queue<WriteableBitmap>();
			_showStack = new Stack<WriteableBitmap>();

			for( var i=0; i<4; i++ )
			{
				var writeableBitmap = new WriteableBitmap(_width, _height);
				_renderQueue.Enqueue(writeableBitmap);
			}

			CurrentRenderBitmap = _renderQueue.Dequeue();
		}


		public WriteableBitmap CurrentRenderBitmap { get; private set; }
		public WriteableBitmap CurrentShowBitmap { get; private set; }
		public WriteableBitmap PreviousShowBitmap { get; private set; }


		public void RenderDone()
		{
			var current = CurrentRenderBitmap;
			if (_renderQueue.Count > 0)
			{
				CurrentRenderBitmap = _renderQueue.Dequeue();
				_showStack.Push(current);
			}
		}

		public WriteableBitmap	GetCurrentShowBitmap()
		{
			if( _showStack.Count == 0 )
			{
				return null;
			}
			var next = _showStack.Pop();
			if( _showStack.Count > 0 )
			{
				foreach( var bitmap in _showStack )
				{
					if( null != PreviousShowBitmap && bitmap.Equals(PreviousShowBitmap))
					{
						continue;
					}
					_renderQueue.Enqueue(bitmap);
				}
				_showStack.Clear();
			}
			CurrentShowBitmap = next;

			return next;
		}

		public void ShowDone()
		{
			var previous = PreviousShowBitmap;
			PreviousShowBitmap = CurrentShowBitmap;
			if (null != previous)
			{
				_renderQueue.Enqueue(previous);
			}
		}
	}
}
#endif