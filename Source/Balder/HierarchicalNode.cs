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

#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif
using Balder.Collections;
using Balder.Execution;
using Balder.Rendering;


namespace Balder
{
#if(SILVERLIGHT)
	[ContentProperty("Children")]
#endif
	public class HierarchicalNode : Node, IHaveChildren
	{
#if(DEFAULT_CONSTRUCTOR)
		protected HierarchicalNode()
			: this(Runtime.Instance.Kernel.Get<IIdentityManager>())
		{
		}
#endif

		protected HierarchicalNode(IIdentityManager identityManager)
			: base(identityManager)
		{
			Children = new NodeCollection(this);
#if(SILVERLIGHT)
			Children.CollectionChanged += ChildrenChanged;
#endif
		
		}

		public NodeCollection Children { get; private set; }

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
