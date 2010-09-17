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

		NodeStatistics Statistics { get; }
		BoundingSphere BoundingSphere { get; set; }

		string Name { get; set; }

		void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world);
	}
}