using Balder.Core.Collections;
using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core.Rendering
{
	public class NodeRenderingService : INodeRenderingService
	{
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
			foreach (var node in nodes)
			{
				var world = Matrix.Identity;
				var view = viewport.View.ViewMatrix;
				var projection = viewport.View.ProjectionMatrix;

				PrepareForRendering(node, viewport, view, projection, world);
			}
		}


		public void Render(Viewport viewport, NodeCollection nodes)
		{
			var view = viewport.View.ViewMatrix;
			var projection = viewport.View.ProjectionMatrix;
			foreach (var node in nodes)
			{
				RenderNode(node, viewport, DetailLevel.Full);
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
			world = node.ActualWorld*world;
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