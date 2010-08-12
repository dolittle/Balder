using Balder.Execution;
using Balder.Rendering.Silverlight;

namespace Balder.Silverlight.SampleBrowser.Samples.Materials.Filtering
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();
			Game.Update += Game_Update;
		}

		private void Game_Update(Game game)
		{
			Time.Text = GeometryDetailLevel.Milliseconds.ToString();
		}
	}
}
