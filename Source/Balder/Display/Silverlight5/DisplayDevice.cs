using System.Collections.Generic;
using Balder.Execution;
using Balder.Rendering;
using Balder.Rendering.Silverlight5;
using Microsoft.Practices.ServiceLocation;

namespace Balder.Display.Silverlight5
{
	[Singleton]
	public class DisplayDevice : IDisplayDevice
	{
		private readonly List<Display> _displays;
		private readonly IPlatform _platform;

		public DisplayDevice(IPlatform platform)
		{
			_platform = platform;
			_displays = new List<Display>();
		}


		public IDisplay CreateDisplay(IRuntimeContext runtimeContext)
		{
			var display = new Display(runtimeContext, ServiceLocator.Current.GetInstance<IRenderingManager>());
			lock (_displays)
			{
				_displays.Add(display);
			}

			return display;
		}


		public void RemoveDisplay(IDisplay display)
		{
			if (display is Display)
			{
				lock (_displays)
				{
					display.Uninitialize();
					_displays.Remove(display as Display);
				}
			}
		}
	}
}
