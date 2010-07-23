using Balder.Display;

namespace Balder.Rendering
{
	public interface ICanPrepare
	{
		void Prepare(Viewport viewport);
		void InvalidatePrepare();
	}
}