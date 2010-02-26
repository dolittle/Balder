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

using Balder.Core.Debug;
using Balder.Core.View;

namespace Balder.Core.Display
{
	/// <summary>
	/// Represents a viewport within a display - the Viewport is a 2D rectangle representing a clipping region.
	/// The viewport also holds the view used to render and also holds the scene that contains the objects that
	/// will be rendered within the viewport
	/// </summary>
	public class Viewport
	{
		/// <summary>
		/// Creates a viewport
		/// </summary>
		public Viewport()
		{
			DebugInfo = new DebugInfo();
		}

		/// <summary>
		/// Get or set the x position in pixelsof the viewport within the display, where 0 is the left
		/// </summary>
		public int XPosition { get; set; }

		/// <summary>
		/// Get or set the y position in pixels of the viewport within the display, where 0 is the top.
		/// </summary>
		public int YPosition { get; set; }

		/// <summary>
		/// Get or set the width in pixels of the viewport within the display
		/// </summary>
		public int Width { get; set; }

		/// <summary>
		/// Get or set the height in pixels of the viewport within the display
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Get or set the scene to use during rendering
		/// </summary>
		public Scene Scene { get; set; }

		/// <summary>
		/// Get or set the view to be used during rendering
		/// </summary>
		public IView View { get; set; }

		/// <summary>
		/// Get or set the debug info for the Viewport
		/// </summary>
		public DebugInfo DebugInfo { get; set; }

		/// <summary>
		/// Get the aspect ratio for the viewport
		/// </summary>
		public float AspectRatio { get { return ((float)Width) / ((float)Height); } }
	}
}
