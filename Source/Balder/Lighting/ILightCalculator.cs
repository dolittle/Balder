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
using Balder.Materials;
using Balder.Math;

namespace Balder.Lighting
{
	/// <summary>
	/// Defines a service for calculating lighting in a viewport
	/// </summary>
	public interface ILightCalculator
	{
		/// <summary>
		/// Prepare the light calculator with the lights needed for calculations
		/// </summary>
		/// <param name="viewport">Viewport to prepare</param>
		/// <param name="lights">Collection of lights to prepare for</param>
		void Prepare(Viewport viewport, NodeCollection lights);

        /// <summary>
        /// Gets a boolean indicating wether or not the lights has changed. Typically used to check if one needs to recalculate lighting.
        /// </summary>
        /// <returns>True is lights has changed, false if not</returns>
        bool HasLightsChanged { get; }

        /// <summary>
        /// Prepare lights for node before rendering
        /// </summary>
        /// <param name="node">Node to prepare for</param>
        /// <param name="viewToLocal">View to local space</param>
        void PrepareForNode(INode node, Matrix viewToLocal);

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
		int Calculate(Viewport viewport, Material material, Vector point, Vector normal, out int diffuseResult, out int specularResult);
	}
}