using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Xna
{
	public static class GraphicsDeviceExtensions
	{
		public static void SetShader(this GraphicsDevice graphicsDevice, Shader shader)
		{
			graphicsDevice.SetVertexShader(shader.Vertex);
			graphicsDevice.SetPixelShader(shader.Pixel);
		}
		
	}
}