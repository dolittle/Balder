using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core.Lighting
{
#pragma warning disable 1591 // Xml Comments
	public class LightCalculator : ILightCalculator
	{
		public ColorAsFloats Calculate(Viewport viewport, Vector vector, Vector normal)
		{
			var color = viewport.Scene.AmbientColor.ToColorAsFloats();

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