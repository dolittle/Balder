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
#if(SILVERLIGHT)
using System.Windows;
using System.Windows.Input;

namespace Balder.MVVM
{
	public static class CommandService
	{
		#region Command Property
		public static readonly DependencyProperty CommandProperty =
			DependencyProperty.RegisterAttached(
				"Command",
				typeof(ICommand),
				typeof(CommandService),
				new PropertyMetadata(new PropertyChangedCallback(OnCommandChanged)));

		public static void OnCommandChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			SetCommand(obj, (ICommand)e.NewValue);
		}

		public static void SetCommand(DependencyObject obj, ICommand value)
		{
			obj.SetValue(CommandProperty, value);
			CommandManager.Instance.SetCommandForUIElement(obj as UIElement,value);
		}

		public static ICommand GetCommand(DependencyObject obj)
		{
			return (ICommand)obj.GetValue(CommandProperty);
		}
		#endregion

		#region CommandParameter Property
		public static readonly DependencyProperty CommandParameterProperty =
			DependencyProperty.RegisterAttached(
				"CommandParameter",
				typeof(object),
				typeof(CommandService),
				new PropertyMetadata(new PropertyChangedCallback(OnCommandParameterChanged)));

		public static void OnCommandParameterChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
		{
			SetCommandParameter(obj, (object)e.NewValue);
		}

		public static void SetCommandParameter(DependencyObject obj, object value)
		{
			obj.SetValue(CommandParameterProperty, value);
			CommandManager.Instance.SetCommandParameterForUIElement(obj as UIElement, value);
		}

		public static object GetCommandParameter(DependencyObject obj)
		{
			return (object)obj.GetValue(CommandParameterProperty);
		}
		#endregion
	}
}
#endif