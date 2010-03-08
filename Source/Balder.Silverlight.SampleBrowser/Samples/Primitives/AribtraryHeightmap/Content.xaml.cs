using System;
using Balder.Core;
using Balder.Core.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Primitives.AribtraryHeightmap
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
		}

		private double _sin;
		private double _movement;

		private void Heightmap_HeightInput(object sender, HeightmapEventArgs e)
		{
			var height = Math.Sin(_sin + _movement) * 2;

			e.Height = (float)height;
			var highlight = (byte)((height * 16f) + 32f);

			e.Color = Color.FromArgb(0xff, highlight, highlight, highlight);

			_sin += 0.03;

			if (e.GridX == Heightmap.LengthSegments &&
				e.GridY == Heightmap.HeightSegments)
			{
				_sin = 0;
				_movement += 0.05;
			}

		}

	}
}
