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
using Balder.Core.Execution;
using Balder.Core.Materials;
using Ninject;

namespace Balder.Silverlight.Rendering
{
	/// <summary>
	/// Represents a concrete implementation of a nodes pixelbuffer
	/// </summary>
	[Singleton]	// TODO: This should be available in the GeometryDetailLevel as an injected property - That is why its a Singleton for now
	public class MetaDataPixelBuffer : IMetaDataPixelBuffer
	{
		public class PixelIdentifier
		{
			public INode Node { get; private set; }
			public Material Material { get; private set; }
			public RenderFace Face { get; private set; }
			public UInt32 Identifier { get; private set; }

			public PixelIdentifier(INode node, Material material, RenderFace renderFace, UInt32 identifier)
			{
				Node = node;
				Material = material;
				Identifier = identifier;
			}

			public static UInt64 GetKey(INode node, Material material, RenderFace renderFace)
			{
				var nodeId = (UInt64)node.Id;
				UInt64 faceIndex = 0;
				UInt64 materialId = 0;
				if (null != renderFace)
				{
					faceIndex = ((UInt64)renderFace.Index) << 16;
				}
				if (null != material)
				{
					materialId = ((UInt64)material.Id) << 32;
				}
				var key = nodeId | faceIndex | materialId;
				return key;
			}
		}

		private Dictionary<UInt32, INode> _nodeIdentifiersByIdentifier;
		private Dictionary<UInt32, Material> _materialIdentifiersByIdentifier;
		private Dictionary<UInt32, RenderFace> _renderFacesByIdentifier;
		private Dictionary<UInt64, PixelIdentifier> _identifiersByKey;
		private UInt32 _identifierCounter;
		private int _width;
		private int _height;
		private UInt32 _currentFrameIdentifierOffset;


		public void Initialize(int width, int height)
		{
			_width = width;
			_height = height;
			_nodeIdentifiersByIdentifier = new Dictionary<UInt32, INode>();
			_renderFacesByIdentifier = new Dictionary<uint, RenderFace>();
			_materialIdentifiersByIdentifier = new Dictionary<UInt32, Material>();
			_identifiersByKey = new Dictionary<UInt64, PixelIdentifier>();
			RenderingBuffer = new UInt32[width * height];
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
			var counter = _identifierCounter + _identifiersByKey.Count;
			if (counter < _identifierCounter)
			{
				Clear();
			}

			_identifiersByKey.Clear();
			_nodeIdentifiersByIdentifier.Clear();
			_materialIdentifiersByIdentifier.Clear();
			_renderFacesByIdentifier.Clear();
		}


		public void Clear()
		{
			Array.Clear(RenderingBuffer, 0, RenderingBuffer.Length);
		}


		public UInt32 GetIdentifier(INode node)
		{
			return GetIdentifier(node, null, null);
		}

		public UInt32 GetIdentifier(INode node, Material material)
		{
			return GetIdentifier(node, null, material);
		}

		public uint GetIdentifier(INode node, RenderFace renderFace)
		{
			return GetIdentifier(node, renderFace, null);
		}

		public uint GetIdentifier(INode node, RenderFace renderFace, Material material)
		{
			var identifier = 0u;
			var associationKey = PixelIdentifier.GetKey(node, material, renderFace);
			if (_identifiersByKey.ContainsKey(associationKey))
			{
				var association = _identifiersByKey[associationKey];
				identifier = association.Identifier;
			}
			else
			{
				identifier = _identifierCounter++;
				var association = new PixelIdentifier(node, material, renderFace, identifier);
				_identifiersByKey[associationKey] = association;
				_nodeIdentifiersByIdentifier[identifier] = node;
				if (null != material)
				{
					_materialIdentifiersByIdentifier[identifier] = material;
				}
				if (null != renderFace)
				{
					_renderFacesByIdentifier[identifier] = renderFace;
				}
			}
			return identifier;
		}

		public void SetNodeAtPosition(INode node, int xPosition, int yPosition)
		{
			SetNodeAtPosition(node, null, null, xPosition, yPosition);
		}

		public void SetNodeAtPosition(INode node, Material material, int xPosition, int yPosition)
		{
			SetNodeAtPosition(node, null, material, xPosition, yPosition);
		}

		public void SetNodeAtPosition(INode node, RenderFace face, int xPosition, int yPosition)
		{
			SetNodeAtPosition(node, face, null, xPosition, yPosition);
		}

		public void SetNodeAtPosition(INode node, RenderFace face, Material material, int xPosition, int yPosition)
		{
			if (xPosition < 0 || yPosition < 0 || xPosition >= _width || yPosition >= _height)
			{
				return;
			}

			var offset = (yPosition * _width) + xPosition;
			var identifier = GetIdentifier(node, face, material);
			RenderingBuffer[offset] = identifier;
		}

		private UInt32 GetIdentifierAtPosition(int xPosition, int yPosition)
		{
			if (xPosition < 0 || yPosition < 0 || xPosition >= _width || yPosition >= _height)
			{
				return 0;
			}
			var offset = (yPosition * _width) + xPosition;
			var identifier = RenderingBuffer[offset];
			if (identifier < _currentFrameIdentifierOffset)
			{
				return 0;
			}
			return identifier;
		}

		public INode GetNodeAtPosition(int xPosition, int yPosition)
		{
			var identifier = GetIdentifierAtPosition(xPosition, yPosition);
			if (identifier == 0)
			{
				return null;
			}

			if (_nodeIdentifiersByIdentifier.ContainsKey(identifier))
			{
				var node = _nodeIdentifiersByIdentifier[identifier];
				return node;
			}

			return null;
		}

		public Material GetMaterialAtPosition(int xPosition, int yPosition)
		{
			var identifier = GetIdentifierAtPosition(xPosition, yPosition);
			if (identifier == 0)
			{
				return null;
			}

			if (_materialIdentifiersByIdentifier.ContainsKey(identifier))
			{
				var material = _materialIdentifiersByIdentifier[identifier];
				return material;
			}
			return null;
		}

		public RenderFace GetRenderFaceAtPosition(int xPosition, int yPosition)
		{
			var identifier = GetIdentifierAtPosition(xPosition, yPosition);
			if (identifier == 0)
			{
				return null;
			}
			if( _renderFacesByIdentifier.ContainsKey(identifier))
			{
				var renderFace = _renderFacesByIdentifier[identifier];
				return renderFace;
			}
			return null;
		}
	}
}
