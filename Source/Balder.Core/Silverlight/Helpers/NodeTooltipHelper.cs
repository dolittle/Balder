using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Balder.Core.Silverlight.Helpers
{
	public class NodeTooltipHelper
	{
		private static ToolTip _currentToolTip;
		private static Node _lastEnterSource;
		private static DispatcherTimer _openTimer;
		private static DispatcherTimer _closeTimer;
		private static DateTime _lastToolTipOpenedTime;
		private static object _rootVisual;


		public static void Register(Node node)
		{
			node.MouseEnter -= MouseEnter;
			node.MouseLeave -= MouseLeave;
			node.MouseMove -= MouseMove;

			node.MouseEnter += MouseEnter;
			node.MouseLeave += MouseLeave;
			node.MouseMove += MouseMove;

			SetRootVisual(node);
		}

		private static void SetRootVisual(Node node)
		{
			if (null == _rootVisual)
			{
				// Todo: If at all there is a better way than this, do fix it.. :)
				ToolTipService.SetToolTip(node, node.ToolTip);
				ToolTipService.SetToolTip(node, null);

				_rootVisual = Application.Current.RootVisual;
			}
		}

		private static void MouseMove(object sender, MouseEventArgs e)
		{
		}

		private static void MouseEnter(object sender, MouseEventArgs e)
		{
			_lastEnterSource = sender as Node;
			var span = (TimeSpan)(DateTime.Now - _lastToolTipOpenedTime);
			if (TimeSpan.Compare(span, new TimeSpan(0, 0, 0, 0, 100)) <= 0)
			{
				OpenAutomaticToolTip(null, EventArgs.Empty);
			}
			else
			{
				if (null == _openTimer)
				{
					_openTimer = new DispatcherTimer();
					_openTimer.Tick += OpenAutomaticToolTip;
				}
				_openTimer.Interval = new TimeSpan(0, 0, 0, 0, 400);
				_openTimer.Start();
			}
		}

		private static void OpenAutomaticToolTip(object sender, EventArgs e)
		{
			if( null != _currentToolTip )
			{
				_currentToolTip.IsOpen = false;
				_currentToolTip = null;
			}

			_openTimer.Stop();
			var source = _lastEnterSource;
			if (null != source)
			{
				var toolTip = source.ToolTip;
				if (null != toolTip)
				{
					_currentToolTip = toolTip;
					toolTip.IsOpen = true;
					toolTip.Visibility = Visibility.Visible;

					if (null == _closeTimer)
					{
						_closeTimer = new DispatcherTimer();
						_closeTimer.Tick += CloseAutomaticToolTip;
					}

					_closeTimer.Interval = new TimeSpan(0, 0, 0, 0, 0x1388);
					_closeTimer.Start();
				}
			}
		}

		private static void CloseAutomaticToolTip(object sender, EventArgs e)
		{
			_currentToolTip = null;
			if( null != _closeTimer )
			{
				_closeTimer.Stop();	
			}
			var source = _lastEnterSource;
			if (null != source)
			{
				var toolTip = source.ToolTip;
				if (null != toolTip)
				{
					toolTip.IsOpen = false;
					_lastToolTipOpenedTime = DateTime.Now;
				}
			}

		}

		private static void MouseLeave(object sender, MouseEventArgs e)
		{
			CloseAutomaticToolTip(null,EventArgs.Empty);
		}


	}
}