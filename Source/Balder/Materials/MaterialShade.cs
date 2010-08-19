namespace Balder.Materials
{
	/// <summary>
	/// Specifies the shading model for a material
	/// </summary>
	public enum MaterialShade
	{
		/// <summary>
		/// No shading
		/// </summary>
		None=1,

		/// <summary>
		/// Flat shading
		/// </summary>
		Flat,

		/// <summary>
		/// Gouraud - smooth shading - linearly interpolated
		/// </summary>
		Gouraud
	}
}