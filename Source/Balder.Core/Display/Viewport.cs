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
using Balder.Core.Math;
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
		private const float MinDepth = 0f;
		private const float MaxDepth = 1f;

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


		/// <summary>
		/// Unproject a 2D coordinate into 3D. Basically convert a 2D point with depth
		/// information (Z) into a real 3D coordinate.
		/// </summary>
		/// <param name="source">Point to unproject</param>
		/// <param name="projection">Projection matrix</param>
		/// <param name="view">View matrix</param>
		/// <param name="world">World matrix</param>
		/// <returns>Unprojected 3D coordinate</returns>
		public Vector Unproject(Vector source, Matrix projection, Matrix view, Matrix world)
		{
			var combinedMatrix = (world * view) * projection;
			var matrix = Matrix.Invert(combinedMatrix);
			source.X = (((source.X - XPosition) / ((float)Width)) * 2f) - 1f;
			source.Y = -((((source.Y - YPosition) / ((float)Height)) * 2f) - 1f);
			source.Z = (source.Z - MinDepth) / (MaxDepth - MinDepth);
			source.W = 1f;
			var vector = Vector.Transform(source, matrix);
			var a = (((source.X * matrix[0, 3]) + (source.Y * matrix[1, 3])) + (source.Z * matrix[2, 3])) + matrix[3, 3];
			if (!WithinEpsilon(a, 1f))
			{
				vector = (Vector)(vector / (a * 2f));
			}
			return vector;
		}

		private static bool WithinEpsilon(float a, float b)
		{
			var num = a - b;
			return ((-1.401298E-45f <= num) && (num <= float.Epsilon));
		}
	}
}
