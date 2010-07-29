using System;
using System.Windows;

namespace Balder.Silverlight.SampleBrowser.Samples.Primitives.Cylinder
{
	public partial class Content
	{
		private bool _loaded;
		public Content()
		{
			InitializeComponent();
			Loaded += Content_Loaded;
		}

		private void Content_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			UpdateProperties();
			_loaded = true;
		}


		private void StartValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (!_loaded)
			{
				return;
			}
			if (e.NewValue > EndSlider.Value)
			{
				EndSlider.Value = e.NewValue;
			}
			UpdateProperties();

		}

		private void EndValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (!_loaded)
			{
				return;
			}
			if (e.NewValue < StartSlider.Value)
			{
				StartSlider.Value = e.NewValue;
			}
			UpdateProperties();
		}


		private void UpdateProperties()
		{
			Cylinder.StartAngle = StartSlider.Value;
			Cylinder.EndAngle = EndSlider.Value;
			Cylinder.CapEnds = (bool)CapEnds.IsChecked;
			Cylinder.Spokes = (bool)Spokes.IsChecked;
			Cylinder.Segments = (int)SegmentsSlider.Value;
			Cylinder.TopRadius = TopRadiusSlider.Value;
			Cylinder.BottomRadius = BottomRadiusSlider.Value;
			Cylinder.FlipNormals = (bool)FlipNormals.IsChecked;
		}

		private void CapEnds_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}

			UpdateProperties();
		}

		private void SliderValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (!_loaded)
			{
				return;
			}

			UpdateProperties();
		}

		private void Spokes_Checked(object sender, System.Windows.RoutedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}
			UpdateProperties();
		}

		private void FlipNormals_Checked(object sender, RoutedEventArgs e)
		{
			if (!_loaded)
			{
				return;
			}
			UpdateProperties();
		}
	}
}