using Balder.Math;
using Balder.Rendering.Xna;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Silverlight5
{
	public interface IRenderingManager
	{
		void Initialize();
		void RegisterForRendering(GeometryDetailLevel geometry, INode node, Matrix view, Matrix projection, Matrix world);
		void Render(GraphicsDevice graphicsDevice);
	}
}
