#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
#if(SILVERLIGHT)
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Balder.Display;
using Balder.Execution;
using Matrix=Balder.Math.Matrix;

namespace Balder.Controls
{
	public class NodesControl : Container
	{
		public static BubbledEvent<NodesControl, BubbledEventHandler> ItemsPreparedEvent =
			BubbledEvent<NodesControl, BubbledEventHandler>.Register(n => n.ItemsPrepared);

		public event BubbledEventHandler ItemsPrepared = (s, e) => { };
		private readonly List<ContentPresenter> _contentPresenters;

		public NodesControl()
		{
			var binding = new Binding {Source = this, Path = new PropertyPath("ItemsSource")};
			SetBinding(ItemsSourceChangedProperty, binding);
			_contentPresenters = new List<ContentPresenter>();
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
				nodesControl._contentPresenters.Clear();
				if( null != e.OldValue && e.OldValue is INotifyCollectionChanged )
				{
					var notifyingItemsSource = e.OldValue as INotifyCollectionChanged;
					notifyingItemsSource.CollectionChanged -= nodesControl.ItemsSourceCollectionChanged;
				}
				
				nodesControl.InvalidatePrepare();
			}
		}

		public static readonly Property<NodesControl, DataTemplate> NodeTemplateProperty =
			Property<NodesControl, DataTemplate>.Register(n => n.NodeTemplate);
		public DataTemplate NodeTemplate
		{
			get { return base.ItemTemplate; }
			set { base.ItemTemplate = value; }
		}

		public static readonly Property<NodesControl, INodeModifier> ModifierProperty =
			Property<NodesControl, INodeModifier>.Register(n => n.Modifier);
		public INodeModifier Modifier
		{
			get { return ModifierProperty.GetValue(this); }
			set { ModifierProperty.SetValue(this, value); }
		}


		#region Hidden Properties
		private new ItemsPanelTemplate ItemsPanel { get; set; }
		private new DataTemplate ItemTemplate { get; set; }
		#endregion



		private void PrepareChildren()
		{
			foreach( var contentPresenter in _contentPresenters )
			{
				var child = VisualTreeHelper.GetChild(contentPresenter, 0);
				if (null != child &&
					child is INode)
				{
					Children.Add(child as INode);
				}
			}

			OnContentPrepared();
		}


		private int _expectedCount;
		private int _childCount;

		protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
		{
			_childCount = 0;
			_expectedCount = -1;
			Children.Clear();

			if( null != ItemsSource && ItemsSource is ICollection )
			{
				_expectedCount = ((ICollection) ItemsSource).Count;
			}

			InvalidatePrepare();
			
			base.OnItemsChanged(e);
		}


		protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
		{
			if (element is ContentPresenter)
			{
				_contentPresenters.Add(element as ContentPresenter);
			}

			_childCount++;
			if( _childCount == _expectedCount )
			{
				ItemsPreparedEvent.Raise(this, this, BubbledEventArgs.Empty);
			}
			base.PrepareContainerForItemOverride(element, item);
		}

		private void HandleItemsSource()
		{
			if( null != ItemsSource &&
				ItemsSource is INotifyCollectionChanged )
			{
				var notifyingItemsSource = ItemsSource as INotifyCollectionChanged;
				notifyingItemsSource.CollectionChanged += ItemsSourceCollectionChanged;
			}
		}

		void ItemsSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			InvalidatePrepare();
		}



		private void HandleModifier()
		{
			if( null == Modifier )
			{
				return;
			}
			var nodeIndex = 0;
			foreach( var child in Children )
			{
				Modifier.Apply(child,nodeIndex,DataContext);
				nodeIndex++;
			}
		}

		public override void Prepare(Viewport viewport)
		{
			PrepareChildren();
			HandleItemsSource();
			HandleModifier();
			base.Prepare(viewport);
		}

		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
		}
	}
}
#endif