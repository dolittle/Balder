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
using System.Windows.Input;
using Balder.Core.Display;
using Balder.Core.Execution;
using System.Windows;

namespace Balder.Core.Silverlight.Input
{
	public class NodeMouseEventHelper : IDisposable
	{
		private readonly Game _game;
		private readonly Viewport _viewport;
		private Node _previousNode;

		public NodeMouseEventHelper(Game game, Viewport viewport)
		{
			_game = game;
			_viewport = viewport;
			_previousNode = null;
			AddEvents(_game);
		}

		private void AddEvents(FrameworkElement element)
		{
			element.MouseLeftButtonDown += MouseLeftButtonDown;
			element.MouseLeftButtonUp += MouseLeftButtonUp;
			element.MouseMove += MouseMove;
			element.MouseEnter += MouseEnter;
			element.MouseLeave += MouseLeave;
		}

		private void RemoveEvents(FrameworkElement element)
		{
			element.MouseLeftButtonDown -= MouseLeftButtonDown;
			element.MouseLeftButtonUp -= MouseLeftButtonUp;
			element.MouseMove -= MouseMove;
			element.MouseEnter -= MouseEnter;
			element.MouseLeave -= MouseLeave;
		}

		public void Dispose()
		{
			RemoveEvents(_game);
		}

		public void HandleMouseMove(int xPosition, int yPosition, MouseEventArgs e)
		{
			var hitNode = _viewport.GetNodeAtScreenCoordinate(xPosition, yPosition);
			if (null != hitNode)
			{
				Node.MouseMoveEvent.Raise(hitNode, hitNode, e);

				if (null == _previousNode ||
					!hitNode.Equals(_previousNode))
				{
					Node.MouseEnterEvent.Raise(hitNode, hitNode, e);
				}
				_previousNode = hitNode;
			}
			else if (null != _previousNode)
			{
				HandleMouseLeave(xPosition, yPosition, e);
			}
			_viewport.HandleMouseDebugInfo(xPosition, yPosition, hitNode);
		}

		public void HandleMouseEnter(int xPosition, int yPosition, MouseEventArgs e)
		{
			var hitNode = _viewport.GetNodeAtScreenCoordinate(xPosition, yPosition);
			if (null != hitNode)
			{
				if (null == _previousNode ||
					!hitNode.Equals(_previousNode))
				{
					Node.MouseEnterEvent.Raise(hitNode, hitNode, e);
				}
			}
			_previousNode = hitNode;
		}

		public void HandleMouseLeave(int xPosition, int yPosition, MouseEventArgs e)
		{
			if (null != _previousNode)
			{
				Node.MouseLeaveEvent.Raise(_previousNode, _previousNode, e);
				_previousNode = null;
			}
		}


		private void MouseMove(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(e.OriginalSource as FrameworkElement);
			HandleMouseMove((int)position.X, (int)position.Y, e);
		}

		private void MouseEnter(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(e.OriginalSource as FrameworkElement);
			HandleMouseEnter((int)position.X, (int)position.Y, e);
		}

		private void MouseLeave(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(e.OriginalSource as FrameworkElement);
			HandleMouseLeave((int)position.X, (int)position.Y, e);
		}

		private void MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			var position = e.GetPosition(_game);
			var hitNode = _viewport.GetNodeAtScreenCoordinate((int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				Node.MouseLeftButtonDownEvent.Raise(hitNode, hitNode, e);
			}
		}

		private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var position = e.GetPosition(_game);
			var hitNode = _viewport.GetNodeAtScreenCoordinate((int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				Node.MouseLeftButtonUpEvent.Raise(hitNode, hitNode, e);
			}
		}
	}
}
