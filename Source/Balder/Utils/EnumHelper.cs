#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Balder.Core.Utils
{
	/// <summary>
	/// Helper class for getting values out of Enums
	/// </summary>
	public static class EnumHelper
	{
		/// <summary>
		/// Get all values in an enum specified as a generic parameter
		/// </summary>
		/// <typeparam name="T">Type of enum to get values from</typeparam>
		/// <returns>Array of values in the enum</returns>
		/// <exception cref="ArgumentException">If the type is not an enum type</exception>
		public static T[] GetValues<T>()
		{
			var enumType = typeof(T);
			var values = GetValues(enumType);
			var typesafeValues = GetTypesafeValues<T>(values);
			return typesafeValues;
		}

		/// <summary>
		/// Get all values in an enum specified as a parameter
		/// </summary>
		/// <param name="enumType">Type of enum to get values from</param>
		/// <returns>Array of values in the enum</returns>
		/// <exception cref="ArgumentException">If the type is not an enum type</exception>
		public static object[] GetValues(Type enumType)
		{
			ValidateEnumType(enumType);
			var fields = GetFields(enumType);
			var values = GetValues(enumType, fields);
			return values.ToArray();
		}

		private static void ValidateEnumType(Type enumType)
		{
			if (!enumType.IsEnum)
			{
				throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
			}
		}

		private static T[] GetTypesafeValues<T>(object[] values)
		{
			var typesafeValues = new T[values.Length];
			for (var valueIndex = 0; valueIndex < values.Length; valueIndex++)
			{
				typesafeValues[valueIndex] = (T)values[valueIndex];
			}
			return typesafeValues;
		}

		private static List<object> GetValues(Type enumType, IEnumerable<FieldInfo> fields)
		{
			var values = new List<object>();
			foreach (var field in fields)
			{
				var value = field.GetValue(enumType);
				values.Add(value);
			}
			return values;
		}

		private static IEnumerable<FieldInfo> GetFields(Type enumType)
		{
			return from field in enumType.GetFields()
			       where field.IsLiteral
			       select field;
		}
	}
}