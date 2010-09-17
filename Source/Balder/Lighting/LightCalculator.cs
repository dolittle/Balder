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

		private static uint White = 0xffffffff;
		private static int WhiteAsInt = (int) White;

		public int Calculate(Viewport viewport, Material material, Vector vector, Vector normal, out int diffuseResult, out int specularResult)
		{
			diffuseResult = Color.AlphaFull | 0xffffff;
			specularResult = 0;
			if (null == _lights || _lights.Length == 0)
			{
				return WhiteAsInt;
			}

			var color = 0; // _sceneAmbient;
			var diffuse = Color.AlphaFull | 0xffffff;
			var specular = 0;
			var lightDiffuse = 0;
			var lightSpecular = 0;
			for (var lightIndex = 0; lightIndex < _lights.Length; lightIndex++)
			{
				var light = _lights[lightIndex];
				var lightColor = light.Calculate(viewport, material, vector, normal, out lightDiffuse, out lightSpecular);
				color = Color.Additive(color, lightColor);
				diffuse = Color.Additive(diffuse, lightDiffuse);
				specular = Color.Additive(specular, lightSpecular);
			}

			diffuseResult = diffuse|Color.AlphaFull;
			specularResult = specular | Color.AlphaFull;

			return color | Color.AlphaFull;
		}
	}
#pragma warning restore 1591
}