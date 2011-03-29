using System;
using System.Windows.Threading;
using Balder.Tools;
using System.Windows.Controls;
using System.Windows;

namespace Balder.Silverlight.SampleBrowser.Samples.Custom.VertexCustom
{
    public partial class Content
    {
        public Content()
        {
            InitializeComponent();

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += new EventHandler(timer_Tick);
            //timer.Start();

            lstBox.Items.Add(TempData.Y);
            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            if(TempData.Changed)
                lstBox.Items.Add(TempData.Y);
        }

        private void spValue_Loaded(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (sender as StackPanel);

            TextBlock disc = new TextBlock();
            disc.Text = "Discretionary Percentage";

            stackPanel.Children.Add(disc);

        }
    }
}
