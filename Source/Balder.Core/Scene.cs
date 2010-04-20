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

using System.Windows;
using Balder.Core.Collections;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Lighting;
using Balder.Core.Objects.Flat;
using Balder.Core.Rendering;

namespace Balder.Core
{
	/// <summary>
	/// Scene represents the root for all elements in a 3D world.
	/// One can add any node implementing the INode interface to the
	/// scene and the scene will handle the management and rendering 
	/// of the nodes
	/// </summary>
#if(SILVERLIGHT)
	public class Scene : FrameworkElement
#else
	public class Scene
#endif
	{
		public static int NodeCount = 0;

		private readonly INodeRenderingService _nodeRenderingService;
		private readonly NodeCollection _renderableNodes;
		private readonly NodeCollection _flatNodes;
		private readonly NodeCollection _environmentalNodes;
		private readonly NodeCollection _lights;
		private readonly NodeCollection _allNodes;

		/// <summary>
		/// Ambient color for the scene - default is set to #1f1f1f (RGB)
		/// </summary>
		public Color AmbientColor;

		/// <summary>
		/// Construct a scene
		/// </summary>
		public Scene()
			: this(Runtime.Instance.Kernel.Get<INodeRenderingService>())
		{
			
		}

		/// <summary>
		/// Construct a scene - specifying the rendering service
		/// </summary>
		/// <param name="nodeRenderingService">NodeRenderingService to use</param>
		public Scene(INodeRenderingService nodeRenderingService)
		{
			_nodeRenderingService = nodeRenderingService;
			_renderableNodes = new NodeCollection(this);
			_flatNodes = new NodeCollection(this);
			_environmentalNodes = new NodeCollection(this);
			_lights = new NodeCollection(this);
			_allNodes = new NodeCollection(this);

			AmbientColor = Color.FromArgb(0xff, 0x1f, 0x1f, 0x1f);
		}


		/// <summary>
		/// Add a node to the scene
		/// </summary>
		/// <param name="node">Node to add</param>
		public void AddNode(INode node)
		{
			node.Scene = this;
			if (node is ICanBeVisible)
			{
				lock (_renderableNodes)
				{
					_renderableNodes.Add(node);

				}
				if (node is Sprite)
				{
					lock (_flatNodes)
					{
						_flatNodes.Add(node);
					}
				}
			}
			else
			{
				lock (_environmentalNodes)
				{
					_environmentalNodes.Add(node);
				}
				if( node is ILight )
				{
					lock( _lights )
					{
						_lights.Add(node);
					}
				}
			}
			lock (_allNodes)
			{
				_allNodes.Add(node);
			}

			Runtime.Instance.SignalRenderingForObject(this);
		}


		/// <summary>
		/// Remove a specific node from the scene
		/// </summary>
		/// <param name="node">Node to remove</param>
		public void RemoveNode(INode node)
		{
			if (node is ICanBeVisible)
			{
				lock (_renderableNodes)
				{
					_renderableNodes.Remove(node);

				}
				if (node is Sprite)
				{
					lock (_flatNodes)
					{
						_flatNodes.Remove(node);
					}
				}
			}
			else
			{
				lock (_environmentalNodes)
				{
					_environmentalNodes.Remove(node);
				}
				if (node is ILight)
				{
					lock (_lights)
					{
						_lights.Remove(node);
					}
				}
			}
			lock (_allNodes)
			{
				_allNodes.Remove(node);
			}
		}

		/// <summary>
		/// Clear out all nodes in the scene
		/// </summary>
		public void Clear()
		{
			lock (_renderableNodes)
			{
				_renderableNodes.Clear();
			}

			lock (_flatNodes)
			{
				_flatNodes.Clear();
			}

			lock (_environmentalNodes)
			{
				_environmentalNodes.Clear();
			}

			lock (_lights)
			{
				_lights.Clear();
			}

			lock (_allNodes)
			{
				_allNodes.Clear();
			}
		}

		/// <summary>
		/// Gets all the renderable nodes in the scene
		/// </summary>
		public NodeCollection RenderableNodes { get { return _renderableNodes; } }

		/// <summary>
		/// Gets all the lights in the scene
		/// </summary>
		public NodeCollection Lights { get { return _lights; } }


		public void Render(Viewport viewport)
		{
			lock( _renderableNodes )
			{
				_nodeRenderingService.PrepareForRendering(viewport, _renderableNodes);
				_nodeRenderingService.Render(viewport, _renderableNodes);
			}
		}

		public void Prepare(Viewport viewport)
		{
			lock (_allNodes)
			{
				_nodeRenderingService.Prepare(viewport, _allNodes);
			}
		}
	}
}
