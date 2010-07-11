namespace Balder.Core.Rendering
{
	public interface IRuntimeContext
	{
		bool PassiveRendering { get; set; }
		PassiveRenderingMode PassiveRenderingMode { get; set; }
	}
}