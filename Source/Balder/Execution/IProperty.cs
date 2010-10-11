using Balder.Rendering;

namespace Balder.Execution
{
	public interface IProperty
	{
		void SetRuntimeContext(object obj, IRuntimeContext runtimeContext);
		void RemoveObjectProperties(object obj);
		void CleanupChildren(object previousValue);
	}
}