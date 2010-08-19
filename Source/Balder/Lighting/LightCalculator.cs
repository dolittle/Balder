using System.Collections.Generic;
using Balder.Collections;
using Balder.Display;
using Balder.Execution;
using Balder.Materials;
using Balder.Math;

namespace Balder.Lighting
{
#pragma warning disable 1591 // Xml Comments
	[Singleton]
	public class LightCalculator : ILightCalculator
	{
		private ILight[] _lights;
		private int _sceneAmbient;

		public void Prepare(Viewport viewport, NodeCollection lights)
		{
			_sceneAmbient = viewport.Scene.AmbientColor.ToInt();

			var lightsToUse = new List<ILight>();
			foreach( ILight light in lights )
			{
				
				if( light.IsEnabled )
				{
					lightsToUse.Add(light);
				}
				
			}
			_lights = lightsToUse.ToArray();
		}

		public int Calculate(Viewport viewport, Material material, Vector vector, Vector normal)
		{
			if (null == _lights)
			{
				return Color.AlphaFull;
			}

			var color = _sceneAmbient;
			for (var lightIndex = 0; lightIndex < _lights.Length; lightIndex++)
			{
				var light = _lights[lightIndex];
				var lightColor = light.Calculate(viewport, material, vector, normal);
				color = Color.Additive(color, lightColor);
			}

			return color | Color.AlphaFull;
		}
	}
#pragma warning restore 1591
}