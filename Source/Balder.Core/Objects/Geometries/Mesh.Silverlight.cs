using System;
using Balder.Core.Silverlight.Helpers;

namespace Balder.Core.Objects.Geometries
{
	public partial class Mesh
	{
		public static DependencyProperty<Mesh, Uri> AssetNameProperty =
			DependencyProperty<Mesh, Uri>.Register(o => o.AssetName);
		public Uri AssetName
		{
			get { return AssetNameProperty.GetValue(this); }
			set { AssetNameProperty.SetValue(this, value); }
		}

		public override void Prepare()
		{
			if (null != AssetName && !IsClone)
			{
				Load(AssetName.ToString());
			}
			base.Prepare();
		}
	}
}
