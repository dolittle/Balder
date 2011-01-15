using Balder.Execution;
using Balder.Objects.Geometries;
using Balder.Rendering;

namespace Balder.Xna.TestApp
{
	public class MyGame : Game
	{
		public MyGame(IRuntimeContext runtimeContext, INodeRenderingService nodeRenderingService) : base(runtimeContext, nodeRenderingService)
		{
			
		}


		public override void OnLoadContent()
		{
			var teapot = ContentManager.Load<Mesh>("Assets/teapot.ASE");
			base.OnLoadContent();
		}
		
	}
}
