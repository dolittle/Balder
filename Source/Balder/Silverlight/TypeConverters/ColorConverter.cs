﻿#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
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
#if(XAML)
using System;
using System.ComponentModel;
using System.Globalization;

namespace Balder.Silverlight.TypeConverters
{
	public class ColorConverter : TypeConverter
	{
		public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
		{
			return sourceType.Equals(typeof(string));
		}

		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
		{
			return destinationType.Equals(typeof(string));
		}

		public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
		{
			if (value is string)
			{
                var type = typeof(System.Windows.Media.Colors);
				var colorProperty = type.GetProperty((string) value);
				if (null != colorProperty)
				{
					var color = (System.Windows.Media.Color) colorProperty.GetValue(null, null);
					return Color.FromSystemColor(color);
				}
				else
				{
					string colorStr = (string)value;
					if (colorStr.StartsWith("#"))
					{
						if (colorStr.Length == 9)
						{
							byte alpha = Byte.Parse(colorStr.Substring(1, 2), NumberStyles.AllowHexSpecifier);
							byte red = Byte.Parse(colorStr.Substring(3, 2), NumberStyles.AllowHexSpecifier);
							byte green = Byte.Parse(colorStr.Substring(5, 2), NumberStyles.AllowHexSpecifier);
							byte blue = Byte.Parse(colorStr.Substring(7, 2), NumberStyles.AllowHexSpecifier);
							return new Color(red, green, blue, alpha);
						}
						else if (colorStr.Length == 7)
						{
							byte red = Byte.Parse(colorStr.Substring(1, 2), NumberStyles.AllowHexSpecifier);
							byte green = Byte.Parse(colorStr.Substring(3, 2), NumberStyles.AllowHexSpecifier);
							byte blue = Byte.Parse(colorStr.Substring(5, 2), NumberStyles.AllowHexSpecifier);
							return new Color(red, green, blue, 255);
						}
					}

					return Colors.Black;
				}
			}
			return base.ConvertFrom(context, culture, value);
		}
	}
}
#endif