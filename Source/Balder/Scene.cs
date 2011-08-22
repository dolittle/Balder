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

using Balder.Collections;
using Balder.Display;
using Balder.Execution;
using Balder.Lighting;
using Balder.Objects.Flat;
using Balder.Rendering;
#if(XAML)
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
#if(XAML)
	public class Scene : FrameworkElement,
#else
	public class Scene
#endif
		IHavePropertyContainer
	{


		internal int AmbientAsInt;
		public static int NodeCount = 0;

		private readonly INodeRenderingService _nodeRenderingService;

		private readonly NodeCollection _renderableNodes;
		private readonly NodeCollection _flatNodes;
		private readonly NodeCollection _environmentalNodes;
		private readonly NodeCollection _lights;
		private readonly NodeCollection _allNodes;

		public static readonly Property<Scene, Color> AmbientColorProperty =
			Property<Scene, Color>.Register(s => s.AmbientColor);

		IPropertyContainer _propertyContainer;
		public IPropertyContainer PropertyContainer
		{
			get
			{
				if (_propertyContainer == null)
					_propertyContainer = new PropertyContainer(this);

				return _propertyContainer;
			}
		}

		/// <summary>
		/// Ambient color for the scene - default is set to black
		/// </summary>
		public Color AmbientColor
		{
			get { return AmbientColorProperty.GetValue(this); }
			set
			{
				AmbientColorProperty.SetValue(this, value);
				AmbientAsInt = value.ToInt();
			}
		}

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

			AmbientColor = Color.FromArgb(0xff, 0x10, 0x10, 0x10);
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
				_renderableNodes.Add(node);
				if (node is Sprite)
				{
					_flatNodes.Add(node);
				}
			}
			else
			{
				_environmentalNodes.Add(node);
				if (node is ILight)
				{
					_lights.Add(node);
				}
			}
			_allNodes.Add(node);

			RuntimeContext.SignalRendering();
#if(XAML)
			if (null != Game && node is UIElement)
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
				_renderableNodes.Remove(node);
				if (node is Sprite)
				{
					_flatNodes.Remove(node);
				}
			}
			else
			{
				_environmentalNodes.Remove(node);
				if (node is ILight)
				{
					_lights.Remove(node);
				}
			}
			_allNodes.Remove(node);

#if(XAML)
			if (null != Game && node is UIElement)
			{
				Game.RemoveNodeFromProgrammaticApproach(node);
			}
#endif

		}

		/// <summary>
		/// Clear out all nodes in the scene
		/// </summary>
		public void Clear()
		{
			_renderableNodes.Clear();
			_flatNodes.Clear();
			_environmentalNodes.Clear();
			_lights.Clear();
			_allNodes.Clear();
#if(XAML)
			Game.ClearAllProgrammaticNodes();
#endif
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
			if (IsPaused)
			{
				// Todo: Figure out a better way to pause/halt everything - hate to have this value floating around everywhere
				// besides - its altering the state of an object within Viewport - not really its concern!
				viewport.Display.Halted = true;
				return;
			}
			_nodeRenderingService.PrepareForRendering(viewport, _allNodes);

			_nodeRenderingService.Render(viewport, _renderableNodes);
		}

		public void Prepare(Viewport viewport)
		{
			if (IsPaused)
			{
				return;
			}
			_nodeRenderingService.Prepare(viewport, _allNodes);
		}
	}
}
