﻿using Balder.Rendering.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Silverlight5
{
	public class RenderingGeometry : RenderingObject
	{
		public GeometryDetailLevel Geometry;

		public override void Render(GraphicsDevice graphicsDevice)
		{
			if (!IsVisible)
				return;

			Geometry.ActualRender(graphicsDevice, Viewport, Node, View, Projection, World);
		}
	}
}