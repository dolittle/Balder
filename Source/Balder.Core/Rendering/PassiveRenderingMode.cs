namespace Balder.Core.Rendering
{
	/// <summary>
	/// Rendering mode for a display
	/// </summary>
	public enum PassiveRenderingMode
	{
		/// <summary>
		/// Renders full detail when in passive mode and display is signaled to render
		/// </summary>
		FullDetail=1,

		/// <summary>
		/// Renders all objects in wireframe when in passive mode and display is signaled to render
		/// </summary>
		Wireframe,

		/// <summary>
		/// Renders all objects as bounding-boxes when in passive mode and display is signaled to render
		/// </summary>
		BoundingBox
	}
}