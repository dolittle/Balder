using Balder.Core.Execution;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Game.Initialize += Game_Initialize;
		}

		private void Game_Initialize(Game game)
		{
			game.ContentManager.AssetsRoot = "Samples/Creative/RubicsCube/Assets";
		}
	}
}
