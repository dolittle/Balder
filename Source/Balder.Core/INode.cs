using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core
{
	public interface INode
	{
		Matrix ActualWorld { get; }
		Matrix RenderingWorld { get; set; }

		void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world);
	}
}