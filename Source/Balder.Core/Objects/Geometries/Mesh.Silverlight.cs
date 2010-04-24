using System;
using Balder.Core.Display;
using Balder.Core.Execution;

namespace Balder.Core.Objects.Geometries
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
