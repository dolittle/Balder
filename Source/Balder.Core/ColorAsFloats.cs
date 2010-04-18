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
#if(SILVERLIGHT)
using SysColor = System.Windows.Media.Color;
#else
using SysColor = System.Drawing.Color;
#endif

namespace Balder.Core
{
	public struct ColorAsFloats : IEquatable<ColorAsFloats>, IColor<ColorAsFloats>
	{
		private static readonly Random Rnd = new Random();

		public ColorAsFloats(float red, float green, float blue, float alpha) : this()
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}


		public float Red { get; set; }
		public float Green { get; set; }
		public float Blue { get; set; }
		public float Alpha { get; set; }

		#region Public Static Methods
		public static ColorAsFloats Random()
		{
			var red = (float)Rnd.NextDouble();
			var green = (float)Rnd.NextDouble();
			var blue = (float)Rnd.NextDouble();
			var color = new ColorAsFloats(red, green, blue, 1f);
			return color;
		}

		public static ColorAsFloats FromColor(Color color)
		{
			var colorAsFloats = color.ToColorAsFloats();
			return colorAsFloats;
		}

		public static ColorAsFloats FromSystemColor(SysColor systemColor)
		{
			var color = Color.FromSystemColor(systemColor).ToColorAsFloats();
			return color;
		}

		#endregion

		#region Public Methods

		public ColorAsFloats Additive(ColorAsFloats secondColor)
		{
			var red = Red + secondColor.Red;
			var green = Green + secondColor.Green;
			var blue = Blue + secondColor.Blue;
			var alpha = Alpha + secondColor.Alpha;

			var result = new ColorAsFloats
			{
				Red = (red > 1f ? 1f : red),
				Green = (green > 1f ? 1f : green),
				Blue = (blue > 1f ? 1f : blue),
				Alpha = (alpha > 1f ? 1f : alpha),
			};
			return result;
		}

		public ColorAsFloats Subtract(ColorAsFloats secondColor)
		{
			var red = Red - secondColor.Red;
			var green = Green - secondColor.Green;
			var blue = Blue - secondColor.Blue;
			var alpha = Alpha - secondColor.Alpha;

			var result = new ColorAsFloats
			{
				Red = (red < 0f ? 0f : red),
				Green = (green < 0f ? 0f : green),
				Blue = (blue < 0f ? 0f : blue),
				Alpha = (alpha < 0f ? 0f : alpha),
			};
			return result;
		}

		public ColorAsFloats Average(ColorAsFloats secondColor)
		{
			var red = Red + secondColor.Red;
			var green = Green + secondColor.Green;
			var blue = Blue + secondColor.Blue;
			var alpha = Alpha + secondColor.Alpha;

			var result = new ColorAsFloats
			{
				Red = (red / 2),
				Green = (green / 2),
				Blue = (blue / 2),
				Alpha = (alpha / 2),
			};
			return result;
		}

		public Color ToColor()
		{
			var color = new Color
			{
				Red = ConvertToByte(Red),
				Green = ConvertToByte(Green),
				Blue = ConvertToByte(Blue),
				Alpha = ConvertToByte(Alpha)
			};
			return color;
		}

		public SysColor ToSystemColor()
		{
			var color = ToColor().ToSystemColor();
			return color;
		}

		public uint ToUInt32()
		{
			var color = ToColor();
			var colorAsUInt32 = color.ToUInt32();
			return colorAsUInt32;
		}


		public override string ToString()
		{
			var colorAsString = string.Format("R: {0}, G: {1}, B: {2}, A: {3}", Red, Green, Blue, Alpha);
			return colorAsString;
		}

		public bool Equals(ColorAsFloats other)
		{
			return other.Red == Red &&
				   other.Green == Green &&
				   other.Blue == Blue &&
				   other.Alpha == Alpha;
		}
		#endregion

		#region Operators
		public static implicit operator Color(ColorAsFloats color)
		{
			var newColor = color.ToColor();
			return newColor;
		}
		
		public static implicit operator ColorAsFloats(SysColor color)
		{
			var newColor = FromSystemColor(color);
			return newColor;
		}

		public static implicit operator ColorAsFloats(Color color)
		{
			var newColor = FromColor(color);
			return newColor;
		}

		public static ColorAsFloats operator *(float value, ColorAsFloats firstColor)
		{
			return firstColor*value;
		}

		public static ColorAsFloats operator *(ColorAsFloats firstColor, float value)
		{
			var color = new ColorAsFloats
			            	{
			            		Red = firstColor.Red*value,
			            		Green = firstColor.Green*value,
			            		Blue = firstColor.Blue*value,
			            		Alpha = firstColor.Alpha*value
			            	};
			color.Clamp();
			return color;
		}

		public static ColorAsFloats operator +(ColorAsFloats firstColor, ColorAsFloats secondColor)
		{
			var result = firstColor.Additive(secondColor);
			return result;
		}

		public static ColorAsFloats operator -(ColorAsFloats firstColor, ColorAsFloats secondColor)
		{
			var newColor = firstColor.Subtract(secondColor);
			return newColor;
		}

		public void Clamp()
		{
			Red = ClampValue(Red);
			Green = ClampValue(Green);
			Blue = ClampValue(Blue);
			Alpha = ClampValue(Alpha);
		}

		#endregion



		private static float ClampValue(float value)
		{
			if (value > 1f)
			{
				value = 1f;
			}
			if (value < 0f)
			{
				value = 0f;
			}
			return value;
		}

		private static byte ConvertToByte(float value)
		{
			var convertedValue = (byte)(value * 255f);
			return convertedValue;
		}
	}
}
