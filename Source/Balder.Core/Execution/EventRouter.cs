#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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
	public class EventRouter
	{
		private static readonly Dictionary<string, EventRouter> Routers = new Dictionary<string, EventRouter>();
		private readonly List<WeakReference> _listeners;

		private EventRouter(string name)
		{
			Name = name;
			_listeners = new List<WeakReference>();
		}

		public string Name { get; private set; }

		public void AddListener(Delegate listener)
		{
			var weakReference = new WeakReference(listener);
			_listeners.Add(weakReference);
		}


		public static EventRouter	Get(string name)
		{
			EventRouter router = null;
			if( Routers.ContainsKey(name))
			{
				router = Routers[name];
			} else
			{
				router = new EventRouter(name);
				Routers[name] = router;
			}
			
			return router;
		}

		public void Raise(Delegate eventHandler, params object[] arguments)
		{
			if (null != eventHandler)
			{
				var delegates = eventHandler.GetInvocationList();
				foreach (var del in delegates)
				{
					del.DynamicInvoke(arguments);
				}
			}

			foreach( var listener in _listeners )
			{
				if( listener.IsAlive )
				{
					var del = (Delegate) listener.Target;
					del.DynamicInvoke(arguments);
				}
			}
		}
	}
}
