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
using System;
using SysColor = System.Windows.Media.Color;
#else
using SysColor = System.Drawing.Color;
#endif

namespace Balder.Core
{
	/// <summary>
	/// Represents a color - generically
	/// </summary>
	/// <typeparam name="T">Type of color it represents</typeparam>
	public interface IColor<T>
	{
		/// <summary>
		/// Add color with a second color and clamp
		/// </summary>
		/// <param name="secondColor">Second color</param>
		/// <remarks>
		/// Clamping means that the color will be in the range of what is
		/// valid and if the values overflows, they are clamped to max value.
		/// </remarks>
		/// <returns>Combined colors, clamped</returns>
		T Additive(T secondColor);

		/// <summary>
		/// Subtract color with a second color and clamp
		/// </summary>
		/// <param name="secondColor">Second color</param>
		/// <remarks>
		/// Clamping means that the color will be in the range of what is
		/// valid and if the values underflows, they are clamped to min value.
		/// </remarks>
		/// <returns>Combined colors, clamped</returns>
		T Subtract(T secondColor);

		/// <summary>
		/// Get an average between this color and a second color
		/// </summary>
		/// <param name="secondColor">Second color</param>
		/// <returns>Average color</returns>
		T Average(T secondColor);

		/// <summary>
		/// Convert to <see cref="SysColor"/>
		/// </summary>
		/// <returns>Converted color</returns>
		SysColor ToSystemColor();

		/// <summary>
		/// Convert to an unsigned integer, 32 bit in length containing all the channels
		/// </summary>
		/// <remarks>
		/// Bits 31..24 - Alpha
		/// Bits 23..16 - Red
		/// Bits 15..8 - Green
		/// Bits 7..0 - Blue
		/// </remarks>
		/// <returns>Unsigned integer containing the converted color</returns>
		UInt32 ToUInt32();
	}
}
