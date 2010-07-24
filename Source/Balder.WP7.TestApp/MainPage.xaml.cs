using System;
using System.Windows.Threading;
using Microsoft.Phone.Controls;

namespace Balder.WP7.TestApp
{
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
        }

        void timer_Tick(object sender, EventArgs e)
        {
            Object.Rotation.Y += 1;
        }
    }
}
