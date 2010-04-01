using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Balder.Core.Silverlight.Helpers;

namespace Balder.Core
{
	[ContentProperty("Children")]
	public partial class Node : ItemsControl
	{
		public new event MouseEventHandler MouseMove;
		public new event MouseEventHandler MouseEnter;
		public new event MouseEventHandler MouseLeave;
		public new event MouseButtonEventHandler MouseLeftButtonDown;
		public new event MouseButtonEventHandler MouseLeftButtonUp;


		partial void Construct()
		{
			Loaded += NodeLoaded;
			Children.CollectionChanged += ChildrenChanged;
			Width = 0;
			Height = 0;
			Visibility = Visibility.Collapsed;
		}

		private void ChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case NotifyCollectionChangedAction.Add:
					{
						foreach (var item in e.NewItems)
						{
							Items.Add(item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Remove:
					{
						foreach (var item in e.OldItems)
						{
							Items.Remove(item);
						}
					}
					break;
				case NotifyCollectionChangedAction.Reset:
					{
						Items.Clear();
					}
					break;

			}
		}

		private void NodeLoaded(object sender, RoutedEventArgs e)
		{
			OnInitialize();
		}


		public static readonly DependencyProperty<Node, ICommand> CommandProperty =
			DependencyProperty<Node, ICommand>.Register(o => o.Command);
		public ICommand Command
		{
			get { return CommandProperty.GetValue(this); }
			set { CommandProperty.SetValue(this, value); }
		}

		public static readonly DependencyProperty<Node, object> CommandParameterProperty =
			DependencyProperty<Node, object>.Register(o => o.CommandParameter);
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

		internal virtual void RaiseMouseLeftButtonUp(MouseButtonEventArgs e)
		{
			OnCommand();
			if (null != MouseLeftButtonUp)
			{
				MouseLeftButtonUp(this, e);
			}
		}

		internal virtual void RaiseMouseMove(MouseEventArgs e)
		{
			if (null != MouseMove)
			{
				MouseMove(this, e);
			}
		}

		internal virtual void RaiseMouseLeftButtonDown(MouseButtonEventArgs e)
		{
			if (null != MouseLeftButtonDown)
			{
				MouseLeftButtonDown(this, e);
			}
		}

		internal virtual void RaiseMouseEnter(MouseEventArgs e)
		{
			if (null != MouseEnter)
			{
				MouseEnter(this, e);
			}
		}

		internal virtual void RaiseMouseLeave(MouseEventArgs e)
		{
			if (null != MouseLeave)
			{
				MouseLeave(this, e);
			}
		}
	}
}
