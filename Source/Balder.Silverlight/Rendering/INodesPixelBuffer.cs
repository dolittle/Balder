using System;
using Balder.Core;

namespace Balder.Silverlight.Rendering
{
	/// <summary>
	/// Represents a manager for maintaining unique identifiers for Nodes
	/// during rendering per frame
	/// </summary>
	public interface INodesPixelBuffer
	{
		/// <summary>
		/// Gets the actual rendering buffer for "drawing" Nodes identifiers to
		/// </summary>
		UInt32[] RenderingBuffer { get; }

		/// <summary>
		/// Initialize with the displays width and height
		/// </summary>
		/// <param name="width">Width of display</param>
		/// <param name="height">Height of display</param>
		void Initialize(int width, int height);

		/// <summary>
		/// Reset state completely
		/// </summary>
		void Reset();

		/// <summary>
		/// Clear pixel buffer
		/// </summary>
		void Clear();

		/// <summary>
		/// Signal that a new frame rendering will start
		/// </summary>
		void NewFrame();

		/// <summary>
		/// Get identifier for a Node
		/// </summary>
		/// <param name="node">Node to get identifier for</param>
		/// <returns>A unique identifier for the node for the current frame</returns>
		UInt32 GetNodeIdentifier(Node node);

		/// <summary>
		/// Set a specific node at a specific pixel position
		/// </summary>
		/// <param name="node">Node to set</param>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		void SetNodeAtPosition(Node node, int xPosition, int yPosition);

		/// <summary>
		/// Get node that is at a specific position, if any
		/// </summary>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		/// <returns>Node at position, null if there is no node</returns>
		Node GetNodeAtPosition(int xPosition, int yPosition);
	}
}