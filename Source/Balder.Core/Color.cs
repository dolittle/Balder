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
using System.ComponentModel;
using Balder.Core.Silverlight.TypeConverters;
#if(SILVERLIGHT)
using SysColor = System.Windows.Media.Color;
#else
using SysColor = System.Drawing.Color;
#endif

namespace Balder.Core
{
	/// <summary>
	/// Represents a color
	/// </summary>
#if(SILVERLIGHT)
	[TypeConverter(typeof(ColorConverter))]
#endif
	public struct Color : IEquatable<Color>, IColor<Color>
	{
		private static readonly Random Rnd = new Random();

		public Color(byte red, byte green, byte blue, byte alpha)
			: this()
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		public byte Red { get; set; }
		public byte Green { get; set; }
		public byte Blue { get; set; }
		public byte Alpha { get; set; }


		#region Public Static Methods
		public static Color Random()
		{
			var red = (byte)Rnd.Next(0, 64);
			var green = (byte)Rnd.Next(0, 64);
			var blue = (byte)Rnd.Next(0, 64);
			var color = new Color(red, green, blue, 0xff);
			return color;
		}

		public static Color FromArgb(byte alpha, byte red, byte green, byte blue)
		{
			var color = new Color
							{
								Red = red,
								Green = green,
								Blue = blue,
								Alpha = alpha
							};
			return color;
		}


		public static Color FromSystemColor(SysColor systemColor)
		{
			var color = new Color
			{
				Red = systemColor.R,
				Green = systemColor.G,
				Blue = systemColor.B,
				Alpha = systemColor.A
			};
			return color;
		}


		#endregion

		#region Public Methods
		public SysColor ToSystemColor()
		{
			var sysColor = SysColor.FromArgb(Alpha, Red, Green, Blue);
			return sysColor;
		}

		public UInt32 ToUInt32()
		{
			var uint32Color = (((UInt32)Alpha) << 24) |
								(((UInt32)Red) << 16) |
								(((UInt32)Green) << 8) |
								(UInt32)Blue;
			return uint32Color;
		}


		public Color Additive(Color secondColor)
		{
			var red = (int)Red + (int)secondColor.Red;
			var green = (int)Green + (int)secondColor.Green;
			var blue = (int)Blue + (int)secondColor.Blue;
			var alpha = (int)Alpha + (int)secondColor.Alpha;

			var result = new Color
							{
								Red = (byte)(red > 255 ? 255 : red),
								Green = (byte)(green > 255 ? 255 : green),
								Blue = (byte)(blue > 255 ? 255 : blue),
								Alpha = (byte)(alpha > 255 ? 255 : alpha),
							};
			return result;
		}

		public Color Subtract(Color secondColor)
		{
			var red = (int)Red - (int)secondColor.Red;
			var green = (int)Green - (int)secondColor.Green;
			var blue = (int)Blue - (int)secondColor.Blue;
			var alpha = (int)Alpha - (int)secondColor.Alpha;

			var result = new Color
			{
				Red = (byte)(red < 0 ? 0 : red),
				Green = (byte)(green < 0 ? 0 : green),
				Blue = (byte)(blue < 0 ? 0 : blue),
				Alpha = (byte)(alpha < 0 ? 0 : alpha),
			};
			return result;
		}

		public Color Average(Color secondColor)
		{
			var red = (int)Red + (int)secondColor.Red;
			var green = (int)Green + (int)secondColor.Green;
			var blue = (int)Blue + (int)secondColor.Blue;
			var alpha = (int)Alpha + (int)secondColor.Alpha;

			var result = new Color
			{
				Red = (byte)(red >> 1),
				Green = (byte)(green >> 1),
				Blue = (byte)(blue >> 1),
				Alpha = (byte)(alpha >> 1),
			};
			return result;
		}


		public ColorAsFloats ToColorAsFloats()
		{
			var color = new ColorAsFloats
							{
								Red = ConvertToFloat(Red),
								Green = ConvertToFloat(Green),
								Blue = ConvertToFloat(Blue),
								Alpha = ConvertToFloat(Alpha)
							};
			return color;
		}


		public bool Equals(Color other)
		{
			return other.Red == Red &&
				   other.Green == Green &&
				   other.Blue == Blue &&
				   other.Alpha == Alpha;
		}

		public override string ToString()
		{
			var colorAsString = string.Format("R: {0}, G: {1}, B: {2}, A: {3}", Red, Green, Blue, Alpha);
			return colorAsString;
		}
		#endregion

		#region Operators
		public static Color operator +(Color firstColor, Color secondColor)
		{
			var result = firstColor.Additive(secondColor);
			return result;
		}

		public static Color operator -(Color firstColor, Color secondColor)
		{
			var newColor = firstColor.Subtract(secondColor);
			return newColor;
		}

		public static implicit operator Color(SysColor color)
		{
			var newColor = FromSystemColor(color);
			return newColor;
		}
		#endregion

		private static float ConvertToFloat(byte value)
		{
			var valueAsFloat = (float)value;
			var convertedValue = (float)System.Math.Round(valueAsFloat / 255f, 2);
			return convertedValue;
		}

	}
}
