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

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Balder.Core.Execution;

namespace Balder.Core
{
	public partial class Node : ItemsControl
	{
		internal static readonly BubbledEvent<Node, MouseEventHandler> MouseMoveEvent =
			BubbledEvent<Node, MouseEventHandler>.Register(n => n.MouseMove);
		internal static readonly BubbledEvent<Node, MouseEventHandler> MouseEnterEvent =
			BubbledEvent<Node, MouseEventHandler>.Register(n => n.MouseEnter);
		internal static readonly BubbledEvent<Node, MouseEventHandler> MouseLeaveEvent =
			BubbledEvent<Node, MouseEventHandler>.Register(n => n.MouseLeave);
		internal static new readonly BubbledEvent<Node, MouseButtonEventHandler> MouseLeftButtonDownEvent =
			BubbledEvent<Node, MouseButtonEventHandler>.Register(n => n.MouseLeftButtonDown);
		internal static new readonly BubbledEvent<Node, MouseButtonEventHandler> MouseLeftButtonUpEvent =
			BubbledEvent<Node, MouseButtonEventHandler>.Register(n => n.MouseLeftButtonUp);

		public new event MouseEventHandler MouseMove;
		public new event MouseEventHandler MouseEnter;
		public new event MouseEventHandler MouseLeave;
		public new event MouseButtonEventHandler MouseLeftButtonDown;
		public new event MouseButtonEventHandler MouseLeftButtonUp;


		partial void Construct()
		{
			Loaded += NodeLoaded;
			Width = 0;
			Height = 0;
			MouseLeftButtonUp += (s, e) => OnCommand();
		}


		private void NodeLoaded(object sender, RoutedEventArgs e)
		{
			OnInitialize();
		}


		public static readonly Property<Node, ICommand> CommandProperty =
			Property<Node, ICommand>.Register(o => o.Command);
		public ICommand Command
		{
			get { return CommandProperty.GetValue(this); }
			set { CommandProperty.SetValue(this, value); }
		}

		public static readonly Property<Node, object> CommandParameterProperty =
			Property<Node, object>.Register(o => o.CommandParameter);
		public object CommandParameter
		{
			get { return CommandParameterProperty.GetValue(this); }
			set { CommandParameterProperty.SetValue(this, value); }
		}


		protected void OnCommand()
		{
			if (null != Command)
			{
				if (Command.CanExecute(CommandParameter))
				{
					Command.Execute(CommandParameter);
				}
			}
		}
	}
}
