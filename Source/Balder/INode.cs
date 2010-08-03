using Balder.Display;
using Balder.Math;

namespace Balder
{
	public interface INode
	{
		INode Parent { get; }

		Matrix ActualWorld { get; }
		Matrix RenderingWorld { get; set; }
		
		Scene Scene { get; set; }

		BoundingSphere BoundingSphere { get; set; }

		void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world);
	}
}