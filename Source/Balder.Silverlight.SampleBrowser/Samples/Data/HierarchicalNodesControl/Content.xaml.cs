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

			Loaded += Content_Loaded;
			Game.Update += Game_Update;
		}

		void Content_Loaded(object sender, RoutedEventArgs e)
		{
			
			int i = 0;
			i++;

			var count = VisualTreeHelper.GetChildrenCount(Geom);
			var presenter = VisualTreeHelper.GetChild(Geom, 0) as ItemsPresenter;
			var presenterCount = VisualTreeHelper.GetChildrenCount(presenter);
			var panel = VisualTreeHelper.GetChild(presenter, 0) as StackPanel;
			
			foreach( ContentPresenter child in panel.Children )
			{
				var c = VisualTreeHelper.GetChild(child, 0);
				var content = child.Content;
				var dataTemplate = child.ContentTemplate as DataTemplate;
				var actualContent = dataTemplate.LoadContent();
			}

			
		}

		void Game_Update(Balder.Core.Execution.Game game)
		{
			NodeCounter.Text = Scene.NodeCount.ToString();
		}

	}
}
