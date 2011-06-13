using Balder.Display;
using Balder.Math;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Silverlight5
{
	public abstract class RenderingObject
	{
		public Matrix World;
		public Matrix View;
		public Matrix Projection;
		public INode Node;
		public Viewport Viewport;
		public bool IsVisible;

		public abstract void Render(GraphicsDevice graphicsDevice);
	}
}