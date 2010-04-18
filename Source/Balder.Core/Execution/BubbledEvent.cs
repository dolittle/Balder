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
	public delegate void BubbledEventHandler(INode sender, BubbledEventArgs eventArgs);
	public delegate void BubbledEventHandler<T>(INode sender, T eventArgs) where T : BubbledEventArgs;

	/// <summary>
	/// Represents a bubbled event, an event that can travel upwards through the
	/// node hierarchy
	/// </summary>
	/// <typeparam name="T">Type of the owner</typeparam>
	/// <typeparam name="TEt">
	/// EventHandler type, see <see cref="BubbledEventHandler"/> or <see cref="BubbledEventHandler{T}"/> 
	/// for event handler types that are generic and can be used.
	/// </typeparam>
	/// <remarks>
	/// In order for a bubbled event to be able to alert the entire hierarchy, all
	/// elements in the hierarchy must be the same type.
	/// 
	/// If using the BubbledEventArgs as base for your event argument, events can
	/// be handled through the hierarchy with the Handled property. That will stop
	/// the bubbling.
	/// </remarks>
	public class BubbledEvent<T,TEt>
		where T:INode
	{
		private readonly Func<T, TEt> _eventHandlerFunc;

		private BubbledEvent(Func<T, TEt> eventHandlerFunc)
		{
			_eventHandlerFunc = eventHandlerFunc;
		}


		private void InternalRaise(T instance, T originalSource, params object[] arguments)
		{
			if (null != _eventHandlerFunc)
			{
				arguments = GetArguments(instance, originalSource, arguments);
				var eventHandler = _eventHandlerFunc((T)instance) as Delegate;
				if (null != eventHandler)
				{
					foreach (var del in eventHandler.GetInvocationList())
					{
						del.DynamicInvoke(arguments);
					}
				}
				if (null != instance.Parent && !IsEventHandled(arguments))
				{
					InternalRaise((T)instance.Parent, originalSource, arguments);
				}
			}
		}

		private bool IsEventHandled(params object[] arguments)
		{
			foreach (var arg in arguments)
			{
				if( arg is BubbledEventArgs )
				{
					return ((BubbledEventArgs) arg).Handled;
				}
			}
			return false;
		}

		private object[] GetArguments(T instance, T originalSource, params object[] arguments)
		{
			var modifiedArguments = new List<object>();

			foreach( var arg in arguments )
			{
				if( arg is INode )
				{
					modifiedArguments.Add(instance);
				} else
				{
					modifiedArguments.Add(arg);
				}
				if( arg is BubbledEventArgs )
				{
					((BubbledEventArgs) arg).OriginalSource = originalSource;
					
				}
			}

			return modifiedArguments.ToArray();
		}


		/// <summary>
		/// Register a bubbled event on a type
		/// </summary>
		/// <param name="eventHandlerFunc">Func representing the eventhandler to invoke during raise</param>
		/// <returns>BubbledEvent ready to be used</returns>
		public static BubbledEvent<T, TEt> Register(Func<T, TEt> eventHandlerFunc)
		{
			var routedEvent = new BubbledEvent<T, TEt>(eventHandlerFunc);
			return routedEvent;
		}

		/// <summary>
		/// Raise a bubbled event through the hierarchy
		/// </summary>
		/// <param name="instance">Instance to raise event for</param>
		/// <param name="arguments">Arguments for the event, important that these match to the signature of the delegate representing the event</param>
		public void Raise(T instance, params object[] arguments)
		{
			InternalRaise(instance,instance,arguments);
		}
	}
}
