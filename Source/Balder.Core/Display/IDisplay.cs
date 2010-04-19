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

using Balder.Core.Rendering;

namespace Balder.Core.Display
{
	/// <summary>
	/// Represents a display on the device
	/// </summary>
	public interface IDisplay
	{
		/// <summary>
		/// Get or set the background color used on the display
		/// </summary>
		Color BackgroundColor { get; set; }

		/// <summary>
		/// Initialize the display
		/// </summary>
		/// <param name="width">Width of the display</param>
		/// <param name="height">Height of the display</param>
		void Initialize(int width, int height);

		/// <summary>
		/// Uninitialize a display
		/// </summary>
		void Uninitialize();

		/// <summary>
		/// Initialize display container
		/// </summary>
		/// <param name="container">Container</param>
		void InitializeContainer(object container);

		/// <summary>
		/// Get a node at a specific position within the display
		/// </summary>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		/// <returns>The node at the position, null if no node is at the position</returns>
		INode GetNodeAtPosition(int xPosition, int yPosition);

		/// <summary>
		/// Gets the current frames pixeldata
		/// </summary>
		/// <returns>An array of pixels, 32 bit with alpha</returns>
		int[] GetCurrentFrame();
	}
}