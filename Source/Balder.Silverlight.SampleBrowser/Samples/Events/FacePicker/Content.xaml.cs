using Balder.Input.Silverlight;
using Balder.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Events.FacePicker
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Loaded += ContentLoaded;
		}

		void ContentLoaded(object sender, System.Windows.RoutedEventArgs e)
		{
			MouseInfoGrid.DataContext = NodeMouseEventHelper.MouseInfo;
		}


		private void Box_MouseMove(object sender, Input.MouseEventArgs args)
		{
			var pickRay = Game.Viewport.GetPickRay((int)args.Position.X, (int)args.Position.Y);
			Face face;
			int faceIndex;
			float faceU;
			float faceV;
			var distance = Box.Intersects(Game.Viewport, pickRay, out face, out faceIndex, out faceU, out faceV);
			if( null != distance )
			{
				Visualizer.FaceIndex = faceIndex;
			}
		}

		private void Box_MouseLeave(object sender, Input.MouseEventArgs args)
		{
			Visualizer.FaceIndex = -1;
		}
	}
}
