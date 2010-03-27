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
using System.Collections.Generic;
using Balder.Core;

namespace Balder.Silverlight.Rendering
{
	/// <summary>
	/// Represents a concrete implementation of a nodes pixelbuffer
	/// </summary>
	public class NodesPixelBuffer : INodesPixelBuffer
	{
		private readonly object _lockObject = new object();

		private Dictionary<UInt32, Node> _nodeIdentifiersByIdentifier;
		private Dictionary<Node, UInt32> _nodeIdentifiers;
		private UInt32 _nodeIdentifierCounter;
		private int _width;
		private int _height;
		private UInt32 _currentFrameIdentifierOffset;
		

		public void Initialize(int width, int height)
		{
			lock (_lockObject)
			{
				_width = width;
				_height = height;
				_nodeIdentifiers = new Dictionary<Node, UInt32>();
				_nodeIdentifiersByIdentifier = new Dictionary<UInt32, Node>();
				RenderingBuffer = new UInt32[width*height];
			}
		}

		public UInt32[] RenderingBuffer { get; private set; }

		public void Reset()
		{
			lock (_lockObject)
			{
				_nodeIdentifierCounter = 1;
				NewFrame();
				Clear();
			}
		}

		public void NewFrame()
		{
			lock( _lockObject )
			{
				_currentFrameIdentifierOffset = _nodeIdentifierCounter;
				var counter = _nodeIdentifierCounter + _nodeIdentifiers.Count;
				if( counter < _nodeIdentifierCounter )
				{
					Clear();
				}
				
				_nodeIdentifiers.Clear();
				_nodeIdentifiersByIdentifier.Clear();
			}
		}

		public void Clear()
		{
			Array.Clear(RenderingBuffer,0,RenderingBuffer.Length);
		}

		public UInt32 GetNodeIdentifier(Node node)
		{
			lock (_lockObject)
			{
				UInt32 identifier = 0;
				if (_nodeIdentifiers.ContainsKey(node))
				{
					identifier = _nodeIdentifiers[node];
				}
				else
				{
					identifier = _nodeIdentifierCounter;
					_nodeIdentifiers[node] = identifier;
					_nodeIdentifiersByIdentifier[identifier] = node;

					_nodeIdentifierCounter++;
				}
				return identifier;
			}
		}

		public void SetNodeAtPosition(Node node, int xPosition, int yPosition)
		{
			if( xPosition < 0 || yPosition < 0 || xPosition >= _width || yPosition >= _height )
			{
				return;
			}
			
			var offset = (yPosition*_width) + xPosition;
			var identifier = GetNodeIdentifier(node);
			RenderingBuffer[offset] = identifier;
		}

		public Node GetNodeAtPosition(int xPosition, int yPosition)
		{
			if (xPosition < 0 || yPosition < 0 || xPosition >= _width || yPosition >= _height)
			{
				return null;
			}
			var offset = (yPosition * _width) + xPosition;
			var identifier = RenderingBuffer[offset];
			if( identifier < _currentFrameIdentifierOffset )
			{
				return null;
			}

			if( _nodeIdentifiersByIdentifier.ContainsKey(identifier))
			{
				var node = _nodeIdentifiersByIdentifier[identifier];
				return node;
			}

			return null;
		}
	}
}
