namespace Balder.Execution
{
	public interface ICanNotifyChanges
	{
		void Notify(string propertyName, object oldValue, object newValue);
	}
}
