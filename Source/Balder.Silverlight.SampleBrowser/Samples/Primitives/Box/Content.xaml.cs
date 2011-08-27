using System;
using System.Windows;
using System.Windows.Threading;
using Balder.Math;
using System.ComponentModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Primitives.Box
{
	public class ViewModel : INotifyPropertyChanged
	{
		public ViewModel()
		{
			var timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromSeconds(1);
			timer.Tick += new EventHandler(timer_Tick);
			timer.Start();

			Rotation = new Coordinate(0, 0, 0);
		}

		public bool IsHitTest { get; set; }

		Coordinate _rotation;
		public Coordinate Rotation 
		{
			get { return _rotation; }
			set
			{
				_rotation = value;
				PropertyChanged(this, new PropertyChangedEventArgs("Rotation"));
			}
		} 

		void timer_Tick(object sender, EventArgs e)
		{
			Rotation = new Coordinate(0, Rotation.Y + 1, 0);
		}

		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
	}


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
