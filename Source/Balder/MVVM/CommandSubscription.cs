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
using System;
using System.Windows;
using System.Windows.Input;

namespace Balder.MVVM
{
	public class CommandSubscription
	{
		internal CommandSubscription(UIElement element)
		{
			UIElement = element;
		}

		public UIElement UIElement { get; private set; }
		public ICommand Command { get; internal set; }
		public object CommandParameter { get; internal set; }

		public bool IsValid
		{
			get
			{
				return (null != Command);
			}
		}


		public void Execute()
		{
			if( !IsValid )
			{
				throw new ArgumentException("Command is not valid");
			}

			if( Command.CanExecute(CommandParameter))
			{
				Command.Execute(CommandParameter);
			}
		}
	}
}
#endif