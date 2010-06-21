using System;
using System.Windows;
using Balder.Core.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
		}

		private void SolveClicked(object sender, RoutedEventArgs e)
		{
			Cube.Solve();
		}

		private void ResetClicked(object sender, RoutedEventArgs e)
		{
			Cube.Reset();
		}

		private void CubeRotate(int deltaX, int deltaY)
		{
			var matrix = CubeContainer.World;

			var rotationMatrix = Matrix.CreateRotation((float)-deltaY, (float)-deltaX, 0);

			CubeContainer.World = matrix*rotationMatrix;
		}
	}
}
