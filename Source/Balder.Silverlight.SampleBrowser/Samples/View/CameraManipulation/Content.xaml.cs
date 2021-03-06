﻿using Balder.Math;
using Balder.View;

namespace Balder.Silverlight.SampleBrowser.Samples.View.CameraManipulation
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Loaded += Content_Loaded;
		}

		void Content_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			CalculateCameraPosition();
		}

		private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (null != _xSlider)
			{
				CalculateCameraPosition();
			}
		}

		private void CalculateCameraPosition()
		{
			var rotationX = Matrix.CreateRotationX((float)_xSlider.Value);
			var rotationY = Matrix.CreateRotationY((float)_ySlider.Value);

			var combined = rotationX*rotationY;
			var forward = Vector.Forward;
			var zoomedForward = forward*(float) _zoomSlider.Value;
			var position = zoomedForward*combined;

			var target = new Vector((float)_game.Camera.Target.X, 
			                        (float)_game.Camera.Target.Y, 
			                        (float)_game.Camera.Target.Z);
			var actualPosition = target - position;

			_game.Camera.Position.X = actualPosition.X;
			_game.Camera.Position.Y = actualPosition.Y;
			_game.Camera.Position.Z = actualPosition.Z;

			_game.Camera.FieldOfView = _fovSlider.Value;
		}

        private void PerspectiveCameraButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentPosition = _game.Camera.Position.ToVector();
            var currentTarget = _game.Camera.Target.ToVector();
            _game.Camera = new Camera();
            _game.Camera.Position = currentPosition;
            _game.Camera.Target = currentTarget;
        }

        private void OrthographicCameraButtonClick(object sender, System.Windows.RoutedEventArgs e)
        {
            var currentPosition = _game.Camera.Position.ToVector();
            var currentTarget = _game.Camera.Target.ToVector();
            _game.Camera = new OrthographicCamera();
            _game.Camera.Position = currentPosition;
            _game.Camera.Target = currentTarget;
        }
    }
}