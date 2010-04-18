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
	public class MessageSubscriptions<T>
	{
		private readonly List<WeakReference> _actions;

		public MessageSubscriptions()
		{
			_actions = new List<WeakReference>();
		}

		public void AddListener(Action<T> listener)
		{
			var reference = new WeakReference(listener);
			_actions.Add(reference);
		}

		public void Notify(T message)
		{
			foreach( var referenceToAction in _actions )
			{
				var action = referenceToAction.Target as Action<T>;
				if (null != action)
				{
					action(message);
				}
			}
		}
	}
}