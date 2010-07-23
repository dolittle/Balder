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
using System.Linq;
using Balder.Collections;
using Balder.Display;
using Balder.Execution;
using Balder.Rendering;
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif

namespace Balder
{
	public class Container : Node, IHaveChildren, ICanBeVisible
	{
#if(DEFAULT_CONSTRUCTOR)
		public Container()
			: this(Runtime.Instance.Kernel.Get<IIdentityManager>())
		{
			
		}
#endif

		public Container(IIdentityManager identityManager)
			: base(identityManager)
		{
			Children = new NodeCollection(this);
			IsVisible = true;
		}

		public NodeCollection Children { get; private set; }

#if(SILVERLIGHT)
		public override void Prepare(Viewport viewport)
		{
			var query = from i in Items
						where i is INode
						select i as INode;

			Children.Merge(query);
			base.Prepare(viewport);
		}
#endif

		public Property<Container, bool> IsVisibleProperty =
			Property<Container, bool>.Register(c => c.IsVisible);
		public bool IsVisible
		{
			get { return IsVisibleProperty.GetValue(this); }
			set { IsVisibleProperty.SetValue(this,value); }
		}
	}
}
