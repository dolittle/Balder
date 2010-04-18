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
using System.Collections.Generic;

namespace Balder.Core.Execution
{
	/// <summary>
	/// Represents a context for the messenger in which one can publish and subscribe
	/// to messages from
	/// </summary>
	public class MessengerContext
	{
		private readonly Dictionary<Type, object> _messageSubscriptions;

		/// <summary>
		/// Creates a messenger context
		/// </summary>
		public MessengerContext()
		{
			_messageSubscriptions = new Dictionary<Type, object>();
		}

		private MessageSubscriptions<T> GetSubscription<T>()
		{
			var type = typeof (T);
			MessageSubscriptions<T> subscription = null;
			if( _messageSubscriptions.ContainsKey(type))
			{
				subscription = _messageSubscriptions[type] as MessageSubscriptions<T>;
			} else
			{
				subscription = new MessageSubscriptions<T>();
				_messageSubscriptions[type] = subscription;
			}
			return subscription;
		}


		/// <summary>
		/// Adds a subscription for a specified message
		/// </summary>
		/// <typeparam name="T">Type of message to subscribe to</typeparam>
		/// <param name="actionToCall">Action to call when a message is received</param>
		public void ListenTo<T>(Action<T> actionToCall)
		{
			var subscription = GetSubscription<T>();
			subscription.AddListener(actionToCall);
		}


		/// <summary>
		/// Sends a specific message to any listeners
		/// </summary>
		/// <typeparam name="T">Type of message to send</typeparam>
		/// <param name="message">Actual message to send</param>
		public void Send<T>(T message)
		{
			var subscription = GetSubscription<T>();
			subscription.Notify(message);
		}
	}
}