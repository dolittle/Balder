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

using Balder.Execution;
using Balder.Rendering;

namespace Balder.Display
{
	/// <summary>
	/// Defines the display device for a <see cref="IPlatform">platform</see>
	/// </summary>
	public interface IDisplayDevice
	{
		/// <summary>
		/// Create a display from the display device
		/// </summary>
		/// <param name="runtimeContext"><see cref="IRuntimeContext"/> that the display will be created for</param>
		/// <returns>Created a <see cref="IDisplay"/></returns>
		IDisplay CreateDisplay(IRuntimeContext runtimeContext);

		/// <summary>
		/// Remove a created display
		/// </summary>
		/// <param name="display"><see cref="IDisplay"/> to remove</param>
		void RemoveDisplay(IDisplay display);
	}
}
