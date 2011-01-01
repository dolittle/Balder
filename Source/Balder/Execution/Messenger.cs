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

using System.Collections.Generic;

namespace Balder.Execution
{
	/// <summary>
	/// Handles decoupled messaging
	/// </summary>
	public class Messenger : IMessenger
	{
		private static readonly Dictionary<object, MessengerContext> Contexts;

		static Messenger()
		{
			DefaultContext = new MessengerContext();
			Contexts = new Dictionary<object, MessengerContext>();
		}


		/// <summary>
		/// Gets the default messenger context
		/// </summary>
		public static IMessengerContext DefaultContext { get; private set; }


		/// <summary>
		/// Get a specific context for an object instance
		/// </summary>
		/// <param name="obj">Object instance to get context for</param>
		/// <returns>Messenger context</returns>
		public static MessengerContext Context(object obj)
		{
			MessengerContext context = null;
			if( Contexts.ContainsKey(obj))
			{
				context = Contexts[obj];
			} else
			{
				context = new MessengerContext();
				Contexts[obj] = context;
			}

			return context;
		}
	}
}
