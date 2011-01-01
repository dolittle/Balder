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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Balder.MVVM
{
	public class CommandManager
	{
		public static readonly CommandManager Instance = new CommandManager();

		
		private Dictionary<UIElement, CommandSubscription> _subscriptions;

		private CommandManager()
		{
			_subscriptions = new Dictionary<UIElement, CommandSubscription>();
		}

		private CommandSubscription GetOrCreateSubscription(UIElement element)
		{
			CommandSubscription subscription = null;
			if( !_subscriptions.ContainsKey(element) )
			{
				subscription = new CommandSubscription(element);
				_subscriptions[element] = subscription;
			} else
			{
				subscription = _subscriptions[element];
			}
			return subscription;
		}


		public void SetCommandForUIElement(UIElement element, ICommand command)
		{
			var subscription = GetOrCreateSubscription(element);
			subscription.Command = command;
			HookupExecuteEvent(subscription);
			HookUpCanExecuteEvent(subscription);
			SetCanExecute(subscription);
		}

		public void SetCommandParameterForUIElement(UIElement element, object parameter)
		{
			var subscription = GetOrCreateSubscription(element);
			subscription.CommandParameter = parameter;
			SetCanExecute(subscription);
		}

		private void SetCanExecute(CommandSubscription subscription)
		{
			subscription.UIElement.Dispatcher.BeginInvoke(
				() =>
					{
						if (subscription.UIElement is Control)
						{
							((Control) subscription.UIElement).IsEnabled = subscription.Command.CanExecute(subscription.CommandParameter);
						}
						else if (subscription.UIElement is ICanBeEnabled)
						{
							((ICanBeEnabled) subscription.UIElement).IsEnabled =
								subscription.Command.CanExecute(subscription.CommandParameter);
						}
					});
		}

		private void HookUpCanExecuteEvent(CommandSubscription subscription)
		{
			subscription.Command.CanExecuteChanged +=
				(s, e) => SetCanExecute(subscription);
		}



		private void HookupExecuteEvent(CommandSubscription subscription)
		{
			if( subscription.UIElement is ButtonBase )
			{
				HookupExecuteEventForButtonBase(subscription);	
			} else if( subscription.UIElement is TextBox )
			{
				HookupExecuteEventForTextBox(subscription);
			} else if( subscription.UIElement is ICanExecuteCommand )
			{
				HookupExecuteEventForRoutedCommand(subscription);
			} else
			{
				HookupExceuteEventForLeftMouseButtonUp(subscription);
			}
		}


		private void HookupExecuteEventForButtonBase(CommandSubscription subscription)
		{
			var button = subscription.UIElement as ButtonBase;
			button.Click += (s,e) => subscription.Execute();
		}

		private void HookupExecuteEventForTextBox(CommandSubscription subscription)
		{
			var textBox = subscription.UIElement as TextBox;
			textBox.TextChanged += (s, e) => subscription.Execute();
		}

		private void HookupExceuteEventForLeftMouseButtonUp(CommandSubscription subscription)
		{
			subscription.UIElement.MouseLeftButtonDown += (s, e) => subscription.Execute();
			
		}

		private void HookupExecuteEventForRoutedCommand(CommandSubscription subscription)
		{
			var routedCommand = subscription.UIElement as ICanExecuteCommand;
			routedCommand.Command += (s, e) => subscription.Execute();
		}
	}
}
#endif