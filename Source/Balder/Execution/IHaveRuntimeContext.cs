using Balder.Rendering;

namespace Balder.Execution
{
	public interface IHaveRuntimeContext
	{
		IRuntimeContext RuntimeContext { get; }
	}
}