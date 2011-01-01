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
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Balder.Input;

namespace Balder.Silverlight.Helpers
{
	public class NodeTooltipHelper
	{
		private static readonly Stack<ToolTip> _openToolTips = new Stack<ToolTip>();

		private static Node _lastEnterSource;
		private static DispatcherTimer _openTimer;
		private static DispatcherTimer _closeTimer;
		private static DateTime _lastToolTipOpenedTime;
		private static UIElement _rootVisual;

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
				// the reason we need to do this, is that we're using existing ToolTipService and Tooltip and it must 
				// initialized with the RootVisual, which it discovers during SetToolTip.
				// I consider this a bug, or at least bad practice in the code by Microsoft.
				// Looking at the code, there is a bad dependency between Tooltip and ToolTipService, were ToolTipService
				// exposes an internal property saying what layoutroot it should be relative to. Since we're displaying
				// the tooltip ourselves, this does not make sense.
				ToolTipService.SetToolTip(node, node.ToolTip);
				ToolTipService.SetToolTip(node, null);

				_rootVisual = Application.Current.RootVisual;
				if (null != _rootVisual)
				{
					_rootVisual.MouseLeave += (s, e) => CloseOpenToolTips();
				}
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
				_openTimer.Interval = new TimeSpan(0, 0, 0, 0, _lastEnterSource.ToolTipStartDelay);
				_openTimer.Start();
			}
		}

		private static void AddOpenToolTip(ToolTip toolTip)
		{
			_openToolTips.Push(toolTip);
		}

		private static void CloseOpenToolTips()
		{
			while( _openToolTips.Count > 0 )
			{
				var toolTip = _openToolTips.Pop();
				toolTip.IsOpen = false;
			}
		}


		private static void OpenAutomaticToolTip(object sender, EventArgs e)
		{
			CloseOpenToolTips();

			_openTimer.Stop();
			var source = _lastEnterSource;
			if (null != source)
			{
				var toolTip = source.ToolTip;
				if (null != toolTip)
				{
					AddOpenToolTip(toolTip);
					// Set the DataContext so we can databind - this should possibly be databound instead, to make it
					// dynamic, in case the datacontext changes during showing of the tooltip
					toolTip.DataContext = source.DataContext;
					toolTip.IsOpen = true;
					toolTip.Visibility = Visibility.Visible;

					if (null == _closeTimer)
					{
						_closeTimer = new DispatcherTimer();
						_closeTimer.Tick += CloseAutomaticToolTip;
					}

					_closeTimer.Interval = new TimeSpan(0, 0, 0, 0, source.ToolTipShowPeriod);
					_closeTimer.Start();
				}
			}
		}

		private static void CloseAutomaticToolTip(object sender, EventArgs e)
		{
			CloseOpenToolTips();
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
#endif