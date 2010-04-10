using System.Windows.Controls;
using Balder.Core;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl
{
	public partial class Content : UserControl
	{
		public Content()
		{
			InitializeComponent();

			Game.Update += Game_Update;
		}


		void Game_Update(Balder.Core.Execution.Game game)
		{
			NodeCounter.Text = Scene.NodeCount.ToString();
		}

	}
}
