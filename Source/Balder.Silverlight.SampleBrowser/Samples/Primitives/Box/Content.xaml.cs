using System;
using System.Windows;

namespace Balder.Silverlight.SampleBrowser.Samples.Primitives.Box
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
		}

		private void FlipNormals_Checked(object sender, RoutedEventArgs e)
		{
			Box.FlipNormals = (bool) FlipNormals.IsChecked;
		}
	}
}
