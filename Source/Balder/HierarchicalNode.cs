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

#if(XAML)
using System.Collections.Specialized;
using System.Windows.Markup;
#endif

using Balder.Collections;
using Balder.Math;
using Balder.Rendering;


namespace Balder
{
#if(XAML)
	[ContentProperty("Children")]
#endif
	public class HierarchicalNode : Node, IHaveChildren
	{
		protected HierarchicalNode()
		{
			Children = new NodeCollection(this);
#if(XAML)
			Children.CollectionChanged += ChildrenChanged;
#endif
		}

		public NodeCollection Children { get; private set; }


		public override void PrepareBoundingObject()
		{
			foreach( var child in Children )
			{
				GenerateBoundingObject(this, child);
			}

			base.PrepareBoundingObject();
		}


		private static void GenerateBoundingObject(INode root, INode current)
		{
			if (!root.Equals(current))
			{
				if( current is Node )
				{
					((Node)current).PrepareBoundingObject();
				}
                root.BoundingObject.Include(current.BoundingObject);
			}

			if (current is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)current).Children)
				{
					GenerateBoundingObject(root, child);
				}
			}
		}

#if(XAML)
		private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach (var item in e.NewItems)
						{
							Items.Add(item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					{
						foreach (var item in e.OldItems)
						{
							Items.Remove(item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					{
						Items.Clear();
					}
					break;
			}
		}
#endif

	}
}
