namespace Balder.Silverlight.Rendering.Drawing
{
	public class Clut
	{
		private static uint[,] _addedRedComponent;
		private static uint[,] _addedGreenComponent;
		private static uint[,] _addedBlueComponent;
		private static uint[,] _addedAlphaComponent;
		private static uint[,] _multipliedRedComponent;
		private static uint[,] _multipliedGreenComponent;
		private static uint[,] _multipliedBlueComponent;
		private static uint[,] _multipliedAlphaComponent;

		static Clut()
		{
			CalculateAddedComponent();
			CalculateMultipliedComponent();
		}

		private static void CalculateAddedComponent()
		{
			_addedRedComponent = new uint[256, 256];
			_addedGreenComponent = new uint[256, 256];
			_addedBlueComponent = new uint[256, 256];
			_addedAlphaComponent = new uint[256, 256];
			for (var a = 0; a < 256; a++)
			{
				for( var b=0; b<256; b++)
				{
					var component = a + b;
					if( component > 255 )
					{
						component = 255;
					}
					_addedRedComponent[a, b] = ((uint)component) << 16;
					_addedGreenComponent[a, b] = ((uint)component) << 8;
					_addedBlueComponent[a, b] = ((uint)component);
					_addedAlphaComponent[a, b] = ((uint)component) << 24;
				}
			}
		}

		private static void CalculateMultipliedComponent()
		{
			_multipliedRedComponent = new uint[256, 256];
			_multipliedGreenComponent = new uint[256, 256];
			_multipliedBlueComponent = new uint[256, 256];
			_multipliedAlphaComponent = new uint[256, 256];
			for (var a = 0; a < 256; a++)
			{
				for (var b = 0; b < 256; b++)
				{
					var floatA = ((float) a)/256f;
					var floatB = ((float) b) / 256f;

					var multiplied = floatA*floatB;
					var component = (int)(multiplied*256f);
					if (component > 255)
					{
						component = 255;
					}
					_multipliedRedComponent[a, b] = ((uint)component) << 16;
					_multipliedGreenComponent[a, b] = ((uint)component) << 8;
					_multipliedBlueComponent[a, b] = ((uint)component);
					_multipliedAlphaComponent[a, b] = ((uint)component) << 24;
				}
			}
			
		}

		public static int MultiplyColors(int a, int b)
		{
			var alphaA = ((a >> 24) & 0xff);
			var redA = ((a >> 16) & 0xff);
			var greenA = ((a >> 8) & 0xff);
			var blueA = ((a) & 0xff);

			var alphaB = ((b >> 24) & 0xff);
			var redB = ((b >> 16) & 0xff);
			var greenB = ((b >> 8) & 0xff);
			var blueB = ((b) & 0xff);

			
			
			var addedPixel =
				_multipliedAlphaComponent[alphaA, alphaB] |
				_multipliedRedComponent[redA, redB] |
				_multipliedGreenComponent[greenA, greenB] |
				_multipliedBlueComponent[blueA, blueB];
			/*
			var addedPixel =
				_addedAlphaComponent[alphaA, alphaB] |
				_addedRedComponent[redA, redB] |
				_addedGreenComponent[greenA, greenB] |
				_addedBlueComponent[blueA, blueB];*/
			return (int)addedPixel;
		}
		
	}
}