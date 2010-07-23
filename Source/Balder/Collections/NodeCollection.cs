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
using System.Collections.Generic;
using System.Collections.ObjectModel;
#else
using System.Collections.Generic;
#endif

namespace Balder.Collections
{
#if(SILVERLIGHT)
	/// <summary>
	/// Collection of nodes	- any node type can be added to this collection type
	/// </summary>
	public class NodeCollection : ObservableCollection<INode>
#else
	public class NodeCollection : List<INode>
#endif
	{
		private readonly object _owner;

		public NodeCollection(object owner)
		{
			_owner = owner;
		}

		public new void Add(INode node)
		{
			if (node is Node && _owner is INode)
			{
				((Node)node).Parent = _owner as INode;
			}
			base.Add(node);
		}

		public void AddRange(IEnumerable<INode> nodes)
		{
			foreach( var node in nodes )
			{
				Add(node);
			}
		}

		public void Merge(IEnumerable<INode> nodes)
		{
			foreach( var node in nodes )
			{
				if( !Contains(node) )
				{
					Add(node);
				}
			}
		}

	}
}