#region License
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
using System;
#if(XAML)
using System.ComponentModel;
using System.Runtime.InteropServices;
using Balder.Execution;
using Balder.Silverlight.TypeConverters;
using SysColor = System.Windows.Media.Color;
#else
#if(!IOS)
using System.Runtime.InteropServices;
using SysColor = System.Drawing.Color;
#endif
#endif
using Balder.Math;

namespace Balder
{
	/// <summary>
	/// Represents a color used throughout Balder
	/// </summary>
#if(XAML)
	[TypeConverter(typeof(ColorConverter))]
#endif
#if(WINDOWS_PHONE)
	[StructLayout(LayoutKind.Sequential, Size = 4)]
#else 
#if(!IOS)
	[StructLayout(LayoutKind.Sequential, Pack = 1, Size = 4)]
#endif
#endif
	public struct Color : IEquatable<Color>
	{
        static float OneMultiplier = 1f/255f;

		static readonly Random Rnd = new Random();
		static readonly int RedMask;
		static readonly int GreenMask;
		static readonly int BlueMask;
		static readonly int AlphaMask;
		static readonly int AlphaShiftedMask;
		static readonly int RedUnderflow;
		static readonly int GreenUnderflow;
		static readonly int BlueUnderflow;
		static readonly int AlphaUnderflow;
		static readonly int AlphaShiftedUnderflow;
		public static int AlphaFull;

		static Color()
		{
			RedMask = 0x00ff0000;
			RedUnderflow = 0x0000ffff;

			GreenMask = 0x0000ff00;
			GreenUnderflow = 0x000000ff;

			BlueMask = 0x000000ff;
			BlueUnderflow = 0x00000000;

			uint a = 0xff000000;
			AlphaMask = (int)a;
			AlphaUnderflow = 0x00ffffff;
			AlphaFull = (int)a;

			AlphaShiftedMask = 0x00ff0000;
			AlphaShiftedUnderflow = 0x0000ffff;
		}


        byte _red;
        byte _green;
        byte _blue;
        byte _alpha;

		/// <summary>
		/// Creates an instance of <see cref="Color"/> with all channels initialized
		/// </summary>
		/// <param name="red">Value for Red channel</param>
		/// <param name="green">Value for Green channel</param>
		/// <param name="blue">Value for Blue channel</param>
		/// <param name="alpha">Value for Alpha channel</param>
		public Color(byte red, byte green, byte blue, byte alpha)
			: this()
		{
			Red = red;
			Green = green;
			Blue = blue;
			Alpha = alpha;
		}

		/// <summary>
		/// Gets or sets the Red channel value
		/// </summary>
		public byte Red 
        {
            get { return _red; }
            set
            {
                _red = value;
                RedAsFloat = ((float)_red) * OneMultiplier;
            }
        }

		/// <summary>
		/// Gets or sets the Green channel value
		/// </summary>
		public byte Green 
        {
            get { return _green; }
            set
            {
                _green = value;
                GreenAsFloat = ((float)_green) * OneMultiplier;
            }
        }

		/// <summary>
		/// Gets or sets the Blue channel value
		/// </summary>
		public byte Blue 
        {
            get { return _blue; }
            set
            {
                _blue = value;
                BlueAsFloat = ((float)_blue) * OneMultiplier;
            }
        }


        /// <summary>
        /// Gets or sets the Alpha channel value
        /// </summary>
        public byte Alpha
        {
            get { return _alpha; }
            set
            {
                _alpha = value;
                AlphaAsFloat = ((float)_alpha) * OneMultiplier;
            }
        }

        /// <summary>
        /// Gets red as float
        /// </summary>
        public float RedAsFloat { get; private set; }

        /// <summary>
        /// Gets red as float
        /// </summary>
        public float GreenAsFloat { get; private set; }

        /// <summary>
        /// Gets red as float
        /// </summary>
        public float BlueAsFloat { get; private set; }

        /// <summary>
        /// Gets red as float
        /// </summary>
        public float AlphaAsFloat { get; private set; }


		#region Public Static Methods
		/// <summary>
		/// Create a random color
		/// </summary>
		/// <returns>Newly created color</returns>
		public static Color Random()
		{
			var red = (byte)Rnd.Next(0, 64);
			var green = (byte)Rnd.Next(0, 64);
			var blue = (byte)Rnd.Next(0, 64);
			var color = new Color(red, green, blue, 0xff);
			return color;
		}

		/// <summary>
		/// Create a color from given channel values
		/// </summary>
		/// <param name="alpha">Alpha channel value</param>
		/// <param name="red">Red channel value</param>
		/// <param name="green">Green channel value</param>
		/// <param name="blue">Blue channel value</param>
		/// <returns>Newly created color</returns>
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

		public static Color FromInt(int colorAsInt)
		{
			var color = new Color();
			color.Red = (byte)((colorAsInt & RedMask) >> 16);
			color.Green = (byte)((colorAsInt & GreenMask) >> 8);
			color.Blue = (byte)((colorAsInt & BlueMask));
			color.Alpha = (byte)((colorAsInt & AlphaMask) >> 24);
			return color;
		}

#if(!IOS)
		/// <summary>
		/// Create a color from an existing <see cref="SysColor"/>
		/// </summary>
		/// <param name="systemColor"></param>
		/// <returns></returns>
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
#endif
		#endregion

