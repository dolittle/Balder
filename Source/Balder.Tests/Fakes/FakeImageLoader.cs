using System;
using Balder.Assets;
using Balder.Content;
using Balder.Imaging;

namespace Balder.Tests.Fakes
{
	public class FakeImageLoader : AssetLoader
	{
		public FakeImageLoader(IFileLoader fileLoader, IContentManager contentManager)
			: base(fileLoader, contentManager)
		{
		}

		public override string[] FileExtensions
		{
			get { return new[] { "jpg", "png" }; }
		}

		public override Type SupportedAssetType
		{
			get { return typeof(Image); }
		}

		public override IAssetPart[] Load(string assetName)
		{
			var imageContext = new FakeImageContext();
			var images = new[] { new Image(imageContext) };
			return images;
		}
	}
}