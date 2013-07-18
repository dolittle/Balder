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
using Balder.Display;
using Balder.Math;

namespace Balder.View
{
	/// <summary>
	/// Represents a view that is used to view a 3D scene
	/// </summary>
	public interface IView
	{
        /// <summary>
        /// Gets or sets the position of the view
        /// </summary>
		Coordinate Position { get; set; }

		/// <summary>
		/// Gets the view <see cref="Matrix"/> for the view
		/// </summary>
		/// <remarks>
		/// The view <see cref="Matrix"/> represents the translation of vectors in 
		/// viewspace
		/// </remarks>
		Matrix ViewMatrix { get; }

		/// <summary>
		/// Gets the projection <see cref="Matrix"/> - the conversion of 3D to 2D
		/// </summary>
		Matrix ProjectionMatrix { get; }

		/// <summary>
		/// Gets or sets the near clipping value - the point closest to the screen before
		/// clipping should occur
		/// </summary>
		float Near { get; set; }

		/// <summary>
		/// Gets or sets the far clipping value - the point furthest away from the screen before
		/// clipping should occur
		/// </summary>
		float Far { get; set; }

		/// <summary>
		/// Gets or sets the depthdivisor typically used by Z buffering
		/// </summary>
		float DepthDivisor { get; }

		/// <summary>
		/// Gets or sets the depth multiplier, typically used by Z buffering
		/// </summary>
		float DepthMultiplier { get; }

		/// <summary>
		/// Gets the point in depth that represents Zero
		/// </summary>
		float DepthZero { get; }

		/// <summary>
		/// Check if a vector is within view
		/// </summary>
		/// <param name="vector"><see cref="Vector"/> to check</param>
		/// <returns>True if in view, false if not</returns>
		bool IsInView(Vector vector);

		/// <summary>
		/// Check if a coordinate is within view
		/// </summary>
		/// <param name="coordinate"><see cref="Coordinate"/> to check</param>
		/// <returns>True if in view, false if not</returns>
		bool IsInView(Coordinate coordinate);

        /// <summary>
        /// Check if a <see cref="IBoundingObject"/> is within the view
        /// </summary>
        /// <param name="boundingObject"><see cref="IBoundingObject"/> to check</param>
        /// <returns>True if in view, false if not</returns>
        bool IsInView(IBoundingObject boundingObject);

		/// <summary>
		/// Get a pick ray that goes directly forward from the given 2D coordinates into the view
		/// </summary>
		/// <param name="x">X position</param>
		/// <param name="y">Y position</param>
		/// <returns></returns>
		Ray GetPickRay(int x, int y);

        /// <summary>
        /// Unproject a vector using a specific world matrix
        /// </summary>
        /// <param name="source">Vector to unproject</param>
        /// <param name="world">World matrix</param>
        /// <returns>Unprojected vector</returns>
		Vector Unproject(Vector source, Matrix world);

        /// <summary>
        /// Unproject a vector using specified projection, view and world matrices
        /// </summary>
        /// <param name="source">Vector to unproject</param>
        /// <param name="projection">Projection matrix to use</param>
        /// <param name="view">View matrix to use</param>
        /// <param name="world">World matrix to use</param>
        /// <returns>Unprojected vector</returns>
		Vector Unproject(Vector source, Matrix projection, Matrix view, Matrix world);

		/// <summary>
		/// Update view - typically called before rendering to update all data view is holding
		/// based upon any changes.
		/// </summary>
		/// <param name="viewport"><see cref="Viewport"/> view is being rendered in</param>
		void Update(Viewport viewport);
	}
}