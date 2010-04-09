using Balder.Core.Collections;

namespace Balder.Core
{
	public interface IHaveChildren
	{
		NodeCollection Children { get; }
	}
}