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
using Balder.Lighting;
using Balder.Math;

namespace Balder.Rendering
{
	public class NodeRenderingService : INodeRenderingService
	{
		private readonly IRuntimeContext _runtimeContext;
		private readonly ILightCalculator _lightCalculator;

		private int _frameCounter;
		private bool _render;
		private readonly ShowMessage _showMessage;
		private readonly PrepareFrameMessage _prepareFrameMessage;

		public NodeRenderingService(IRuntimeContext runtimeContext, ILightCalculator lightCalculator)
		{
			_runtimeContext = runtimeContext;
			_lightCalculator = lightCalculator;
			runtimeContext.MessengerContext.SubscriptionsFor<PassiveRenderingSignal>().AddListener(this, PassiveRenderingSignaled);
			_render = true;
			_showMessage = new ShowMessage();
			_prepareFrameMessage = new PrepareFrameMessage();
		}


		private void PassiveRenderingSignaled(PassiveRenderingSignal signal)
		{
			_frameCounter = 2;
			_render = true;
		}

		#region INodeRenderingService Members

		public void Prepare(Viewport viewport, NodeCollection nodes)
		{
			foreach (var node in nodes)
			{
				PrepareNode(viewport, node);
			}
		}


		public void PrepareForRendering(Viewport viewport, NodeCollection nodes)
		{
			if (_runtimeContext.PassiveRendering && !_render)
			{
				_runtimeContext.Display.Paused = true;
			}
			else
			{
				_runtimeContext.Display.Paused = false;
			}

			if (_render)
			{
				_lightCalculator.Prepare(viewport, viewport.Scene.Lights);
				_runtimeContext.MessengerContext.Send(_prepareFrameMessage);
				foreach (var node in nodes)
				{
					var world = Matrix.Identity;
					var view = viewport.View.ViewMatrix;
					var projection = viewport.View.ProjectionMatrix;

					PrepareForRendering(node, viewport, view, projection, world);
				}
			}
		}

		public void PrepareNodeForRendering(INode node, Viewport viewport)
		{
			PrepareForRendering(node, viewport, viewport.View.ViewMatrix, viewport.View.ProjectionMatrix, Matrix.Identity);
		}


		public void Render(Viewport viewport, NodeCollection nodes)
		{
			if (_render)
			{
				viewport.Statistics.Reset();

				var detailLevel = DetailLevel.Full;
				if (_runtimeContext.PassiveRendering)
				{
					if (_frameCounter-- < 0)
					{
						_frameCounter = 0;
						_render = false;
						detailLevel = DetailLevel.Full;

					}
					else
					{
						detailLevel = _runtimeContext.PassiveRenderingMode == PassiveRenderingMode.BoundingBox ? DetailLevel.BoundingBox : DetailLevel.Full;

					}
				}
				foreach (var node in nodes)
				{
					RenderNode(node, viewport, detailLevel);
				}

				if (_runtimeContext.PassiveRendering)
				{
					_runtimeContext.MessengerContext.Send(_showMessage);
				}
			}
		}

		#endregion

		#region Private Helpers

		public void PrepareNode(Viewport viewport, INode node)
		{
			if (node is Node)
			{
				((Node)node).OnPrepare(viewport);
			}

			PrepareChildren(viewport, node);
			PrepareBoundingSphereForNode(node);
		}

		private void PrepareChildren(Viewport viewport, INode node)
		{
			if (node is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)node).Children)
				{
					PrepareNode(viewport, child);
				}
			}
		}

		private void PrepareBoundingSphereForNode(INode node)
		{
			if( node is Node )
			{
				((Node)node).OnPrepareBoundingSphere();
			}
		}

		private static void PrepareForRendering(INode node, Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			world = node.ActualWorld * world;
			node.RenderingWorld = world;
			node.BeforeRendering(viewport, view, projection, node.RenderingWorld);
			PrepareChildrenForRendering(node, world, viewport, view, projection);
		}

		private static void PrepareChildrenForRendering(INode node, Matrix world, Viewport viewport, Matrix view,
														Matrix projection)
		{
			if (node is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)node).Children)
				{
					PrepareForRendering(child, viewport, view, projection, world);
				}
			}
		}


		public void RenderNode(INode node, Viewport viewport, DetailLevel detailLevel)
		{
			if (!node.IsVisible())
			{
				return;
			}

			if( node.BoundingSphere.IsSet() )
			{
				var boundingSpherePosition = node.BoundingSphere.Center*node.RenderingWorld;
				var inView = viewport.View.IsInView(boundingSpherePosition, node.BoundingSphere.Radius);
				if( !inView )
				{
					return;
				}
			}

			if (node is ICanRender)
			{
				if( null != node.Statistics )
				{
					node.Statistics.BeginNodeTiming();	
				}
				
				((ICanRender) node).Render(viewport, detailLevel);
				viewport.Statistics.RenderedNodes++;

				if( null != node.Statistics )
				{
					node.Statistics.EndNodeTiming();	
				}
				
				((ICanRender)node).RenderDebugInfo(viewport, detailLevel);
			}

			if( null != node.Statistics )
			{
				node.Statistics.BeginChildrenTiming();	
			}
			
			RenderChildren(node, viewport, detailLevel);

			if( null != node.Statistics )
			{
				node.Statistics.EndNodeTiming();	
			}
		}


		private void RenderChildren(INode node, Viewport viewport, DetailLevel detailLevel)
		{
			if (node is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)node).Children)
				{
					RenderNode(child, viewport, detailLevel);
				}
			}
		}

		#endregion
	}
}