using System.Windows;
using Balder.Execution;
using Balder.Input.Silverlight;
using Balder.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Loaded += Content_Loaded;
			
		}

		public void GoBack()
		{
			Runtime.Instance.UnregisterGame(Game);
		}


		void Content_Loaded(object sender, RoutedEventArgs e)
		{
			// Update camera and rotate cube for initialization
			Game.Camera.Update(Game.Viewport);
			CubeRotate(0, 0);

			InfoGrid.DataContext = ManipulationEventHelper.ManipulationInfo;
			CubeInfoGrid.DataContext = Cube.Info;
		}

		
		/// <summary>
		/// Gets called when SolveButton is clicked
		/// </summary>
		private void SolveClicked(object sender, RoutedEventArgs e)
		{
			Cube.Solve();
		}

		/// <summary>
		/// Gets called when ResetButton is clicked
		/// </summary>
		private void ResetClicked(object sender, RoutedEventArgs e)
		{
			CubeContainer.World = Matrix.Identity;
			Cube.Reset();
		}

		/// <summary>
		/// Rotate the cube - or the container for the cube, rather
		/// </summary>
		/// <param name="deltaX"></param>
		/// <param name="deltaY"></param>
		private void CubeRotate(int deltaX, int deltaY)
		{
			// Get the containers current world matrix
			var matrix = CubeContainer.World;

			// Create a delta rotation matrix from the delta values
			var rotationMatrix = Matrix.CreateRotation((float)-deltaY, (float)-deltaX, 0);

			// Combine the matrices (current world and new delta rotation matrix)
			CubeContainer.World = matrix*rotationMatrix;

			// Store the actual world, which consists of the Cameras viewmatrix and the 
			// rotation for the CubeContainer - used for manipulation of the cube
			Cube.ActualViewMatrix = CubeContainer.World*Game.Camera.ViewMatrix;
		}

		/// <summary>
		/// Gets called when ShuffleButton is clicked
		/// </summary>
		private void ShuffleClicked(object sender, RoutedEventArgs e)
		{
			Cube.Shuffle();
		}

		/// <summary>
		/// Gets called when CheatListToggleButton is clicked
		/// </summary>
		private void CheatListToggleClicked(object sender, RoutedEventArgs e)
		{
			if( CheatList.Visibility == Visibility.Visible)
			{
				CheatList.Visibility = Visibility.Collapsed;
			} else
			{
				CheatList.Visibility = Visibility.Visible;
			}
		}
	}
}
