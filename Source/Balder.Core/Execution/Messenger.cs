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

using System;

namespace Balder.Core.Execution
{
	public class MessengerSubscription<T>
	{
		private WeakReference _action;

		public MessengerSubscription(Action<T> actionToCall)
		{
			_action = new WeakReference(actionToCall);
		}

		public void Notify(T message)
		{
			var action = _action.Target as Action<T>;
			if( null != action )
			{
				action(message);
			}
		}
	}

	public class MessengerContext
	{
		public void ListenTo<T>(T message, Action<T> actionToCall)
		{
			
		}

		public void Send<T>(T message)
		{
			
		}
	}

	public class Messenger
	{
		public Messenger()
		{
			DefaultContext = new MessengerContext();
		}

		public MessengerContext DefaultContext { get; private set; }

		public MessengerContext Context(object context)
		{
			
		}
	}
}
