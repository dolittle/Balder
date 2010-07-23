using System.Reflection;

namespace Balder
{
	public class NodeClonePropertyInfo
	{
		public PropertyInfo PropertyInfo { get; set; }
		public bool IsCloneable { get; set; }
		public bool IsCopyable { get; set; }
	}
}