		#region Public Methods

#if(!IOS)
		public SysColor ToSystemColor()
		{
			var sysColor = SysColor.FromArgb(Alpha, Red, Green, Blue);
			return sysColor;
		}
#endif

		public UInt32 ToUInt32()
		{
			var uint32Color = (((UInt32)Alpha) << 24) |
								(((UInt32)Red) << 16) |
								(((UInt32)Green) << 8) |
								(UInt32)Blue;
			return uint32Color;
		}

		public int ToInt()
		{
			return (int)ToUInt32();
		}

		/// <summary>
		/// Adds a color with this color and returns a new color
		/// </summary>
		/// <param name="secondColor">The color to add with this color</param>
		/// <returns>A new color with the two colors added together</returns>
		public Color Additive(Color secondColor)
		{
			var red = (int)Red + (int)secondColor.Red;
			var green = (int)Green + (int)secondColor.Green;
			var blue = (int)Blue + (int)secondColor.Blue;
			var alpha = (int)Alpha + (int)secondColor.Alpha;

			var result = new Color
			{
				Red = (byte)(red > 0xff ? 0xff : red),
				Green = (byte)(green > 0xff ? 0xff : green),
				Blue = (byte)(blue > 0xff ? 0xff : blue),
				Alpha = (byte)(alpha > 0xff ? 0xff : alpha),
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

		public static Color Scale(Color color, float scale)
		{
			var intScale = (int)(scale * 256f);

			var redScaled = (color.Red * intScale) >> 8;
			var greenScaled = (color.Green * intScale) >> 8;
			var blueScaled = (color.Blue * intScale) >> 8;
			var alphaScaled = (color.Alpha * intScale) >> 8;

			var newColor = new Color(
					(byte)(redScaled < 0 ? 0 : redScaled > 0xff ? 0xff : redScaled),
					(byte)(greenScaled < 0 ? 0 : greenScaled > 0xff ? 0xff : greenScaled),
					(byte)(blueScaled < 0 ? 0 : blueScaled > 0xff ? 0xff : blueScaled),
					(byte)(alphaScaled < 0 ? 0 : alphaScaled > 0xff ? 0xff : alphaScaled)
				);

			return newColor;
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
		/// <summary>
		/// Add colors - look at <seealso cref="Additive"/> for more details on the operation
		/// </summary>
		/// <param name="firstColor">First color in addition</param>
		/// <param name="secondColor">Second color in addition</param>
		/// <returns>Combined color</returns>
		public static Color operator +(Color firstColor, Color secondColor)
		{
			var result = firstColor.Additive(secondColor);
			return result;
		}

		/// <summary>
		/// Subtract colors - look at <seealso cref="Subtract"/> for more details on the operation
		/// </summary>
		/// <param name="firstColor">First color in subtraction</param>
		/// <param name="secondColor">Second color in subtraction</param>
		/// <returns>Combined color</returns>
		public static Color operator -(Color firstColor, Color secondColor)
		{
			var newColor = firstColor.Subtract(secondColor);
			return newColor;
		}

		public static Color operator *(Color firstColor, Color secondColor)
		{
			var newColor = new Color(
				(byte)(((int)firstColor.Red * (int)secondColor.Red) >> 8),
				(byte)(((int)firstColor.Green * (int)secondColor.Green) >> 8),
				(byte)(((int)firstColor.Blue * (int)secondColor.Blue) >> 8),
				(byte)(((int)firstColor.Alpha * (int)secondColor.Alpha) >> 8)
				);

			return newColor;
		}

		public static Color operator *(float value, Color color)
		{
			return Scale(color, value);
		}


		public static Color operator *(Color color, float value)
		{
			return Scale(color, value);
		}



		public static int Multiply(int color1, int color2)
		{
			var red2 = (color2 & RedMask) >> 16;
			var red = RedMask & (((color1 & RedMask) >> 16) * red2);

			var green2 = (color2 & GreenMask) >> 8;
			var green = GreenMask & (((color1 & GreenMask) >> 8) * green2);

			var blue2 = (color2 & BlueMask);
			var blue = BlueMask & (((color1 & BlueMask) * blue2) >> 8);

			var alpha2 = 0xff & ((color2 & AlphaMask) >> 24);
			var alpha = AlphaMask & (((color1>>8) & AlphaShiftedMask) * alpha2);

            return red | green | blue | alpha;

		}

		public static int Blend(int color1, int color2)
		{
			var color1Alpha = (((color1) >> 24) & 0xff) + 1;
			var color2Alpha = (((color2) >> 24) & 0xff) + 1;

			var red2 = (((color2 & RedMask) >> 16) * color2Alpha) >> 8;
			var red = RedMask & ((((color1 & RedMask) >> 16) * color1Alpha) * red2);

			var green2 = (((color2 & GreenMask) >> 8) * color2Alpha) >> 8;
			var green = GreenMask & ((((color1 & GreenMask) * color1Alpha) >> 16) * green2);

			var blue2 = ((color2 & BlueMask) * color2Alpha) >> 8;
			var blue = BlueMask & ((((color1 & BlueMask) * color1Alpha) * blue2) >> 16);

			var alpha2 = 0xff & (color2 >> 24);
			var alpha = AlphaMask & (((color1 >> 8) & AlphaShiftedMask) * alpha2);

			return red | green | blue | alpha;
		}



		public static int Blend(int color1, int color2, int factor)
		{
            if (factor == 256) return color1;
            if (factor == 0) return color2;

			var alpha1 = (color1 >> 24) & 0xff;
			var red1 = (color1 >> 16) & 0xff;
			var green1 = (color1 >> 8) & 0xff;
			var blue1 = (color1) & 0xff;

			var alpha2 = (color2 >> 24) & 0xff;
			var red2 = (color2 >> 16) & 0xff;
			var green2 = (color2 >> 8) & 0xff;
			var blue2 = (color2) & 0xff;

			var inverseFactor = 0x100 - factor;
			var alpha = ((alpha1 * factor) + (alpha2 * inverseFactor)) >> 8;
			var red = ((red1 * factor) + (red2 * inverseFactor)) >> 8;
			var green = ((green1 * factor) + (green2 * inverseFactor)) >> 8;
			var blue = ((blue1 * factor) + (blue2 * inverseFactor)) >> 8;

			return alpha << 24 |
			       red << 16 |
			       green << 8 |
			       blue;

		}

		public static int Additive(int color1, int color2)
		{
			var red = (color1 & RedMask) + (color2 & RedMask);
			if (red > RedMask)
				red = RedMask;

			var green = (color1 & GreenMask) + (color2 & GreenMask);
			if (green > GreenMask)
				green = GreenMask;

			var blue = (color1 & BlueMask) + (color2 & BlueMask);
			if (blue > BlueMask)
				blue = BlueMask;

			var alpha = ((color1 >> 8) & AlphaShiftedMask) + ((color2 >> 8) & AlphaShiftedMask);
			if (alpha > (AlphaShiftedMask))
				alpha = AlphaMask;
			else
				alpha <<= 8;

			
			var result = red | green | blue | alpha;
			return result;
		}

		public static int Scale(int color, int scale)
		{
			var red = RedMask & (((color & RedMask) >> 8) * scale);
			if (red > RedMask)
				red = RedMask;
			else if (red <= RedUnderflow)
				red = 0;


			var green = GreenMask & (((color & GreenMask) >> 8) * scale);
			if (green > GreenMask)
				green = GreenMask;
			else if (green <= GreenUnderflow)
				green = 0;


			var blue = BlueMask & (((color & BlueMask) * scale) >> 8);
			if (blue > BlueMask)
				blue = BlueMask;
			else if (blue <= BlueUnderflow)
				blue = 0;

			var alpha = AlphaMask & (((color >> 8) & AlphaShiftedMask) * scale);
			if ((alpha >> 8) > (AlphaShiftedMask))
				alpha = AlphaMask;
			else if ((alpha >> 8) < AlphaShiftedUnderflow)
				alpha = 0;

			return red | green | blue | alpha;
		}


		public static int Scale(int color, float scale)
		{
			var scaleAsInt = (int)(scale * 65536f);

			var red = RedMask & (((color & RedMask) >> 8) * scaleAsInt) >> 8;
			if (red > RedMask) red = RedMask;
			else if (red < RedUnderflow) red = 0;

			var green = GreenMask & (((color & GreenMask) >> 8) * scaleAsInt) >> 8;
			if (green > GreenMask) green = GreenMask;
			else if (green <= GreenUnderflow) green = 0;

			var blue = BlueMask & (((color & BlueMask) * scaleAsInt) >> 16);
			if (blue > BlueMask) blue = BlueMask;
			else if (blue <= BlueUnderflow) blue = 0;

			var alpha = (0xff0000 & (((((color & AlphaMask) >> 8)) * scaleAsInt) >> 8));
			if ((alpha >> 8) > (AlphaShiftedMask)) alpha = AlphaMask;
			else if ((alpha >> 8) < AlphaShiftedUnderflow) alpha = 0;

			return red | green | blue | alpha;
		}

#if(!IOS)
		/// <summary>
		/// Implicitly convert to <see cref="Color"/> from <see cref="SysColor"/>
		/// </summary>
		/// <param name="color"></param>
		/// <returns>Converted color</returns>
		public static implicit operator Color(SysColor color)
		{
			var newColor = FromSystemColor(color);
			return newColor;
		}
#endif

#if(XNA)
        public static implicit operator Microsoft.Xna.Framework.Color(Color color)
        {
            var newColor = new Microsoft.Xna.Framework.Color(color.Blue, color.Green, color.Red, color.Alpha);
            return newColor;
        }
#endif

		#endregion

	}
}
