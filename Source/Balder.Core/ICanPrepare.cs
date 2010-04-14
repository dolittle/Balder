using Balder.Core.Display;

namespace Balder.Core
{
	public interface ICanPrepare
	{
		void Prepare(Viewport viewport);
		void InvalidatePrepare();
	}
}