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

namespace Balder.Input
{
	/// <summary>
	/// Defines the behavior of getting a <see cref="RenderableNode"/> at a particular position
	/// </summary>
	public interface ICanGetNodeAtPosition
	{
		/// <summary>
		/// Get the nearest <see cref="RenderableNode"/> at a specific position and direction based upon a Ray
		/// </summary>
		/// <param name="viewport">The <see cref="Viewport"/> the node is within </param>
		/// <param name="pickRay"><see cref="Ray"/> to use for defining the position and direction to start the search</param>
		/// <param name="closestNode">Returns the <see cref="RenderableNode">closest node</see> found, null if it didn't hit any node</param>
		/// <param name="closestDistance">Returns the closest distance of the node it finds, if any. Will return null if it didn't find any node</param>
		void GetNodeAtPosition(Viewport viewport, Ray pickRay, ref RenderableNode closestNode, ref float? closestDistance);
	}
}