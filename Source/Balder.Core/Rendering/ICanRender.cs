using Balder.Core.Display;

namespace Balder.Core.Rendering
{
	public interface ICanRender
	{
		void Render(Viewport viewport, DetailLevel detailLevel);
		void RenderDebugInfo(Viewport viewport, DetailLevel detailLevel);
	}
}