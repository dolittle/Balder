using Balder.Core.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			var normal = Vector.Left;
			var desired = Vector.Forward;
			var rotation = Matrix.CreateRotation(0, 90, 0);
			var rotated = Vector.TransformNormal(normal, rotation);
				
			rotated.Normalize();

			var length = (desired - rotated).Length;


		}
	}
}
