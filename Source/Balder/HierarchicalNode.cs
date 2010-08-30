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

#if(SILVERLIGHT)
using System.Collections.Specialized;
using System.Windows.Markup;
#endif

using Balder.Collections;
using Balder.Math;
using Balder.Rendering;


namespace Balder
{
#if(SILVERLIGHT)
	[ContentProperty("Children")]
#endif
	public class HierarchicalNode : Node, IHaveChildren
	{
		protected HierarchicalNode()
		{
			Children = new NodeCollection(this);
#if(SILVERLIGHT)
			Children.CollectionChanged += ChildrenChanged;
#endif
			BoundingSphereGenerated = false;
		}

		public NodeCollection Children { get; private set; }

		private BoundingSphere _boundingSphere;
		private bool _boundingSphereGenerated;
		private bool BoundingSphereGenerated
		{
			get { return _boundingSphereGenerated; }
			set { _boundingSphereGenerated = value; }
		}

		public override BoundingSphere BoundingSphere
		{
			get
			{
				if( !BoundingSphereGenerated )
				{
					BoundingSphereGenerated = true;
					GenerateBoundingSphere(this,this);
				}
				return _boundingSphere;
				
			}
			set
			{
				_boundingSphere = value;
				BoundingSphereGenerated = true;
			}
		}


		private static void GenerateBoundingSphere(INode root, INode current)
		{
			if( !root.Equals(current))
			{
				root.BoundingSphere = BoundingSphere.CreateMerged(root.BoundingSphere, current.BoundingSphere);
			}

			if( current is IHaveChildren )
			{
				foreach( var child in ((IHaveChildren)current).Children)
				{
					GenerateBoundingSphere(root, child);
				}
			}
		}


#if(SILVERLIGHT)
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
