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
using Balder.Execution;
using Balder.Rendering;
using Balder.Rendering.Silverlight;

namespace Balder.Display.Silverlight
{
	[Singleton]
	public class DisplayDevice : IDisplayDevice
	{
		private readonly List<Display> _displays;
		private readonly IPlatform _platform;
		readonly IRenderingManager _renderingManager;

		public DisplayDevice(IPlatform platform)
		{
			_platform = platform;
			_renderingManager = new RenderingManager();
			_displays = new List<Display>();
		}


		public IDisplay CreateDisplay(IRuntimeContext runtimeContext)
		{
			var display = new Display(runtimeContext, _renderingManager);
			lock( _displays )
			{
				_displays.Add(display);	
			}
			
			return display;
		}


		public void RemoveDisplay(IDisplay display)
		{
			if (display is Display)
			{
				lock( _displays )
				{
					display.Uninitialize();
					_displays.Remove(display as Display);	
				}
			}
		}
	}
}
#endif