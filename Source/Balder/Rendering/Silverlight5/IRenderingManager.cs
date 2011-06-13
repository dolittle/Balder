using Balder.Display;
using Balder.Math;
using Balder.Rendering.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Silverlight5
{
	public interface IRenderingManager
	{
		void Initialize();
		void RegisterForRendering(SpriteContext sprite, Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world);
		void RegisterForRendering(GeometryDetailLevel geometry, Viewport viewport, INode node, Matrix view, Matrix projection, Matrix world);
		void Render(GraphicsDevice graphicsDevice);
	}
}
