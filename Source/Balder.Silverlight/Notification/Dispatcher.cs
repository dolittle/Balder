﻿#region License

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

namespace Balder.Silverlight.Notification
{
	public class Dispatcher : IDispatcher
	{
		private readonly System.Windows.Threading.Dispatcher _systemDispatcher;

		public Dispatcher(System.Windows.Threading.Dispatcher systemDispatcher)
		{
			_systemDispatcher = systemDispatcher;
		}

		public bool CheckAccess()
		{
			return _systemDispatcher.CheckAccess();
		}

		public void BeginInvoke(Delegate del, params object[] arguments)
		{
			_systemDispatcher.BeginInvoke(del, arguments);
		}

		public void BeginInvoke(Action action)
		{
			_systemDispatcher.BeginInvoke(action);
		}
	}
}
