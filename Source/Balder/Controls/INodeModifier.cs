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
#if(SILVERLIGHT)
namespace Balder.Controls
{
	/// <summary>
	/// Defines a modifier of nodes during binding for some control types
	/// </summary>
	public interface INodeModifier
	{
		/// <summary>
		/// Apply modification based upon a particular index and datacontext
		/// </summary>
		/// <param name="node"><see cref="INode"/> to apply for</param>
		/// <param name="nodeIndex">Index within the binding</param>
		/// <param name="dataContext">DataContext for the node</param>
		void Apply(INode node, int nodeIndex, object dataContext);
	}
}
#endif