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
using System;
using System.ComponentModel;
using Balder.Display;
using Balder.Execution;
using System.Windows;
using Balder.Extensions.Silverlight;

namespace Balder.Input.Silverlight
{
	public class MouseInfo : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private int _xPosition;
		public int XPosition
		{
			get { return _xPosition; }
			set
			{
				_xPosition = value;
				PropertyChanged.Notify(() => XPosition);
			}
		}

		private int _yPosition;
		public int YPosition
		{
			get { return _yPosition; }
			set
			{
				_yPosition = value;
				PropertyChanged.Notify(() => YPosition);
			}
		}
	}

	public class NodeMouseEventHelper : IDisposable
	{
		private readonly Game _game;
		private readonly Viewport _viewport;
		private readonly ManipulationEventHelper _manipulationEventHelper;
		private Node _previousNode;

		public static MouseInfo	MouseInfo = new MouseInfo();

		public NodeMouseEventHelper(Game game, Viewport viewport)
		{
			_game = game;
			_viewport = viewport;
			_manipulationEventHelper = new ManipulationEventHelper(viewport);
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



		private static void RaiseEvent(BubbledEvent<Node, MouseEventHandler> bubbledEvent, Node node, System.Windows.Input.MouseEventArgs args)
		{
			var position = args.GetPosition(args.OriginalSource as FrameworkElement);
			bubbledEvent.Raise(node, node, new MouseEventArgs(args, position));
		}

		private static void RaiseEvent(BubbledEvent<Node, MouseButtonEventHandler> bubbledEvent, Node node, System.Windows.Input.MouseButtonEventArgs args)
		{
			var position = args.GetPosition(args.OriginalSource as FrameworkElement);
			bubbledEvent.Raise(node, node, new MouseButtonEventArgs(args,position));
		}



		public void HandleMouseMove(int xPosition, int yPosition, System.Windows.Input.MouseEventArgs e)
		{
			MouseInfo.XPosition = xPosition;
			MouseInfo.YPosition = yPosition;

			var hitNode = _viewport.GetNodeAtPosition(xPosition, yPosition);
			if (null != hitNode)
			{
				RaiseEvent(Node.MouseMoveEvent,hitNode,e);

				if (null == _previousNode ||
					!hitNode.Equals(_previousNode))
				{
					RaiseEvent(Node.MouseEnterEvent, hitNode, e);
				}
				_previousNode = hitNode;
			}
			else if (null != _previousNode)
			{
				HandleMouseLeave(xPosition, yPosition, e);
			}
			_viewport.HandleMouseDebugInfo(xPosition, yPosition, hitNode);
		}

		public void HandleMouseEnter(int xPosition, int yPosition, System.Windows.Input.MouseEventArgs e)
		{
			var hitNode = _viewport.GetNodeAtPosition(xPosition, yPosition);
			if (null != hitNode)
			{
				if (null == _previousNode ||
					!hitNode.Equals(_previousNode))
				{
					RaiseEvent(Node.MouseEnterEvent, hitNode, e);
				}
			}
			_previousNode = hitNode;
		}

		public void HandleMouseLeave(int xPosition, int yPosition, System.Windows.Input.MouseEventArgs e)
		{
			if (null != _previousNode)
			{
				RaiseEvent(Node.MouseLeaveEvent,_previousNode,e);
				_previousNode = null;
			}
		}


		private void MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var position = e.GetPosition(e.OriginalSource as FrameworkElement);
			HandleMouseMove((int)position.X, (int)position.Y, e);
			_manipulationEventHelper.HandleManipulation(position);
		}

		private void MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var position = e.GetPosition(e.OriginalSource as FrameworkElement);
			HandleMouseEnter((int)position.X, (int)position.Y, e);
		}

		private void MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
		{
			var position = e.GetPosition(e.OriginalSource as FrameworkElement);
			HandleMouseLeave((int)position.X, (int)position.Y, e);
			_manipulationEventHelper.StopManipulation();
		}

		private void MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var position = e.GetPosition(_game);
			var hitNode = _viewport.GetNodeAtPosition((int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				RaiseEvent(Node.MouseLeftButtonDownEvent,hitNode,e);
				_manipulationEventHelper.StartManipulation(hitNode, position);
			}
		}


		private void MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
		{
			var position = e.GetPosition(_game);
			var hitNode = _viewport.GetNodeAtPosition((int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				RaiseEvent(Node.MouseLeftButtonUpEvent, hitNode, e);
			}
			_manipulationEventHelper.StopManipulation();
		}
	}
}
#endif