﻿using System.Windows;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Input;

namespace Balder.Core.Silverlight.Input
{
	public class ManipulationEventHelper
	{
		private readonly Viewport _viewport;
		private Point _previousPosition;
		private ManipulationDirection _manipulationDirection;
		private bool _manipulating;
		private Node _nodeBeingManipulated;

		public ManipulationEventHelper(Viewport viewport)
		{
			_viewport = viewport;
			ResetManipulation();
		}

		private void RaiseManipulationEvent(BubbledEvent<Node, ManipulationDeltaEventHandler> bubbledEvent, Node node, Point position)
		{
			var deltaX = position.X - _previousPosition.X;
			var deltaY = position.Y - _previousPosition.Y;
			HandleManipulationDirection(deltaX, deltaY);

			var material = _viewport.Display.GetMaterialAtPosition((int)position.X, (int)position.Y);
			bubbledEvent.Raise(node, node, new ManipulationDeltaEventArgs(material, (int)deltaX, (int)deltaY, _manipulationDirection));
		}

		private void ResetManipulation()
		{
			_manipulating = false;
			_manipulationDirection = ManipulationDirection.None;
		}

		public void StartManipulation(Node node, Point position)
		{
			_manipulating = true;
			_nodeBeingManipulated = node;

			Node.ManipulationStartedEvent.Raise(node, node, BubbledEventArgs.Empty);
		}

		public void HandleManipulation(Point position)
		{
			if (_manipulating)
			{
				RaiseManipulationEvent(Node.ManipulationDeltaEvent, _nodeBeingManipulated, position);
			}
			_previousPosition = position;
		}

		public void StopManipulation()
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
	}
}