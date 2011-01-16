using Balder.Execution;
using Balder.Imaging;
using Balder.Lighting;
using Balder.Materials;
using Balder.Objects.Geometries;

namespace Balder.Xna.TestApp
{
	public class MyGame : Game
	{
		public override void OnInitialize()
		{
			Viewport.Width = 800;
			Viewport.Height = 600;


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
			var texture = ContentManager.Load<Image>("Assets/BalderLogo.png");
			var material = ContentManager.Creator.CreateMaterial();
			var map = new ImageMap(texture);
			material.DiffuseMap = map;
			

			var teapot = ContentManager.Load<Mesh>("Assets/teapot.ASE");
			teapot.Material = material;
			Scene.AddNode(teapot);
			base.OnLoadContent();
		}

		public override void OnUpdate()
		{
			base.OnUpdate();
		}
		
	}
}
