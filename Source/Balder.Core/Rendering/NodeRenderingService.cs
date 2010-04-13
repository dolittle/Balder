using Balder.Core.Collections;
using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core.Rendering
{
	public class NodeRenderingService : INodeRenderingService
	{
		public void Prepare(Viewport viewport, NodeCollection nodes)
		{
			foreach (var node in nodes)
			{
				Prepare(node);
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
				RenderNode(node, viewport, view, projection);
			}
		}

		#region Private Helpers
		private static void Prepare(INode node)
		{
			if (node is Node)
			{
				((Node)node).OnPrepare();
			}

			PrepareChildren(node);
		}

		private static void PrepareChildren(INode node)
		{
			if (node is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)node).Children)
				{
					Prepare(child);
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

		private static void PrepareChildrenForRendering(INode node, Matrix world, Viewport viewport, Matrix view, Matrix projection)
		{
			if (node is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)node).Children)
				{
					PrepareForRendering(child, viewport, view, projection, world);
				}
			}
		}


		private static void RenderNode(INode node, Viewport viewport, Matrix view, Matrix projection)
		{
			if (!node.IsVisible())
			{
				return;
			}

			if (node is ICanRender)
			{
				((ICanRender)node).Render(viewport, view, projection, node.RenderingWorld);
				((ICanRender)node).RenderDebugInfo(viewport, view, projection, node.RenderingWorld);
			}
			RenderChildren(node, viewport, view, projection);
		}


		private static void RenderChildren(INode node, Viewport viewport, Matrix view, Matrix projection)
		{
			if (node is IHaveChildren)
			{
				foreach (var child in ((IHaveChildren)node).Children)
				{
					RenderNode(child, viewport, view, projection);
				}
			}
		}
		#endregion
	}
}