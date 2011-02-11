using System;
using System.Collections.ObjectModel;
using System.Windows.Threading;
using Balder.Math;
using Microsoft.Phone.Controls;

namespace Balder.WP7.TestApp
{
	public class Something
	{
		public Coordinate Position { get; set; }
	}


    public partial class MainPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();

			Items = new ObservableCollection<Something>();
        	Nodes.ItemsSource = Items;
        }

        void timer_Tick(object sender, EventArgs e)
        {
            //Object.Rotation.Y += 1;
        }

		public ObservableCollection<Something> Items { get; set; }

		private void Button_Click(object sender, System.Windows.RoutedEventArgs e)
		{
			Items.Add(new Something());
		}
    }
}
