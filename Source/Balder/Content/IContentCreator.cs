using Balder.Materials;
using Balder.Objects.Geometries;

namespace Balder.Content
{
	/// <summary>
	/// Handles the creation of content programatically
	/// </summary>
	public interface IContentCreator
	{
		/// <summary>
		/// Creates a geometry based on the geometry type
		/// </summary>
		/// <typeparam name="T">Type of geometry to create</typeparam>
		/// <returns>An instance of the geometry created</returns>
		T CreateGeometry<T>() where T : Geometry;

		/// <summary>
		/// Creates a material
		/// </summary>
		/// <returns>An instance of a Material</returns>
		Material CreateMaterial();
	}
}