using System.Windows;

namespace Balder.Silverlight.SampleBrowser.Samples.View.Clipping
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			SlidersValueChanged(this, new RoutedPropertyChangedEventArgs<double>(0,0));
		}

		private void SlidersValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (null != NearSlider)
			{
				Camera.Near = (float) NearSlider.Value;
			}
			if (null != FarSlider)
			{
				Camera.Far = (float) FarSlider.Value;
			}
		}
	}
}
