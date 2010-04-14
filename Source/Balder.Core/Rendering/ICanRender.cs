using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core.Rendering
{
	public interface ICanRender
	{
		void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world);
		void RenderDebugInfo(Viewport viewport, Matrix view, Matrix projection, Matrix world);
	}
}