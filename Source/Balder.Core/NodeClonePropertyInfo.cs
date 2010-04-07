using System.Reflection;

namespace Balder.Core
{
	public class NodeClonePropertyInfo
	{
		public PropertyInfo PropertyInfo { get; set; }
		public bool IsCloneable { get; set; }
	}
}