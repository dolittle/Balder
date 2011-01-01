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

namespace Balder.Rendering
{
	/// <summary>
	/// Represents rendering capabilities for nodes
	/// </summary>
	public interface INodeRenderingService
	{
		/// <summary>
		/// Prepare nodes
		/// </summary>
		/// <param name="viewport">Viewport that the nodes are being rendered to</param>
		/// <param name="nodes">Nodes to prepare</param>
		void Prepare(Viewport viewport, NodeCollection nodes);

		/// <summary>
		/// Prepare specific node and its children
		/// </summary>
		/// <param name="viewport">Viewport to prepare it for</param>
		/// <param name="node">Node to prepare</param>
		void PrepareNode(Viewport viewport, INode node);

		/// <summary>
		/// Prepare nodes for rendering
		/// </summary>
		/// <param name="viewport">Viewport that the nodes are being rendered to</param>
		/// <param name="nodes">Nodes to prepare</param>
		void PrepareForRendering(Viewport viewport, NodeCollection nodes);

		/// <summary>
		/// Render nodes
		/// </summary>
		/// <param name="viewport">Viewport that the nodes are being rendered to</param>
		/// <param name="nodes">Nodes to render</param>
		void Render(Viewport viewport, NodeCollection nodes);

		/// <summary>
		/// Prepare specific node for rendering
		/// </summary>
		/// <param name="node">Node to prepare</param>
		/// <param name="viewport">Viewport to prepare it for rendering into</param>
		void PrepareNodeForRendering(INode node, Viewport viewport);

		/// <summary>
		/// Render a specific node with a given detail level
		/// </summary>
		/// <param name="node">Node to render</param>
		/// <param name="viewport">Viewport to render to</param>
		/// <param name="detailLevel"><see cref="DetailLevel">DetailLevel</see> to use for rendering</param>
		void RenderNode(INode node, Viewport viewport, DetailLevel detailLevel);
	}
}