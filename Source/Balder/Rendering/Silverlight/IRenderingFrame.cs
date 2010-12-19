namespace Balder.Rendering.Silverlight
{
	public interface IRenderingFrame
	{
		void AddTask(IRenderingTask task, INode node, DetailLevel detailLevel);
		void Render(IRenderingContext renderingContext);
	}
}