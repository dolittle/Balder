using Balder.Core.Imaging;

namespace Balder.Core.Tests.Fakes
{
	public class FakeImageContext : IImageContext
	{
		private static readonly ImageFormat[] ImageFormats = new[]
		                                                     	{
		                                                     		new ImageFormat {PixelFormat = PixelFormat.RGBAlpha, Depth = 32}
		                                                     	};

		public void SetFrame(byte[] frameBytes)
		{

		}

		public void SetFrame(ImageFormat format, byte[] frameBytes)
		{

		}

		public void SetFrame(ImageFormat format, byte[] frameBytes, ImagePalette palette)
		{

		}

		public ImageFormat[] SupportedImageFormats { get { return ImageFormats; }}
	}
}