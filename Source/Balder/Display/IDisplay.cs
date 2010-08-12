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
#if(SILVERLIGHT)

#endif
using Balder.Objects;

namespace Balder.Display
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
		/// Get or set wether or not clear should occur
		/// </summary>
		bool ClearEnabled { get; set; }

		/// <summary>
		/// Get or set wether or not the display is paused
		/// By paused, this means that it won't clear or swap next buffer
		/// </summary>
		bool Paused { get; set; }

		/// <summary>
		/// Get or set wether or not the entire display + rendering events
		/// should be halted
		/// </summary>
		bool Halted { get; set; }

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

#if(SILVERLIGHT)
		/// <summary>
		/// Initialize display container
		/// </summary>
		/// <param name="container">Container</param>
		void InitializeContainer(object container);
#endif

		void InitializeSkybox(Skybox skybox);

		/// <summary>
		/// Gets the current frames pixeldata
		/// </summary>
		/// <returns>An array of pixels, 32 bit with alpha</returns>
		int[] GetCurrentFrame();
	}
}