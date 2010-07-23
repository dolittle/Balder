using System.Windows.Media;
using Balder.Execution;
using Balder.Lighting;
using Balder.Materials;
using Balder.Objects.Geometries;
using Color=Balder.Color;

namespace Balder.Silverlight.SampleBrowser.Samples.Programatic.Teapot
{
	public class MyGame : Game
	{
		private Mesh _teapot;

		public override void OnInitialize()
		{
			ContentManager.AssetsRoot = "Assets";

			var light = new OmniLight();
			light.Diffuse = Color.FromArgb(0xff, 255, 121, 32);
			light.Specular = Color.FromArgb(0xff, 0xff, 0xff, 0xff);
			light.Ambient = Color.FromArgb(0xff, 0x7f, 0x3f, 0x10);

			light.Position.X = -100;
			light.Position.Y = 0;
			light.Position.Z = 0;
			Scene.AddNode(light);

			Camera.Position.X = 0;
			Camera.Position.Y = 0;
			Camera.Position.Z = -100;

			Camera.Target.Y = 0;

			base.OnInitialize();
		}

		public override void OnLoadContent()
		{

			_teapot = ContentManager.Load<Mesh>("teapot.ASE");

			var material = new Material
			               	{
			               		Diffuse = Colors.Blue,
			               		Shade = MaterialShade.Gouraud,
			               		DoubleSided = true
			               	};
			_teapot.Material = material;

			_teapot.Color = Colors.Black;
			Scene.AddNode(_teapot);
			
			base.OnLoadContent();
		}

		private float _yRotation;

		private bool _hasDoneStuff;

		public override void OnUpdate()
		{
			if( !_hasDoneStuff)
			{


				//_hasDoneStuff = true;
			}
			_teapot.Rotation.Y += 1;
			base.OnUpdate();
		}
	}
}
