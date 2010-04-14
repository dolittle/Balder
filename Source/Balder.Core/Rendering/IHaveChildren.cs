using Balder.Core.Collections;

namespace Balder.Core.Rendering
{
	public interface IHaveChildren
	{
		NodeCollection Children { get; }
	}
}