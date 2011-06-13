using Balder.Rendering.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Silverlight5
{
	public class RenderingSprite : RenderingObject
	{
		public SpriteContext Sprite;

		public override void Render(GraphicsDevice graphicsDevice)
		{
			if (!IsVisible)
				return;

			Sprite.ActualRender(graphicsDevice, Viewport, Node, View, Projection, World);
		}
	}
}