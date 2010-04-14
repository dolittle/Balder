using Balder.Core.Rendering;

namespace Balder.Core
{
	public static class NodeExtensions
	{
		public static bool IsVisible(this INode node)
		{
			if (node is ICanBeVisible &&
			    ((ICanBeVisible)node).IsVisible)
			{
				return true;
			}
			return false;
		}
	}
}