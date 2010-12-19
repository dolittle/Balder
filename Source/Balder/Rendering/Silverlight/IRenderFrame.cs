namespace Balder.Rendering.Silverlight
{
	public interface IRenderFrame
	{
		void AddTask(IRenderTask task, INode node, DetailLevel detailLevel);
		void Render(IRenderingContext renderingContext);
	}
}