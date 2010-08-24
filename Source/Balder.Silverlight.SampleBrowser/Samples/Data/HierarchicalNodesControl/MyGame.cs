using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Browser;
using System.Windows.Data;
using Balder.Execution;
using Balder.Objects.Geometries;
using Balder.Silverlight.ValueConverters;

namespace Balder.Silverlight.SampleBrowser.Samples.Data.HierarchicalNodesControl
{
	public class MyGame : Game
	{

		public override void OnLoadContent()
		{
			/*
			var conv = new VectorToCoordinateValueConverter();
			var n = new List<INode>();
			for( var i=0; i<3375; i++ )
			{
				var c = new Column(0, 0, 0);
				var t = ContentManager.Load<Mesh>("teapot.ASE");
				t.IsVisible = false;
				t.DataContext = c;
				var binding = new Binding("Position");
				binding.Converter = conv;
				t.SetBinding(Node.PositionProp.ActualDependencyProperty, binding);
				Scene.AddNode(t);
			}

			HtmlPage.Window.Navigate(new Uri("http://www.vg.no"));
			*/
			base.OnLoadContent();
		}

	}
}
