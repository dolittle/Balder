using System.Collections.Generic;
using Balder.Collections;
using Balder.Display;
using Balder.Execution;
using Balder.Materials;
using Balder.Math;
using Balder.Rendering;

namespace Balder.Lighting
{
#pragma warning disable 1591 // Xml Comments
	[Singleton]
	public class LightCalculator : ILightCalculator
	{
		private readonly static uint White = 0xffffffff;
		private readonly static int WhiteAsInt = (int)White;
		private ILight[] _lights;
		private int _sceneAmbient;

        public LightCalculator(IRuntimeContext runtimeContext)
        {
            runtimeContext.MessengerContext.SubscriptionsFor<RenderDoneMessage>().AddListener(this, m => HasLightsChanged = false);
            runtimeContext.MessengerContext.SubscriptionsFor<LightChangedMessage>().AddListener(this, m => HasLightsChanged = true);
        }

		public void Prepare(Viewport viewport, NodeCollection lights)
		{
			_sceneAmbient = viewport.Scene.AmbientColor.ToInt();

			var lightsToUse = new List<ILight>();
			foreach( ILight light in lights )
			{
				if( light.IsEnabled )
					lightsToUse.Add(light);

                if (!light.IsStatic) HasLightsChanged = true;
			}
			_lights = lightsToUse.ToArray();
		}

        public void PrepareForNode(INode node, Matrix viewToLocal)
        {
            foreach (var light in _lights)
                light.PrepareForNode(node, viewToLocal);
        }


		public int Calculate(Viewport viewport, Material material, Vector vector, Vector normal, out int diffuseResult, out int specularResult)
		{
			diffuseResult = Color.AlphaFull | 0xffffff;
			specularResult = 0;
			if (null == _lights || _lights.Length == 0)
			{
				return WhiteAsInt;
			}

			var color = material.AmbientAsInt; 
			var diffuse = 0; 
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


        public bool HasLightsChanged { get; private set; }


    }
#pragma warning restore 1591 // Xml Comments
}