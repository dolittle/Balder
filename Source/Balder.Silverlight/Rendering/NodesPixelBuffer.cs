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
using System.Collections.Generic;
using Balder.Core;
using Balder.Core.Materials;

namespace Balder.Silverlight.Rendering
{
	/// <summary>
	/// Represents a concrete implementation of a nodes pixelbuffer
	/// </summary>
	public class NodesPixelBuffer : INodesPixelBuffer
	{
		public class NodeMaterialAssociation
		{
			public INode Node { get; private set; }
			public Material Material { get; private set; }
			public UInt32 Identifier { get; private set; }

			public NodeMaterialAssociation(INode node, Material material, UInt32 identifier)
			{
				Node = node;
				Material = material;
				Identifier = identifier;
			}

			public static UInt64 GetKey(INode node, Material material)
			{
				var key = (UInt64)node.GetHashCode();
				if( null != material )
				{
					key |= (((UInt64) material.GetHashCode())<<32);
				}
				return key;
			}
		}

		private readonly object _lockObject = new object();

		private Dictionary<UInt32, INode> _nodeIdentifiersByIdentifier;
		private Dictionary<UInt32, Material> _materialIdentifiersByIdentifier;
		private Dictionary<UInt64, NodeMaterialAssociation> _nodeMaterialsByKey;
		private UInt32 _identifierCounter;
		private int _width;
		private int _height;
		private UInt32 _currentFrameIdentifierOffset;
		
		

		public void Initialize(int width, int height)
		{
			_width = width;
			_height = height;
			_nodeIdentifiersByIdentifier = new Dictionary<UInt32, INode>();
			_materialIdentifiersByIdentifier = new Dictionary<UInt32, Material>();
			_nodeMaterialsByKey = new Dictionary<UInt64, NodeMaterialAssociation>();
			RenderingBuffer = new UInt32[width*height];
			Reset();
		}

		public UInt32[] RenderingBuffer { get; private set; }

		public void Reset()
		{
			_identifierCounter = 1;
			NewFrame();
			Clear();
		}

		public void NewFrame()
		{
			_currentFrameIdentifierOffset = _identifierCounter;
			var counter = _identifierCounter + _nodeMaterialsByKey.Count;
			if( counter < _identifierCounter )
			{
				Clear();
			}

			_nodeMaterialsByKey.Clear();
			_nodeIdentifiersByIdentifier.Clear();
		}


		public void Clear()
		{
			Array.Clear(RenderingBuffer,0,RenderingBuffer.Length);
		}


		public UInt32 GetNodeIdentifier(INode node)
		{
			return GetNodeIdentifier(node, null);
		}

		public UInt32 GetNodeIdentifier(INode node, Material material)
		{
			//lock (_lockObject)
			{
				var identifier = 0u;
				var associationKey = NodeMaterialAssociation.GetKey(node, material);
				if( _nodeMaterialsByKey.ContainsKey(associationKey) )
				{
					var association = _nodeMaterialsByKey[associationKey];
					identifier = association.Identifier;
				} else
				{
					identifier = _identifierCounter++;
					var association = new NodeMaterialAssociation(node, material, identifier);
					_nodeMaterialsByKey[associationKey] = association;
					_nodeIdentifiersByIdentifier[identifier] = node;
					_materialIdentifiersByIdentifier[identifier] = material;
				}
				return identifier;
			}
		}

		public void SetNodeAtPosition(INode node, int xPosition, int yPosition)
		{
			SetNodeAtPosition(node, null, xPosition, yPosition);
		}

		public void SetNodeAtPosition(INode node, Material material, int xPosition, int yPosition)
		{
			if( xPosition < 0 || yPosition < 0 || xPosition >= _width || yPosition >= _height )
			{
				return;
			}
			
			var offset = (yPosition*_width) + xPosition;
			var identifier = GetNodeIdentifier(node, material);
			RenderingBuffer[offset] = identifier;
		}

		public INode GetNodeAtPosition(int xPosition, int yPosition)
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

		public Material GetMaterialAtPosition(int xPosition, int yPosition)
		{
			if (xPosition < 0 || yPosition < 0 || xPosition >= _width || yPosition >= _height)
			{
				return null;
			}
			var offset = (yPosition * _width) + xPosition;
			var identifier = RenderingBuffer[offset];
			if (identifier < _currentFrameIdentifierOffset)
			{
				return null;
			}

			if( _materialIdentifiersByIdentifier.ContainsKey(identifier))
			{
				var material = _materialIdentifiersByIdentifier[identifier];
				return material;
			}
			return null;
		}
	}
}
