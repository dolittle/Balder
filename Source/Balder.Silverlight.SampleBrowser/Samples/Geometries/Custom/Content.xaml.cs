namespace Balder.Silverlight.SampleBrowser.Samples.Geometries.Custom
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Game.Update += new Balder.Execution.GameEventHandler(Game_Update);
			Game.Camera.Near = 15;
		}

		void Game_Update(Balder.Execution.Game game)
		{
			Game.Camera.Near += 0.01f;
		}
	}
}
