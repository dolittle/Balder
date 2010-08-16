using Balder.Display;
using Balder.Materials;
using Balder.Math;

namespace Balder.Lighting
{
#pragma warning disable 1591 // Xml Comments
	public class LightCalculator : ILightCalculator
	{
		public int Calculate(Viewport viewport, Material material, Vector vector, Vector normal)
		{
			var color = viewport.Scene.AmbientColor.ToInt();
			foreach( ILight light in viewport.Scene.Lights )
			{
				var lightColor = light.Calculate(viewport, material, vector, normal);
				color = Color.Additive(color, lightColor);
			}

			return color;
		}
	}
#pragma warning restore 1591
}