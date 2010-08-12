using Balder.Display;
using Balder.Execution;

namespace Balder.Rendering
{
	public interface IRuntimeContext
	{
		IDisplay Display { get; }
		bool PassiveRendering { get; set; }
		PassiveRenderingMode PassiveRenderingMode { get; set; }
		bool Paused { get; set; }
		IMessengerContext MessengerContext { get; }
		void SignalRendering();
	}
}