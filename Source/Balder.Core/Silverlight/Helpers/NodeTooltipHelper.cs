using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Balder.Core.Silverlight.Helpers
{
	public class NodeTooltipHelper
	{
		private static Node _lastEnterSource;
		private static DispatcherTimer _openTimer;
		private static DispatcherTimer _closeTimer;
		private static DateTime _lastToolTipOpenedTime;
		private static Point _mousePosition;


		public static void Register(Node node)
		{
			node.MouseEnter -= MouseEnter;
			node.MouseLeave -= MouseLeave;
			node.MouseEnter += MouseEnter;
			node.MouseLeave += MouseLeave;
			node.MouseMove += MouseMove;
		}

		private static void MouseMove(object sender, MouseEventArgs e)
		{
			_mousePosition = e.GetPosition(Application.Current.RootVisual);
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
			_openTimer.Stop();
			var source = _lastEnterSource;
			if (null != source)
			{
				var toolTip = source.ToolTip;
				if (null != toolTip)
				{
					toolTip.IsOpen = true;
					toolTip.Visibility = Visibility.Visible;
					toolTip.Margin = new Thickness(_mousePosition.X, _mousePosition.Y, 0,0);
					

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