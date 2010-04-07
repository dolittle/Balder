using Balder.Core;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.NodesControl
{
	public partial class Content
	{
		public Content()
		{
			InitializeComponent();

			Game.Update += new Balder.Core.Execution.GameEventHandler(Game_Update);
		}

		void Game_Update(Balder.Core.Execution.Game game)
		{
			NodeCounter.Text = Scene.NodeCount.ToString();
		}
	}
}