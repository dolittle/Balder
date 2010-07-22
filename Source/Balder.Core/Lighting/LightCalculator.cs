using Balder.Core.Display;
using Balder.Core.Materials;
using Balder.Core.Math;

namespace Balder.Core.Lighting
{
#pragma warning disable 1591 // Xml Comments
	public class LightCalculator : ILightCalculator
	{
		public Color Calculate(Viewport viewport, Material material, Vector vector, Vector normal)
		{
			var color = viewport.Scene.AmbientColor;

			foreach( ILight light in viewport.Scene.Lights )
			{
				var lightColor = light.Calculate(viewport, vector, normal);
				color = color.Additive(lightColor);
				return color;
			}

			return color;
		}
	}
#pragma warning restore 1591
}