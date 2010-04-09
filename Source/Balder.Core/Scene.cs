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
using Balder.Core.Collections;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Lighting;
using Balder.Core.Objects.Flat;
using Matrix = Balder.Core.Math.Matrix;

namespace Balder.Core
{
	public class Scene
	{
		public static int NodeCount = 0;
		

		private readonly NodeCollection _renderableNodes;
		private readonly NodeCollection _flatNodes;
		private readonly NodeCollection _environmentalNodes;
		private readonly NodeCollection _lights;
		private readonly NodeCollection _allNodes;

		public Color AmbientColor;

		public Scene()
		{
			_renderableNodes = new NodeCollection();
			_flatNodes = new NodeCollection();
			_environmentalNodes = new NodeCollection();
			_lights = new NodeCollection();
			_allNodes = new NodeCollection();

			AmbientColor = Color.FromArgb(0xff, 0x1f, 0x1f, 0x1f);
		}

		public void AddNode(Node node)
		{
			node.Scene = this;
			if (node is RenderableNode)
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

		public void RemoveNode(Node node)
		{
			if (node is RenderableNode)
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

		internal void Render(Viewport viewport)
		{
			NodeCount = 0;
			
			var view = viewport.View.ViewMatrix;
			var projection = viewport.View.ProjectionMatrix;
			lock( _allNodes )
			{
				foreach( var node in _allNodes )
				{
					var actualNode = node as Node;
					if (null != actualNode)
					{
						Prepare(actualNode);
						var world = Matrix.Identity;
						PrepareRender(actualNode, viewport, view, projection, world);
					}
				}
			}
			
			lock (_renderableNodes)
			{
				foreach (RenderableNode node in _renderableNodes)
				{
					RenderNode(node, viewport, view, projection);
				}
			}
		}

		private static void Prepare(Node node)
		{
			node.OnPrepare();
			foreach (var child in node.Children)
			{
				if( child is Node )
				{
					Prepare((Node)child);	
				}
			}
		}

		private static void PrepareRender(INode node, Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if( node is Node && !((Node)node).IsVisible )
			{
				return;
			}
			world = node.ActualWorld * world;
			node.RenderingWorld = world;
			
			node.BeforeRendering(viewport,view,projection,node.RenderingWorld);
			if( node is Node )
			{
				foreach (var child in ((Node)node).Children)
				{
					PrepareRender(child, viewport, view, projection, world);
				}
					
			}
		}

		private static void RenderNode(RenderableNode node, Viewport viewport, Matrix view, Matrix projection)
		{
			if( !node.IsVisible )
			{
				return;
			}

			NodeCount++;
			
			node.Render(viewport, view, projection, node.RenderingWorld);

			foreach (var child in node.Children)
			{
				if (child is RenderableNode)
				{
					RenderNode((RenderableNode)child, viewport, view, projection);
				}
			}
			node.RenderDebugInfo(viewport, view, projection, node.RenderingWorld);
		}
	}
}
