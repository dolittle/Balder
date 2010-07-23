using System;
using Balder;
using Balder.Materials;

namespace Balder.Rendering.Silverlight
{
	/// <summary>
	/// Represents a manager for maintaining unique identifiers for Nodes
	/// during rendering per frame
	/// </summary>
	public interface IMetaDataPixelBuffer
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
		/// Get identifier to be used during rendering
		/// </summary>
		/// <param name="node">Node to get identifier for</param>
		/// <returns>A unique identifier for the node for the current frame</returns>
		UInt32 GetIdentifier(INode node);

		/// <summary>
		/// Get identifier to be used during rendering
		/// </summary>
		/// <param name="node">Node to get identifier for</param>
		/// <param name="material">Material associated with node</param>
		/// <returns>A unique identifier for the node for the current frame</returns>
		UInt32 GetIdentifier(INode node, Material material);

		/// <summary>
		/// Get identifier to be used during rendering
		/// </summary>
		/// <param name="node">Node to get identifier for</param>
		/// <param name="renderFace">RenderFace associated with the node</param>
		/// <returns>A unique identifier for the node for the current frame</returns>
		UInt32 GetIdentifier(INode node, RenderFace renderFace);

		/// <summary>
		/// Get identifier to be used during rendering
		/// </summary>
		/// <param name="node">Node to get identifier for</param>
		/// <param name="renderFace">RenderFace associated with the node</param>
		/// <param name="material">Material associated with node</param>
		/// <returns>A unique identifier for the node for the current frame</returns>
		UInt32 GetIdentifier(INode node, RenderFace renderFace, Material material);

		/// <summary>
		/// Set a specific node at a specific pixel position
		/// </summary>
		/// <param name="node">Node to set</param>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		void SetNodeAtPosition(INode node, int xPosition, int yPosition);

		/// <summary>
		/// Set a specific node associated with a material at a specific pixel position
		/// </summary>
		/// <param name="node">Node to set</param>
		/// <param name="material">Material to associate at position</param>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		void SetNodeAtPosition(INode node, Material material, int xPosition, int yPosition);

		/// <summary>
		/// Set a specific node associated with a material at a specific pixel position
		/// </summary>
		/// <param name="node">Node to set</param>
		/// <param name="face">Face to associate at position</param>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		void SetNodeAtPosition(INode node, RenderFace face, int xPosition, int yPosition);

		/// <summary>
		/// Set a specific node associated with a material at a specific pixel position
		/// </summary>
		/// <param name="node">Node to set</param>
		/// <param name="face">Face to associate at position</param>
		/// <param name="material">Material to associate at position</param>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		void SetNodeAtPosition(INode node, RenderFace face, Material material, int xPosition, int yPosition);

		/// <summary>
		/// Get node that is at a specific position, if any
		/// </summary>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		/// <returns>Node at position, null if there is no node</returns>
		INode GetNodeAtPosition(int xPosition, int yPosition);

		/// <summary>
		/// Get material that is at a specific position, if any
		/// </summary>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		/// <returns>Material at position, null if there is no material</returns>
		Material GetMaterialAtPosition(int xPosition, int yPosition);

		/// <summary>
		/// Get RenderFace that is at a specific position, if any
		/// </summary>
		/// <param name="xPosition">X position</param>
		/// <param name="yPosition">Y position</param>
		/// <returns>RenderFace at position, null if there is no RenderFace</returns>
		RenderFace GetRenderFaceAtPosition(int xPosition, int yPosition);
	}
}