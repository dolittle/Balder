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
using Balder.Materials;
using Balder.Math;

namespace Balder.Lighting
{
	/// <summary>
	/// Represents a light
	/// </summary>
	public interface ILight
	{
		/// <summary>
		/// Gets or sets the diffuse light <see cref="Color"/>
		/// </summary>
		Color Diffuse { get; set; }

		/// <summary>
		/// Gets or sets the ambient light <see cref="Color"/>
		/// </summary>
		Color Ambient { get; set; }

		/// <summary>
		/// Gets or sets the specular light <see cref="Color"/>
		/// </summary>
		Color Specular { get; set; }

		/// <summary>
		/// Calculate color for the light in a viewport for a specified point and normal of the point
		/// </summary>
		/// <param name="viewport"><see cref="Viewport"/> that holds the point</param>
		/// <param name="point">Point to calculate for - in the form of a <see cref="Vector"/></param>
		/// <param name="normal">Normal for the point to calculate for - in the form of a <see cref="Vector"/></param>
		/// <returns>Calculated <see cref="Color"/></returns>
		int Calculate(Viewport viewport, Material material, Vector point, Vector normal, out int diffuseResult, out int specularResult);

		/// <summary>
		/// Gets or sets wether or not the light is enabled
		/// </summary>
		bool IsEnabled { get; set; }
	}
}