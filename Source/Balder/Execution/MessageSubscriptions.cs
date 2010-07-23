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

namespace Balder.Execution
{
	/// <summary>
	/// Represents all subscriptions for a message type
	/// </summary>
	/// <typeparam name="T">Type of message it holds subscriptions for</typeparam>
	public class MessageSubscriptions<T>
	{
		private readonly List<MessageAction<T>> _actions;

		/// <summary>
		/// Creates a message subscriptions container
		/// </summary>
		public MessageSubscriptions()
		{
			_actions = new List<MessageAction<T>>();
		}

		/// <summary>
		/// Add a listener to the subscriptions
		/// </summary>
		/// <param name="target">Target object action belongs to</param>
		/// <param name="listener">Action to call when subsriptions are notified</param>
		public void AddListener(object target, Action<T> listener)
		{
			var action = new MessageAction<T>(target, listener);
			_actions.Add(action);
		}

		/// <summary>
		/// Removes a listener from the subscriptions
		/// </summary>
		/// <param name="target">Target object action belongs to</param>
		/// <param name="listener">Action to remove</param>
		public void RemoveListener(object target, Action<T> listener)
		{
			var actionsToRemove = new List<MessageAction<T>>();

			foreach( var action in _actions )
			{
				if( action.Target.Target.Equals(target) &&
					action.Action.Equals(listener))
				{
					actionsToRemove.Add(action);
				}
			}

			foreach( var action in actionsToRemove )
			{
				_actions.Remove(action);
			}
		}


		/// <summary>
		/// Notify to the subscriptions with a specific message
		/// </summary>
		/// <param name="message">Message to notify with</param>
		public void Notify(T message)
		{
			foreach( var referenceToAction in _actions )
			{
				var action = referenceToAction.Action;
				if (null != action)
				{
					action(message);
				}
			}
		}
	}
}