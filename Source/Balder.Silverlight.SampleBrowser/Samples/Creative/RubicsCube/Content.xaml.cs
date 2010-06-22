using System.Windows;
using Balder.Core.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Loaded += new RoutedEventHandler(Content_Loaded);
		}

		void Content_Loaded(object sender, RoutedEventArgs e)
		{
			Game.Camera.Update(Game.Viewport);
			CubeRotate(0, 0);
		}

		
		private void SolveClicked(object sender, RoutedEventArgs e)
		{
			Cube.Solve();
		}

		private void ResetClicked(object sender, RoutedEventArgs e)
		{
			CubeContainer.World = Matrix.Identity;
			Cube.Reset();
		}

		private void CubeRotate(int deltaX, int deltaY)
		{
			var matrix = CubeContainer.World;

			var rotationMatrix = Matrix.CreateRotation((float)-deltaY, (float)-deltaX, 0);

			CubeContainer.World = matrix*rotationMatrix;

			Cube.ActualViewMatrix = CubeContainer.World*Game.Camera.ViewMatrix;
		}
	}
}
