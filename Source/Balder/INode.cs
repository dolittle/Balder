using Balder.Display;
using Balder.Execution;
using Balder.Math;

namespace Balder
{
	public interface INode : IHaveIdentity
	{
		INode Parent { get; }

		Matrix ActualWorld { get; }
		Matrix RenderingWorld { get; set; }
		
		Scene Scene { get; set; }

		void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world);
	}
}