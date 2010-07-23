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
using System.Threading;
using System.Windows.Media;

namespace Balder.Rendering.Silverlight
{
	public delegate void RenderEventHandler();

	public class RenderingManager
	{
		public static readonly RenderingManager Instance = new RenderingManager();

		public event RenderEventHandler Updated = () => { };
		public event RenderEventHandler Render = () => { };
		public event RenderEventHandler Clear = () => { };
		public event RenderEventHandler Swapped = () => { };
		public event RenderEventHandler Show = () => { };
		public event RenderEventHandler Prepare = () => { };

		private Thread _renderingThread;
		private bool _active;

		private ManualResetEvent _showStartedEvent;


		private RenderingManager()
		{
		}

		public void Start()
		{
			if (!_active)
			{
				CompositionTarget.Rendering += ShowTimer;

				_showStartedEvent = new ManualResetEvent(false);

				_active = true;
				_renderingThread = new Thread(RenderingThread);
				_renderingThread.Start();
			}
		}

		public void Stop()
		{
			_active = false;
		}

		public void SignalRendering()
		{
		}

		private void RenderingThread()
		{
			_showStartedEvent.WaitOne();

			return;
			while( _active )
			{
				Clear();
				Render();
				
				Swapped();
			}
		}

		private void ShowTimer(object sender, EventArgs e)
		{
			Updated();

			_showStartedEvent.Set();
			Prepare();
			Clear();
			Render();
			Swapped();

			Show();
		}
	}
}