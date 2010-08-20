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

		private const int ArrayWidth = 40;
		private const int ArrayHeight = 40;

		private float[,] _waveMap1;
		private float[,] _waveMap2;
		private float[,] _currentWaveMap;
		private float[,] _otherWaveMap;
		private float[, ,] _waveMap;

		public Content()
		{
			InitializeComponent();

			_waveMap1 = new float[ArrayWidth, ArrayHeight];
			_waveMap2 = new float[ArrayWidth, ArrayHeight];

			SetCurrentWavemap();


			Game.Update += Game_Update;
		}


		private void SetCurrentWavemap()
		{
			if (_frameNumber == 0)
			{
				_currentWaveMap = _waveMap1;
				_otherWaveMap = _waveMap2;
			}
			else
			{
				_currentWaveMap = _waveMap2;
				_otherWaveMap = _waveMap1;
			}
			HeightMap.HeightmapArray = _otherWaveMap;
		}

		private void UpdateWavemap()
		{
			var n = 0f;
			for (var z = 1; z < ArrayHeight-1; z++)
			{
				for (var x = 1; x < ArrayWidth-1; x++)
				{
					n = ((_otherWaveMap[x - 1, z] +
						_otherWaveMap[x + 1, z] +
						_otherWaveMap[x, z - 1] +
						_otherWaveMap[x, z + 1]) / 2) -
						_currentWaveMap[x, z];
					n = n - n / 16;
					_currentWaveMap[x, z] = n;
				}
			}
		}


		private void Heightmap_HeightInput(object sender, HeightmapEventArgs e)
		{
			var n = 0f;
			var x = e.GridX;
			var z = e.GridY;
			if (x <= 0 || z <= 0 || x >= HeightMap.LengthSegments - 1 || z >= HeightMap.HeightSegments - 1)
			{
				return;
			}


			e.Height = n;
		}

		private void Game_Update(Game game)
		{
			
			UpdateWavemap();
			SetCurrentWavemap();
			if (_rainCounter-- <= 0)
			{
				_rainCounter = 50;

				var x = System.Math.Abs(rnd.Next((int)HeightMap.LengthSegments - 2) + 1);
				var z = System.Math.Abs(rnd.Next((int)HeightMap.HeightSegments - 2) + 1);

				_otherWaveMap[x, z] = -10;
			}


			_frameNumber ^= 1;

		}
	}
}