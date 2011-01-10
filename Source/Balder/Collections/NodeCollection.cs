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
using System.Collections.Generic;
using System.Collections.ObjectModel;
#else
using System.Collections.Generic;
#endif

namespace Balder.Collections
{
#if(SILVERLIGHT)
	/// <summary>
	/// Represents a collection holding <see cref="INode">nodes</see>
	/// </summary>
	public class NodeCollection : ObservableCollection<INode>
#else
	public class NodeCollection : List<INode>
#endif
	{
		private readonly object _owner;
		private Scene _ownersScene;

		/// <summary>
		/// Initializes a new instance of <see cref="NodeCollection"/> with a specific owner
		/// </summary>
		/// <param name="owner">Owner of the collection</param>
		public NodeCollection(object owner)
		{
			_owner = owner;
		}


		/// <summary>
		/// Add a node to the collection
		/// </summary>
		/// <param name="node"><see cref="INode"/> to add</param>
		public new void Add(INode node)
		{
			if (node is Node && _owner is INode)
			{
				((Node) node).Scene = OwnersScene;
				((Node)node).Parent = _owner as INode;
			}
			base.Add(node);
		}

		/// <summary>
		/// Add a range of nodes to the collection
		/// </summary>
		/// <param name="nodes"><see cref="IEnumerable{INode}">Nodes</see> to add</param>
		public void AddRange(IEnumerable<INode> nodes)
		{
			foreach( var node in nodes )
			{
				Add(node);
			}
		}

		/// <summary>
		/// Merge collection with a range of nodes
		/// </summary>
		/// <param name="nodes"><see cref="IEnumerable{INode}">Nodes</see> to merge into the collection</param>
		/// <remarks>
		/// Nodes are added if the collection does not already contain a node
		/// </remarks>
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

		private Scene OwnersScene
		{
			get
			{
				if (null == _ownersScene)
				{
					if (_owner is Node)
					{
						_ownersScene = ((Node)_owner).Scene;
					}
					if (_owner is Scene)
					{
						_ownersScene = (Scene)_owner;
					}
				}

				return _ownersScene;
			}
		}


	}
}