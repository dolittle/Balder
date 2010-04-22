namespace Balder.Core.Content
{
	public interface IContentCache
	{
		bool Exists<T>(object key);
		object Get<T>(object key);
		void Put<T>(object key, object content);
	}
}