#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//

#endregion

using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Balder.Core;
using Balder.Core.Display;
using Balder.Core.Execution;
using Matrix=Balder.Core.Math.Matrix;

namespace Balder.Silverlight.Controls
{
	public class NodesControl : Container
	{
		private bool _prepareChildren;

		public NodesControl()
		{
			_prepareChildren = false;

			var binding = new Binding {Source = this, Path = new PropertyPath("ItemsSource")};
			SetBinding(ItemsSourceChangedProperty, binding);
		}

		public static readonly DependencyProperty ItemsSourceChangedProperty =
			DependencyProperty.Register("ItemsSourceChanged",
			                            typeof (IEnumerable),
			                            typeof (NodesControl),
										new PropertyMetadata(ItemsSourceChanged));
		private static void ItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			var nodesControl = obj as NodesControl;
			if( null != nodesControl )
			{
				nodesControl._prepareChildren = true;
			}
		}

		public static readonly Property<NodesControl, DataTemplate> NodeTemplateProperty =
			Property<NodesControl, DataTemplate>.Register(n => n.NodeTemplate);
		public DataTemplate NodeTemplate
		{
			get { return base.ItemTemplate; }
			set { base.ItemTemplate = value; }
		}

		#region Hidden Properties
		private new ItemsPanelTemplate ItemsPanel { get; set; }
		private new DataTemplate ItemTemplate { get; set; }
		#endregion

		private void PrepareChildren()
		{
			Children.Clear();
			var count = VisualTreeHelper.GetChildrenCount(this);
			if( count == 1 )
			{
				var itemsPresenter = VisualTreeHelper.GetChild(this, 0) as ItemsPresenter;
				if( null != itemsPresenter )
				{
					count = VisualTreeHelper.GetChildrenCount(itemsPresenter);
					if( count == 1 )
					{
						var panel = VisualTreeHelper.GetChild(itemsPresenter,0) as StackPanel;
						if( null != panel )
						{
							foreach( ContentPresenter contentPresenter in panel.Children )
							{
								var child = VisualTreeHelper.GetChild(contentPresenter,0);
								if( null != child &&
									child is INode )
								{
									Children.Add(child as INode);
								}
							}
						}
					}
				}
			}
		}

		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if( _prepareChildren )
			{
				_prepareChildren = false;
				PrepareChildren();
			}
		}
	}
}
