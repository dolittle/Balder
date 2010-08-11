using System;
using Balder.Execution;
using Balder.Objects.Geometries;
using Balder.Rendering;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.Water
{
	public partial class Content
	{
		private static readonly Random rnd = new Random();
		private int _frameNumber = 0;
		private int _rainCounter = 0;

		private float[, ,] _waveMap;

		public Content()
		{
			InitializeComponent();

			_waveMap = new float[2, (int)HeightMap.LengthSegments, (int)HeightMap.HeightSegments];

			Game.Update += Game_Update;
		}




		private void Heightmap_HeightInput(object sender, HeightmapEventArgs e)
		{
			var n = 0f;
			var x = e.GridX;
			var z = e.GridY;
			if (x <= 0 || z <= 0 || x >= HeightMap.LengthSegments-1 || z >= HeightMap.HeightSegments-1)
			{
				return;
			}

			var frame1 = _frameNumber;
			var frame0 = _frameNumber ^ 1;

			n = ((_waveMap[frame1, x - 1, z] +
				_waveMap[frame1, x + 1, z] +
				_waveMap[frame1, x, z - 1] +
				_waveMap[frame1, x, z + 1]) / 2) -
				_waveMap[frame0, x, z];
			n = n - n / 16;
			_waveMap[frame0, x, z] = n;

			e.Height = n;
		}

		private void Game_Update(Game game)
		{
			var frame1 = _frameNumber;

			if (_rainCounter-- <= 0)
			{
				_rainCounter = 50;

				var x = System.Math.Abs(rnd.Next((int)HeightMap.LengthSegments - 2) + 1);
				var z = System.Math.Abs(rnd.Next((int)HeightMap.HeightSegments - 2) + 1);
				//x = HeightMap.LengthSegments/2;
				//z = HeightMap.HeightSegments / 2;

				_waveMap[frame1, x, z] = -10;
			}

			var geometryDetailLevel = HeightMap.GeometryContext.GetDetailLevel(DetailLevel.Full);
			if( geometryDetailLevel.FaceCount > 0 &&
				geometryDetailLevel.VertexCount > 0)
			{
				//GeometryHelper.CalculateNormals(geometryDetailLevel, false);
			}


			_frameNumber ^= 1;
		}
	}
}