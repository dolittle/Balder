using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Balder.Core;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl
{
	public partial class Content : UserControl
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
