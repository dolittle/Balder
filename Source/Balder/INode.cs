using Balder.Display;
using Balder.Execution;
using Balder.Math;

namespace Balder
{
	public interface INode : IHaveLabel
	{
		INode Parent { get; }

		Matrix ActualWorld { get; }
		Matrix RenderingWorld { get; set; }
		
		Scene Scene { get; set; }

		NodeStatistics Statistics { get; }
		BoundingSphere BoundingSphere { get; set; }

		/// <summary>
		/// Gets or sets wether or not Node should cause any hit test during interaction
		/// with input devices such as mouse or touch
		/// 
		/// Default value is true - meaning that Hit Test will happen
		/// </summary>
		bool IsHitTestEnabled { get; set; }

		/// <summary>
		/// Gets or sets wether or not Node should be considered during intersection testing
		/// 
		/// Default value is true - meaning that intersection testing will occur
		/// </summary>
		bool IsIntersectionTestEnabled { get; set; }

		void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world);
	}
}