using System;
using Balder.Display;
using Balder.Execution;

namespace Balder.Objects.Geometries
{
	public partial class Mesh
	{
		public static Property<Mesh, Uri> AssetNameProperty =
			Property<Mesh, Uri>.Register(o => o.AssetName);
		public Uri AssetName
		{
			get { return AssetNameProperty.GetValue(this); }
			set { AssetNameProperty.SetValue(this, value); }
		}

		public override void Prepare(Viewport viewport)
		{
			if (null != AssetName && !IsClone)
			{
				_contentManager.LoadInto(this, AssetName.OriginalString);
			}
			base.Prepare(viewport);
		}
	}
}
