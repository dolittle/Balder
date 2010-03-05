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
using Balder.Core.Display;
using Balder.Core.Math;

namespace Balder.Core.Lighting
{
	/// <summary>
	/// Represents a service for calculating lighting in a viewport
	/// </summary>
	public interface ILightCalculator
	{
		/// <summary>
		/// Calculate color based on lighting in the viewport
		/// </summary>
		/// <param name="viewport"><see cref="Viewport"/> to calculate from</param>
		/// <param name="point">Point to calculate for - in the form of a <see cref="Vector"/></param>
		/// <param name="normal">Normal of the point to calculate for - in the form of a <see cref="Vector"/></param>
		/// <returns>Calculated <see cref="Color"/></returns>
		/// <remarks>
		/// It is assumed that it will take into account all lighting information
		/// inside the viewport - meaning that it will calculate using all lights
		/// available.
		/// </remarks>
		ColorAsFloats Calculate(Viewport viewport, Vector point, Vector normal);
	}
}