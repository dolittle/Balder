namespace Balder.Core
{
	public delegate void PreparedEventHandler(ICanPrepare preparedObject);

	public interface ICanPrepare
	{
		event PreparedEventHandler Prepared;

		void Prepare();
		void InvalidatePrepare();
	}
}