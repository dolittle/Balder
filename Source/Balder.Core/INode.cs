using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core
{
	public interface INode
	{
		INode Parent { get; }

		Matrix ActualWorld { get; }
		Matrix RenderingWorld { get; set; }
		
		Scene Scene { get; set; }

		void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world);
	}
}