using Balder.Display;
using Balder.Math;
using Balder.Rendering.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Silverlight5
{
	public class RenderingGeometry
	{
		public GeometryDetailLevel Geometry;
		public Matrix World;
		public Matrix View;
		public Matrix Projection;
		public INode Node;
		public Viewport Viewport;

		public bool IsVisible;


		public void Render(GraphicsDevice graphicsDevice)
		{
			if (!IsVisible)
				return;

			Geometry.ActualRender(graphicsDevice, Viewport, Node, View, Projection, World);
		}
	}
}