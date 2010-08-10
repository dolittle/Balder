using System;
using System.Windows.Threading;

namespace Balder.Silverlight.SampleBrowser.Samples.View.Skybox
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			var timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(20);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();
		}

		private double _sin;
		void timer_Tick(object sender, EventArgs e)
		{
			var x = System.Math.Cos(_sin)*30d;
			var z = System.Math.Sin(_sin) * 30d;
			var y = System.Math.Sin(_sin*2) * 30d;


			Camera.Position.X = x;
			Camera.Position.Z = z;
			Camera.Position.Y = y;

			_sin += 0.005d;

			PositionText.Text = string.Format("{0}, {1}, {2}", x, y, z);
		}
	}
}
