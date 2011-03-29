using Balder.Execution;
using Balder.Lighting;
using Balder.Materials;
using Balder.Objects.Geometries;
using Balder.Math;
using Balder.Sound;
using Matrix = Balder.Math.Matrix;
using System;

namespace Balder.Silverlight.SampleBrowser.Samples.Sound.MP3
{
    public class MP3 : Game, ISound
	{
		private Mesh _teapot;
        private Mesh _audi;
        private float _audiRotation;

        public Uri AssetSound { get; set; }

		public override void OnInitialize()
		{
			ContentManager.AssetsRoot = "Assets";
            //Display.BackgroundColor = Colors.Yellow;

			var light = new ViewLight();
			Scene.AddNode(light);

            Camera.Position.X = 0;
            Camera.Position.Y = 0;
            Camera.Position.Z = -10;

            Camera.Target.Y = 0;

            //Camera.Target = Vector.Zero;
            //Camera.Position = new Vector(0, 0, 100);

			base.OnInitialize();
           
		}

		public override void OnLoadContent()
		{
            _audi = ContentManager.Load<Mesh>("audi.ASE");
            Scene.AddNode(_audi);

            //_teapot = ContentManager.Load<Mesh>("teapot.ASE");

            //var material = new Material
            //                {
            //                    Diffuse = Colors.Blue,
            //                    Shade = MaterialShade.Gouraud,
            //                    DoubleSided = true
            //                };
            //_teapot.Material = material;

			//_teapot.Color = Colors.Black;
			//Scene.AddNode(_teapot);
            
			
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

            //_audi.World = Matrix.CreateRotationY(_audiRotation);
            _audi.Rotation.Y += 1;
			base.OnUpdate();
		}
	}
}
