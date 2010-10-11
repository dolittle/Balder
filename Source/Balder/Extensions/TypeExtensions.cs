using System;

namespace Balder.Extensions
{
	public static class TypeExtensions
	{
		public static bool HasInterface<T>(this Type type)
		{
			var hasInterface = type.GetInterface(typeof(T).Name, false) != null;
			return hasInterface;
		}
	}
}