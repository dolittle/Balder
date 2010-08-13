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

using Balder.Collections;
using Balder.Display;
using Balder.Execution;
using Balder.Lighting;
using Balder.Objects.Flat;
using Balder.Rendering;
#if(SILVERLIGHT)
using System.Windows;
#endif
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif

namespace Balder
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

#if(DEFAULT_CONSTRUCTOR)
		/// <summary>
		/// Construct a scene
		/// </summary>
		public Scene()
			: this(Runtime.Instance.Kernel.Get<IRuntimeContext>(),
			Runtime.Instance.Kernel.Get<INodeRenderingService>())
		{
			IsPaused = false;
		}
#endif

		/// <summary>
		/// Construct a scene - specifying the rendering service
		/// </summary>
		/// <param name="runtimeContext">RuntimeContext that the Scene belongs to</param>
		/// <param name="nodeRenderingService">NodeRenderingService to use</param>
		public Scene(IRuntimeContext runtimeContext, INodeRenderingService nodeRenderingService)
		{
			RuntimeContext = runtimeContext;
			_nodeRenderingService = nodeRenderingService;
			_renderableNodes = new NodeCollection(this);
			_flatNodes = new NodeCollection(this);
			_environmentalNodes = new NodeCollection(this);
			_lights = new NodeCollection(this);
			_allNodes = new NodeCollection(this);
			IsPaused = false;

			AmbientColor = Color.FromArgb(0xff, 0x1f, 0x1f, 0x1f);
		}

		public IRuntimeContext RuntimeContext { get; private set; }
		public Game Game { get; internal set; }

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

			RuntimeContext.SignalRendering();
#if(SILVERLIGHT)
			if( null != Game && node is UIElement )
			{
				Game.AddChildFromProgrammaticApproach(node);
			}
#endif
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

		public bool IsPaused { get; set; }

		public void Render(Viewport viewport)
		{
			if( IsPaused )
			{
				// Todo: Figure out a better way to pause/halt everything - hate to have this value floating around everywhere
				// besides - its altering the state of an object within Viewport - not really its concern!
				viewport.Display.Halted = true;
				return;
			} 
			lock( _renderableNodes )
			{
				lock(_allNodes)
				{
					_nodeRenderingService.PrepareForRendering(viewport, _allNodes);
				}

				_nodeRenderingService.Render(viewport, _renderableNodes);
			}
		}

		public void Prepare(Viewport viewport)
		{
			if (IsPaused)
			{
				return;
			}
			lock (_allNodes)
			{
				_nodeRenderingService.Prepare(viewport, _allNodes);
			}
		}
	}
}
