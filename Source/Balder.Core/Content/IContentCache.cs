using Balder.Core.Assets;

namespace Balder.Core.Content
{
	/// <summary>
	/// Represents a caching mechanism for content being created or loaded
	/// </summary>
	public interface IContentCache
	{
		/// <summary>
		/// Check if content is in the cache based upon a key
		/// </summary>
		/// <typeparam name="T">Type of asset to check</typeparam>
		/// <param name="key">Key of the content</param>
		/// <returns>True if content is in the cache, false if not</returns>
		bool Exists<T>(object key) where T : IAsset;

		/// <summary>
		/// Get content from the cache based upon the key
		/// </summary>
		/// <typeparam name="T">Type of asset the content belongs to</typeparam>
		/// <param name="key">Key of the content</param>
		/// <returns>The asset parts in the cache</returns>
		IAssetPart[] Get<T>(object key) where T : IAsset;


		/// <summary>
		/// Put content into the cache based upon a key
		/// </summary>
		/// <typeparam name="T">Type of asset the content belongs to</typeparam>
		/// <param name="key">Key of the content</param>
		/// <param name="parts">AssetParts to put in the cache</param>
		void Put<T>(object key, IAssetPart[] parts) where T : IAsset;
	}
}