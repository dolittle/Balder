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
using System;

namespace Balder.Execution
{
	/// <summary>
	/// Base class for arguments for Bubbled events
	/// </summary>
	public class BubbledEventArgs : EventArgs
	{
		public static new readonly BubbledEventArgs	Empty = new BubbledEventArgs();

		/// <summary>
		/// Gets the original source of the event in the form of <see cref="INode"/>
		/// </summary>
		public INode OriginalSource { get; internal set; }

		/// <summary>
		/// Gets or sets wether or not the event was handled
		/// </summary>
		/// <remarks>
		/// Setting this to true in an event handler will make the bubbling of the event
		/// stop from that point
		/// </remarks>
		public bool Handled { get; set; }
	}
}