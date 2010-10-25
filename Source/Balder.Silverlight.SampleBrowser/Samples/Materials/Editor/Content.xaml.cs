using System;
using System.Windows;

namespace Balder.Silverlight.SampleBrowser.Samples.Materials.Editor
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
		}


		private void SlidersValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			if (null != NearSlider)
			{
				Camera.Near = (float)NearSlider.Value;
			}
			if (null != FarSlider)
			{
				Camera.Far = (float)FarSlider.Value;
			}
		}
	}
}
