using System;
using System.Windows;

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
	}
}
