using Balder.Collections;
using Balder.Display;
using Balder.Execution;
using Balder.Math;

namespace Balder.Rendering
{
	public class NodeRenderingService : INodeRenderingService
	{
		private readonly IRuntimeContext _runtimeContext;

		private int _frameCounter;
		private bool _render;
		private readonly ShowMessage _showMessage;
		private readonly PrepareFrameMessage _prepareFrameMessage;

		public NodeRenderingService(IRuntimeContext runtimeContext)
		{
			_runtimeContext = runtimeContext;
			Messenger.DefaultContext.SubscriptionsFor<PassiveRenderingSignal>().AddListener(this, PassiveRenderingSignaled);
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
				Prepare(viewport, node);
			}
		}


		public void PrepareForRendering(Viewport viewport, NodeCollection nodes)
		{
			if( _runtimeContext.PassiveRendering && !_render )
			{
				viewport.Display.Paused = true;
			} else
			{
				viewport.Display.Paused = false;
			}

			if (_render)
			{
				Messenger.DefaultContext.Send(_prepareFrameMessage);
				foreach (var node in nodes)
				{
					var world = Matrix.Identity;
					var view = viewport.View.ViewMatrix;
					var projection = viewport.View.ProjectionMatrix;

					PrepareForRendering(node, viewport, view, projection, world);
				}
			}
		}


		public void Render(Viewport viewport, NodeCollection nodes)
		{
			if (_render)
			{
				var detailLevel = DetailLevel.Full;
				if( _runtimeContext.PassiveRendering )
				{
					if( _frameCounter-- < 0 )
					{
						_frameCounter = 0;
						_render = false;
						detailLevel = DetailLevel.Full;
						
					} else
					{
						detailLevel = _runtimeContext.PassiveRenderingMode==PassiveRenderingMode.BoundingBox?DetailLevel.BoundingBox:DetailLevel.Full;
						
					}
				}
				foreach (var node in nodes)
				{
					RenderNode(node, viewport, detailLevel);
				}

				if (_runtimeContext.PassiveRendering)
				{
					Messenger.DefaultContext.Send(_showMessage);
				}
			}
		}

		#endregion

		#region Private Helpers

		private static void Prepare(Viewport viewport, INode node)
		{
			if (node is Node)
			{
				((Node) node).OnPrepare(viewport);
			}

			PrepareChildren(viewport, node);
		}

		private static void PrepareChildren(Viewport viewport, INode node)
		{
			if (node is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren) node).Children)
				{
					Prepare(viewport, child);
				}
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
				foreach (var child in ((IHaveChildren) node).Children)
				{
					PrepareForRendering(child, viewport, view, projection, world);
				}
			}
		}


		private static void RenderNode(INode node, Viewport viewport, DetailLevel detailLevel)
		{
			if (!node.IsVisible())
			{
				return;
			}

			if (node is ICanRender)
			{
				((ICanRender) node).Render(viewport, detailLevel);
				((ICanRender)node).RenderDebugInfo(viewport, detailLevel);
			}
			RenderChildren(node, viewport, detailLevel);
		}


		private static void RenderChildren(INode node, Viewport viewport, DetailLevel detailLevel)
		{
			if (node is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren) node).Children)
				{
					RenderNode(child, viewport, detailLevel);
				}
			}
		}

		#endregion
	}
}