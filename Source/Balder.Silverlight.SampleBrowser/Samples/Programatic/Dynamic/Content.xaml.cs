using System;
using System.Windows;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Programatic.Dynamic
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
		}

		private static readonly Random rnd = new Random();

		private void Button_Click(object sender, RoutedEventArgs e)
		{
			var position = new Coordinate((rnd.NextDouble()*10)-5, (rnd.NextDouble()*10)-5, (rnd.NextDouble()*10)-5);
			var box = new Box {Position = position, Dimension = new Coordinate(5, 5, 5), InteractionEnabled = true};
			Game.Scene.AddNode(box);
		}
	}
}
