using Balder.Display;

namespace Balder.Rendering
{
	public interface ICanRender
	{
		void Render(Viewport viewport, DetailLevel detailLevel);
		void RenderDebugInfo(Viewport viewport, DetailLevel detailLevel);
	}
}