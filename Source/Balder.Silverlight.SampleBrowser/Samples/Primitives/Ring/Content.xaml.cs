using System.Windows;

namespace Balder.Silverlight.SampleBrowser.Samples.Primitives.Ring
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
			Ring.StartAngle = StartSlider.Value;
			Ring.EndAngle = EndSlider.Value;
			Ring.CapEnds = (bool)CapEnds.IsChecked;
			Ring.Spokes = (bool)Spokes.IsChecked;
			Ring.Segments = (int)SegmentsSlider.Value;
			Ring.InnerRadius = InnerRadiusSlider.Value;
			Ring.OuterRadius = OuterRadiusSlider.Value;
			Ring.FlipNormals = (bool)FlipNormals.IsChecked;
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

		private void InnerRadiusValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (!_loaded)
			{
				return;
			}

			if( e.NewValue > OuterRadiusSlider.Value )
			{
				OuterRadiusSlider.Value = e.NewValue + 0.1;
			}

			UpdateProperties();

		}

		private void OuterRadiusValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
		{
			if (!_loaded)
			{
				return;
			}

			if( e.NewValue < InnerRadiusSlider.Value )
			{
				InnerRadiusSlider.Value = e.NewValue - 0.1;
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