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

			//Chamfer.ManipulationDelta += Chamfer_ManipulationDelta;
		}

		/*
		void Chamfer_ManipulationDelta(object sender, Core.Input.ManipulationDeltaEventArgs args)
		{
			var matrix = Chamfer.World;

			var rotationMatrix = Matrix.CreateRotation((float)-args.DeltaY, (float)-args.DeltaX, 0);

			Chamfer.World = matrix * rotationMatrix;
			
		}*/

		
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
		}
	}
}
