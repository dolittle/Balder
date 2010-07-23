using Balder.Collections;

namespace Balder.Rendering
{
	public interface IHaveChildren
	{
		NodeCollection Children { get; }
	}
}