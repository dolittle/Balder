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
using System.Windows;
using Balder.Display;
using Balder.Execution;
using Balder.Math;
using Balder.Objects.Geometries;


namespace Balder.Input.Silverlight
{
	public class ManipulationEventHelper
	{
		private readonly Viewport _viewport;
		private Point _previousPosition;
		private ManipulationDirection _manipulationDirection;
		private bool _manipulating;
		private Node _nodeBeingManipulated;
		private bool _manipulationStarted;

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

			if (node is Geometry)
			{
				var pickRay = _viewport.GetPickRay((int)position.X, (int)position.Y);

				var geometry = node as Geometry;
				var vertices = geometry.FullDetailLevel.GetVertices();
				var faces = geometry.FullDetailLevel.GetFaces();


				Face closestFace = null;
				var closestFaceIndex = -1;
				var closestDistance = float.MaxValue;

				for (var faceIndex = 0; faceIndex < faces.Length; faceIndex++)
				{
					var face = faces[faceIndex];
					var vertex1 = vertices[face.A].ToVector();
					var vertex2 = vertices[face.B].ToVector();
					var vertex3 = vertices[face.C].ToVector();

					var rayDistance = pickRay.IntersectsTriangle(vertex1, vertex2, vertex3);

					if (null == rayDistance || rayDistance < 0)
					{
						continue;
					}

					if( rayDistance < closestDistance )
					{
						closestFace = face;
						closestFaceIndex = faceIndex;
						closestDistance = rayDistance.Value;
					}
				}

				if (null != closestFace)
				{
					var material = closestFace.Material;
					var faceIndex = closestFaceIndex;
					var face = closestFace;
					bubbledEvent.Raise(node, node,
					                   new ManipulationDeltaEventArgs(material, face, faceIndex, (int) deltaX, (int) deltaY,
					                                                  _manipulationDirection));
				}

			}



			/*
			var material = _viewport.Display.GetMaterialAtPosition((int)position.X, (int)position.Y);
			var faceIndex = _viewport.Display.GetFaceIndexAtPosition((int)position.X, (int)position.Y);
			var face = _viewport.Display.GetFaceAtPosition((int)position.X, (int)position.Y);
			bubbledEvent.Raise(node, node, new ManipulationDeltaEventArgs(material, face, faceIndex, (int)deltaX, (int)deltaY, _manipulationDirection));
			 * */
		}

		private void ResetManipulation()
		{
			_manipulating = false;
			_manipulationDirection = ManipulationDirection.None;
			_manipulationStarted = false;
			_nodeBeingManipulated = null;
		}

		public void StartManipulation(Node node, Point position)
		{
			_manipulating = true;
			_nodeBeingManipulated = node;
			_manipulationStarted = true;
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