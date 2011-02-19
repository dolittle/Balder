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
#if(XAML)
using System.Windows;
using Balder.Display;
using Balder.Execution;
using Balder.Objects.Geometries;


namespace Balder.Input.Silverlight
{
	public class ManipulationEventHelper
	{
		private static readonly Point ZeroPoint = new Point(0, 0);
		private readonly Viewport _viewport;
		
		private ManipulationDirection _manipulationDirection;
		private bool _manipulating;
		private Node _nodeBeingManipulated;
		
		private ManipulationDeltaEventArgs _deltaEventArgs;

#if(!SILVERLIGHT4 && !WINDOWS_PHONE)
		private Point _previousPosition;
		private bool _manipulationStarted;
#endif

		public static ManipulationInfo	ManipulationInfo = new ManipulationInfo();

		public ManipulationEventHelper(Game game, Viewport viewport)
		{
			_viewport = viewport;
			_deltaEventArgs = new ManipulationDeltaEventArgs();
#if(SILVERLIGHT4 || WINDOWS_PHONE)
			AddEvents(game);
#else
			ResetManipulation();
#endif
		}

		public bool IsManipulating { get { return _manipulating; } }

#if(SILVERLIGHT4 || WINDOWS_PHONE)
		private void AddEvents(FrameworkElement element)
		{
			element.ManipulationStarted += ElementManipulationStarted;
			element.ManipulationDelta += ElementManipulationDelta;
			element.ManipulationCompleted += ElementManipulationCompleted;
		}


		void ElementManipulationCompleted(object sender, System.Windows.Input.ManipulationCompletedEventArgs e)
		{
			Node.ManipulationStoppedEvent.Raise(_nodeBeingManipulated, _nodeBeingManipulated, BubbledEventArgs.Empty);
			_manipulating = false;
			_nodeBeingManipulated = null;
		}

		void ElementManipulationDelta(object sender, System.Windows.Input.ManipulationDeltaEventArgs e)
		{
			if( _manipulating )
			{
				RaiseManipulationEvent(Node.ManipulationDeltaEvent, _nodeBeingManipulated, e.ManipulationOrigin, e.DeltaManipulation.Translation);
			}
		}

		void ElementManipulationStarted(object sender, System.Windows.Input.ManipulationStartedEventArgs e)
		{
			var hitNode = _viewport.GetNodeAtPosition((int)e.ManipulationOrigin.X, (int)e.ManipulationOrigin.Y);
			if( null != hitNode )
			{
				_nodeBeingManipulated = hitNode;
				PopulateManipulationEvent(hitNode, e.ManipulationOrigin);
				RaiseManipulationEvent(Node.ManipulationStartedEvent, hitNode, e.ManipulationOrigin, ZeroPoint);
				_manipulating = true;
			}
		}

		private void RaiseManipulationEvent(BubbledEvent<Node, ManipulationDeltaEventHandler> bubbledEvent, Node node, Point position, Point delta)
		{
			var deltaX = (int) delta.X;
			var deltaY = (int)delta.Y;
			ManipulationInfo.DeltaX = deltaX;
			ManipulationInfo.DeltaY = deltaY;
			HandleManipulationDirection(deltaX, deltaY);

			_deltaEventArgs.DeltaX = deltaX;
			_deltaEventArgs.DeltaY = deltaY;
			_deltaEventArgs.Direction = _manipulationDirection;
			_deltaEventArgs.Viewport = _viewport;

			bubbledEvent.Raise(node, node, _deltaEventArgs);
		}

#else


		private void RaiseManipulationEvent(BubbledEvent<Node, ManipulationDeltaEventHandler> bubbledEvent, Node node, Point position)
		{
			var deltaX = position.X - _previousPosition.X;
			var deltaY = position.Y - _previousPosition.Y;

			ManipulationInfo.DeltaX = (int)deltaX;
			ManipulationInfo.DeltaY = (int)deltaY;
			HandleManipulationDirection(deltaX, deltaY);

			_deltaEventArgs.DeltaX = (int)deltaX;
			_deltaEventArgs.DeltaY = (int) deltaY;
			_deltaEventArgs.Direction = _manipulationDirection;
			_deltaEventArgs.Viewport = _viewport;

			bubbledEvent.Raise(node, node, _deltaEventArgs);
		}

		private void ResetManipulation()
		{
			_manipulating = false;
			_manipulationDirection = ManipulationDirection.None;
			_manipulationStarted = false;
			_nodeBeingManipulated = null;
			ManipulationInfo.IsManipulating = false;
			ManipulationInfo.Node = null;
		}

		public void StartManipulation(Node node, Point position)
		{
			_manipulating = true;
			_nodeBeingManipulated = node;
			_manipulationStarted = true;
			_previousPosition = position;
			ManipulationInfo.IsManipulating = true;
			ManipulationInfo.Node = node;

			PopulateManipulationEvent(node, position);
		}

		

		public void HandleManipulation(Point position)
		{
			if (_manipulating)
			{
				if (_manipulationStarted)
				{
					RaiseManipulationEvent(Node.ManipulationStartedEvent, _nodeBeingManipulated, position);
					_manipulationStarted = false;
				}
				RaiseManipulationEvent(Node.ManipulationDeltaEvent, _nodeBeingManipulated, position);
			}
			_previousPosition = position;
		}

		public void StopManipulation()
		{
			Node.ManipulationStoppedEvent.Raise(_nodeBeingManipulated, _nodeBeingManipulated, BubbledEventArgs.Empty);
			ResetManipulation();
		}

#endif
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


		private void PopulateManipulationEvent(INode node, Point position)
		{
			if (node is Geometry)
			{
				var pickRay = _viewport.GetPickRay((int)position.X, (int)position.Y);

				var geometry = node as Geometry;
				Face face = null;
				var faceIndex = -1;
				var faceU = 0f;
				var faceV = 0f;

				var distance = geometry.Intersects(_viewport, pickRay, out face, out faceIndex, out faceU, out faceV);
				if (null != face)
				{
					var material = geometry.Material;

					_deltaEventArgs.Material = material;
					_deltaEventArgs.Face = face;
					_deltaEventArgs.FaceIndex = faceIndex;
					_deltaEventArgs.FaceU = faceU;
					_deltaEventArgs.FaceV = faceV;
					_deltaEventArgs.Distance = distance.Value;
				}
			}
		}
	}
}
#endif