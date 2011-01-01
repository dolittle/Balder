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

using Balder.Imaging;

namespace Balder
{
	/// <summary>
	/// Specifies the pixelformat used by typical <see cref="Image"/>
	/// </summary>
	public enum PixelFormat
	{
		/// <summary>
		/// 8 bit gray without alpha
		/// </summary>
		Grayscale = 1,

		/// <summary>
		/// 8 bit gray with alpha
		/// </summary>
		GrayscaleAlpha,

		/// <summary>
		/// 8 bit palette based (256 colors)
		/// </summary>
		Palette,

		/// <summary>
		/// 24 bit RGB, 8 bits per channel
		/// </summary>
		RGB,

		/// <summary>
		/// 32 bit RGB with Alpha, 8 bits per channel
		/// </summary>
		RGBAlpha
	}
}