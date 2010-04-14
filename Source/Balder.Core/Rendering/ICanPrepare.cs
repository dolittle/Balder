using Balder.Core.Display;

namespace Balder.Core.Rendering
{
	public interface ICanPrepare
	{
		void Prepare(Viewport viewport);
		void InvalidatePrepare();
	}
}