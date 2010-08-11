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
using System.ComponentModel;
using System.Windows;
using Balder.Display;
using Balder.Execution;
using Balder.Extensions.Silverlight;
using Balder.Objects.Geometries;


namespace Balder.Input.Silverlight
{
	public class ManipulationInfo : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => {};

		private INode _node;
		public INode Node
		{
			get { return _node; }
			set
			{
				_node = value;
				PropertyChanged.Notify(() => Node);
			}
		}

		private int _deltaX;
		public int DeltaX
		{
			get { return _deltaX; }
			set
			{
				_deltaX = value;
				PropertyChanged.Notify(() => DeltaX);
			}
		}

		private int _deltaY;
		public int DeltaY
		{
			get { return _deltaY; }
			set
			{
				_deltaY = value;
				PropertyChanged.Notify(() => DeltaY);
			}
		}

		private bool _isManipulating;
		public bool IsManipulating
		{
			get { return _isManipulating; }
			set
			{
				_isManipulating = value;
				PropertyChanged.Notify(() => IsManipulating);
			}
		}
	}


	public class ManipulationEventHelper
	{
		private readonly Viewport _viewport;
		private Point _previousPosition;
		private ManipulationDirection _manipulationDirection;
		private bool _manipulating;
		private Node _nodeBeingManipulated;
		private bool _manipulationStarted;
		private ManipulationDeltaEventArgs _deltaEventArgs;

		public static ManipulationInfo	ManipulationInfo = new ManipulationInfo();

		public ManipulationEventHelper(Viewport viewport)
		{
			_viewport = viewport;
			_deltaEventArgs = new ManipulationDeltaEventArgs();
			ResetManipulation();
		}

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

			if (node is Geometry)
			{
				var pickRay = _viewport.GetPickRay((int)position.X, (int)position.Y);

				var geometry = node as Geometry;
				Face face = null;
				var faceIndex = -1;
				var faceU = 0f;
				var faceV = 0f;

				var distance = geometry.Intersects(pickRay, out face, out faceIndex, out faceU, out faceV);
				if (null != face)
				{
					var material = face.Material;

					_deltaEventArgs.Material = material;
					_deltaEventArgs.Face = face;
					_deltaEventArgs.FaceIndex = faceIndex;
					_deltaEventArgs.FaceU = faceU;
					_deltaEventArgs.FaceV = faceV;
					_deltaEventArgs.Distance = distance.Value;
				}
			}
		}

		public bool IsManipulating { get { return _manipulating; } }

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
#endif