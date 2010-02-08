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
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using Balder.Core;
using Balder.Core.Helpers;

namespace Balder.Silverlight.Controls
{
	public class NodesControl : RenderableNode
	{
		public NodesControl()
		{
			Loaded += NodesControlLoaded;
		}

		private void NodesControlLoaded(object sender, RoutedEventArgs e)
		{
			PopulateFromItemsSource();
		}

		public static readonly DependencyProperty<NodesControl, DataTemplate> NodeTemplateProperty =
			DependencyProperty<NodesControl, DataTemplate>.Register(n => n.NodeTemplate);
		public DataTemplate NodeTemplate
		{
			get { return NodeTemplateProperty.GetValue(this); }
			set { NodeTemplateProperty.SetValue(this, value); }
		}


		public static new readonly DependencyProperty<NodesControl, IEnumerable> ItemsSourceProperty =
			DependencyProperty<NodesControl, IEnumerable>.Register(n => n.ItemsSource);

		private IEnumerable _itemsSource;
		public new IEnumerable ItemsSource
		{
			get { return ItemsSourceProperty.GetValue(this); }
			set
			{
				HandlePreviousItemsSource();
				_itemsSource = value;
				ItemsSourceProperty.SetValue(this, value);
				HandleNewItemsSource();

			}
		}

		public static readonly DependencyProperty<NodesControl, INodeModifier> ModifierProperty =
			DependencyProperty<NodesControl, INodeModifier>.Register(n => n.Modifier);
		
		public INodeModifier Modifier
		{
			get { return ModifierProperty.GetValue(this); }
			set { ModifierProperty.SetValue(this,value); }
		}

		private void HandleNewItemsSource()
		{
			var itemsSource = _itemsSource as INotifyCollectionChanged;
			if( null != itemsSource )
			{
				itemsSource.CollectionChanged += ItemsSourceCollectionChanged;
			}
		}

		private void HandlePreviousItemsSource()
		{
			var itemsSource = _itemsSource as INotifyCollectionChanged;
			if (null != itemsSource)
			{
				itemsSource.CollectionChanged -= ItemsSourceCollectionChanged;
			}
		}

		private void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch( e.Action )
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach( var item in e.NewItems )
						{
							LoadAndAddChild(item);
						}
					}
					break;

				case NotifyCollectionChangedAction.Remove:
					{
						foreach( var item in e.OldItems )
						{
							RemoveChildBasedOnItem(item);
						}
					}
					break;

				case NotifyCollectionChangedAction.Reset:
					{
						Children.Clear();
					}
					break;

			}
		}


		private void PopulateFromItemsSource()
		{
			foreach (var item in _itemsSource)
			{
				LoadAndAddChild(item);
			}
		}

		private void LoadAndAddChild(object item)
		{
			if (null != NodeTemplate)
			{
				var content = NodeTemplate.LoadContent() as Node;
				if (null == content)
				{
					throw new ArgumentException("Content of the template for NodeTemplate must be a derivative of Node");
				}

				var modifier = Modifier;
				if( null != modifier )
				{
					var index = 0;
					if( _itemsSource is IList )
					{
						index = ((IList) _itemsSource).IndexOf(item);
					}
					
					
					modifier.Apply(content, index, item);
				}
				content.DataContext = item;
				Children.Add(content);
			}
		}

		private void RemoveChildBasedOnItem(object item)
		{
			var nodesToRemove = new List<Node>();
			foreach( var node in Children )
			{
				if( node.DataContext.Equals(item))
				{
					nodesToRemove.Add(node);
				}
			}

			foreach( var node in nodesToRemove )
			{
				Children.Remove(node);
			}
		}
	}
}
