using Balder.Execution;
using Balder.Lighting;
using Balder.Objects.Geometries;

namespace Balder.Xna.TestApp
{
	public class MyGame : Game
	{
		public override void OnInitialize()
		{
			ContentManager.AssetsRoot = "Assets";

			var light = new ViewLight();
			Scene.AddNode(light);

			Camera.Position.X = 0;
			Camera.Position.Y = 0;
			Camera.Position.Z = -100;

			Camera.Target.Y = 0;

			base.OnInitialize();
		}



		public override void OnLoadContent()
		{
			var teapot = ContentManager.Load<Mesh>("Assets/teapot.ASE");
			Scene.AddNode(teapot);
			base.OnLoadContent();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}
		
	}
}
