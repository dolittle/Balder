using System;
using System.Windows;
using Balder.Math;
using Balder.Objects.Geometries;
using System.Collections.Generic;
using System.Net;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Balder.Silverlight.SampleBrowser.Samples.Sound.MP3
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
            SoundFile.Source = new Uri("Assets/Sounds/ding.wav", UriKind.Relative);
		}

        private void StopAll()
        {
            MusicFile.Stop();
            SoundFile.Stop();
            VideoFile.Stop();
        }

        private void Button_Click_Music(object sender, RoutedEventArgs e)
        {
            StopAll();
            SoundFile.Play();
        }

        private void Button_Click_Sound(object sender, RoutedEventArgs e)
        {
            StopAll();
            MusicFile.Play();
        }

        private void Button_Click_Video(object sender, RoutedEventArgs e)
        {
            StopAll();
            VideoFile.Play();
        }
	}
}
