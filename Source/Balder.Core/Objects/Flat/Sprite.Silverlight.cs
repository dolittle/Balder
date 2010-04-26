using System;
using Balder.Core.Display;
using Balder.Core.Silverlight.Helpers;

namespace Balder.Core.Objects.Flat
{
	public partial class Sprite
	{
		public static DependencyProperty<Sprite, Uri> AssetNameProperty =
			DependencyProperty<Sprite, Uri>.Register(o => o.AssetName);
		public Uri AssetName
		{
			get { return AssetNameProperty.GetValue(this); }
			set { AssetNameProperty.SetValue(this, value); }
		}

		public override void Prepare(Viewport viewport)
		{
			if (null != AssetName)
			{
				_contentManager.LoadInto(this,AssetName.OriginalString);
			}
			base.Prepare(viewport);
		}
	}
}
