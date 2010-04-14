#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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
using Balder.Core.Math;

namespace Balder.Core.Rendering
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
	}
}
