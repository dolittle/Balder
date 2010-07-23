namespace Balder.Rendering
{
	public interface IRuntimeContext
	{
		bool PassiveRendering { get; set; }
		PassiveRenderingMode PassiveRenderingMode { get; set; }
		bool Paused { get; set; }
	}
}