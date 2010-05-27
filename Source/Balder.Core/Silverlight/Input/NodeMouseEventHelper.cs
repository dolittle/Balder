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
using System;
using Balder.Core.Display;
using Balder.Core.Execution;
using System.Windows;
using Balder.Core.Input;
using MouseButtonEventArgs = System.Windows.Input.MouseButtonEventArgs;
using MouseEventHandler = Balder.Core.Input.MouseEventHandler;
using MouseButtonEventHandler = Balder.Core.Input.MouseButtonEventHandler;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace Balder.Core.Silverlight.Input
{
	public class NodeMouseEventHelper : IDisposable
	{
		private readonly Game _game;
		private readonly Viewport _viewport;
		private Node _previousNode;
		private Point _previousPosition;
		private ManipulationDirection _manipulationDirection;
		private bool _manipulating;
		private Node _nodeBeingManipulated;

		public NodeMouseEventHelper(Game game, Viewport viewport)
		{
			_game = game;
			_viewport = viewport;
			_previousNode = null;
			ResetManipulation();
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

		

		private static void RaiseEvent(BubbledEvent<Node, MouseEventHandler> bubbledEvent, Node node, MouseEventArgs args)
		{
			var position = args.GetPosition(args.OriginalSource as FrameworkElement);
			bubbledEvent.Raise(node, node, new Core.Input.MouseEventArgs(args,position));
		}

		private static void RaiseEvent(BubbledEvent<Node, MouseButtonEventHandler> bubbledEvent, Node node, MouseButtonEventArgs args)
		{
			var position = args.GetPosition(args.OriginalSource as FrameworkElement);
			bubbledEvent.Raise(node, node, new Core.Input.MouseButtonEventArgs(args,position));
		}

		private void RaiseManipulationEvent(BubbledEvent<Node, ManipulationDeltaEventHandler> bubbledEvent, Node node, Point position)
		{
			var deltaX = position.X - _previousPosition.X;
			var deltaY = position.Y - _previousPosition.Y;
			HandleManipulationDirection(deltaX,deltaY);

			var material = _viewport.Display.GetMaterialAtPosition((int) position.X, (int) position.Y);
			bubbledEvent.Raise(node, node, new ManipulationDeltaEventArgs(material,(int)deltaX,(int)deltaY,_manipulationDirection));
		}

		private void ResetManipulation()
		{
			_manipulating = false;
			_manipulationDirection = ManipulationDirection.None;
		}

		private void StartManipulation(Node node, Point position)
		{
			_manipulating = true;
			_nodeBeingManipulated = node;

			Node.ManipulationStartedEvent.Raise(node, node, BubbledEventArgs.Empty);
		}

		private void HandleManipulation(Point position)
		{
			if( _manipulating )
			{
				RaiseManipulationEvent(Node.ManipulationDeltaEvent,_nodeBeingManipulated,position);
			}
			_previousPosition = position;
		}

		private void StopManipulation()
		{
			ResetManipulation();
			Node.ManipulationStoppedEvent.Raise(_nodeBeingManipulated, _nodeBeingManipulated, BubbledEventArgs.Empty);
		}


		private void HandleManipulationDirection(double deltaX, double deltaY)
		{
			if (_manipulationDirection == ManipulationDirection.None)
			{
				var absoluteDeltaX = System.Math.Abs(deltaX);
				var absoluteDeltaY = System.Math.Abs(deltaY);

				if (absoluteDeltaX > absoluteDeltaY)
				{
					if (deltaX <= 0)
					{
						_manipulationDirection = ManipulationDirection.Left;
					}
					else
					{
						_manipulationDirection = ManipulationDirection.Right;
					}
				}
				else
				{
					if (deltaY <= 0)
					{
						_manipulationDirection = ManipulationDirection.Up;
					}
					else
					{
						_manipulationDirection = ManipulationDirection.Down;
					}
				}
			}
		}


		public void HandleMouseMove(int xPosition, int yPosition, MouseEventArgs e)
		{
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

		public void HandleMouseEnter(int xPosition, int yPosition, MouseEventArgs e)
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

		public void HandleMouseLeave(int xPosition, int yPosition, MouseEventArgs e)
		{
			if (null != _previousNode)
			{
				RaiseEvent(Node.MouseLeaveEvent,_previousNode,e);
				_previousNode = null;
			}
		}


		private void MouseMove(object sender, MouseEventArgs e)
		{
			var position = e.GetPosition(e.OriginalSource as FrameworkElement);
			HandleMouseMove((int)position.X, (int)position.Y, e);
			HandleManipulation(position);
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
			var hitNode = _viewport.GetNodeAtPosition((int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				RaiseEvent(Node.MouseLeftButtonDownEvent,hitNode,e);
				StartManipulation(hitNode, position);
			}
		}


		private void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			var position = e.GetPosition(_game);
			var hitNode = _viewport.GetNodeAtPosition((int)position.X, (int)position.Y);
			if (null != hitNode)
			{
				RaiseEvent(Node.MouseLeftButtonUpEvent, hitNode, e);
				StopManipulation();
			}
		}
	}
}
