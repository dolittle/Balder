﻿using System.Windows.Input;
using System.Windows.Media;

namespace Balder.Silverlight.SampleBrowser.Samples.Rendering.Passive
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
		}

		private void Box_MouseEnter(object sender, MouseEventArgs e)
		{
			Box.Color = Colors.Blue;

		}

		private void Box_MouseLeave(object sender, MouseEventArgs e)
		{
			Box.Color = Colors.Green;
		}
	}
}
