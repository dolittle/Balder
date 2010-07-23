using System.Windows.Controls;
using System.Windows.Media;
using Balder.Silverlight.Helpers;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class ImageButton : Button
	{
		public ImageButton()
		{
			DefaultStyleKey = typeof (ImageButton);
		}

		public static readonly DependencyProperty<ImageButton, ImageSource> ImageProperty =
			DependencyProperty<ImageButton, ImageSource>.Register(t => t.Image);
		public ImageSource Image
		{
			get { return ImageProperty.GetValue(this); }
			set { ImageProperty.SetValue(this, value); }
		}

		public static readonly DependencyProperty<ImageButton, object> ToolTipProperty =
			DependencyProperty<ImageButton, object>.Register(t => t.ToolTip);
		public object ToolTip
		{
			get { return ToolTipProperty.GetValue(this); }
			set { ToolTipProperty.SetValue(this, value); }
		}


	}
}
