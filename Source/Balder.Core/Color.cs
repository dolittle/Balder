﻿#if(SILVERLIGHT)
using System;
using SysColor = System.Windows.Media.Color;
#else
using SysColor = System.Drawing.Color;
#endif

namespace Balder.Core
{
	public struct Color : IEquatable<Color>
	{
		public static Color Black = FromArgb(0xff, 0, 0, 0);
		public static Color White = FromArgb(0xff, 0xff, 0xff, 0xff);


		private byte _red;
		public byte Red
		{
			get { return _red; }
			set
			{
				_red = value;
				_redAsFloat = ConvertToFloat(value);
			}
		}

		private byte _green;
		public byte Green
		{
			get { return _green; }
			set
			{
				_green = value;
				_greenAsFloat = ConvertToFloat(value);
			}
		}

		private byte _blue;
		public byte Blue
		{
			get { return _blue; }
			set
			{
				_blue = value;
				_blueAsFloat = ConvertToFloat(value);
			}
		}

		private byte _alpha;
		public byte Alpha
		{
			get { return _alpha; }
			set
			{
				_alpha = value;
				_alphaAsFloat = ConvertToFloat(value);
			}
		}

		private float _redAsFloat;
		public float RedAsFloat
		{
			get { return _redAsFloat; }
			set
			{
				_redAsFloat = value;
				_red = ConvertToByte(value);
			}
		}

		private float _greenAsFloat;
		public float GreenAsFloat
		{
			get { return _greenAsFloat; }
			set
			{
				_greenAsFloat = value;
				_green = ConvertToByte(value);
			}
		}

		private float _blueAsFloat;
		public float BlueAsFloat
		{
			get { return _blueAsFloat; }
			set
			{
				_blueAsFloat = value;
				_blue = ConvertToByte(value);
			}
		}

		private float _alphaAsFloat;
		public float AlphaAsFloat
		{
			get { return _alphaAsFloat; }
			set
			{
				_alphaAsFloat = value;
				_alpha = ConvertToByte(value);
			}
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


		public static explicit operator Color(SysColor systemColor)
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

		public static Color operator +(Color firstColor, Color secondColor)
		{
			var newColor = new Color
			{
				RedAsFloat = firstColor.RedAsFloat + secondColor.RedAsFloat,
				GreenAsFloat = firstColor.GreenAsFloat + secondColor.GreenAsFloat,
				BlueAsFloat = firstColor.BlueAsFloat + secondColor.BlueAsFloat,
				AlphaAsFloat = firstColor.AlphaAsFloat + secondColor.AlphaAsFloat,
			};
			return newColor;
		}

		public static Color operator -(Color firstColor, Color secondColor)
		{
			var newColor = new Color
			{
				RedAsFloat = firstColor.RedAsFloat - secondColor.RedAsFloat,
				GreenAsFloat = firstColor.GreenAsFloat - secondColor.GreenAsFloat,
				BlueAsFloat = firstColor.BlueAsFloat - secondColor.BlueAsFloat,
				AlphaAsFloat = firstColor.AlphaAsFloat - secondColor.AlphaAsFloat,
			};
			return newColor;
		}

		public static Color operator *(Color color, float scale)
		{
			var scaledColor = new Color
								{
									RedAsFloat = color.RedAsFloat * scale,
									GreenAsFloat = color.GreenAsFloat * scale,
									BlueAsFloat = color.BlueAsFloat * scale,
									AlphaAsFloat = color.AlphaAsFloat * scale,
								};
			return scaledColor;
		}

		public static Color operator *(float scale, Color color)
		{
			var scaledColor = color * scale;
			return scaledColor;
		}

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

		public void Clamp()
		{
			RedAsFloat = ClampValue(RedAsFloat);
			GreenAsFloat = ClampValue(GreenAsFloat);
			BlueAsFloat = ClampValue(BlueAsFloat);
			AlphaAsFloat = ClampValue(AlphaAsFloat);
		}


		public bool Equals(Color other)
		{
			if (other.Red == Red &&
				other.Green == Green &&
				other.Blue == Blue &&
				other.Alpha == Alpha)
			{
				return true;
			}
			return false;
		}

		public Color Additive(Color secondColor)
		{
			var result = new Color
							{
								RedAsFloat = (RedAsFloat + secondColor.RedAsFloat) / 2,
								GreenAsFloat = (GreenAsFloat + secondColor.GreenAsFloat) / 2,
								BlueAsFloat = (BlueAsFloat + secondColor.BlueAsFloat) / 2,
								AlphaAsFloat = (AlphaAsFloat + secondColor.AlphaAsFloat) / 2,
							};
			return result;
		}


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

		private static float ConvertToFloat(byte value)
		{
			var valueAsFloat = (float)value;
			var convertedValue = (float)System.Math.Round(valueAsFloat / 255f, 2);
			return convertedValue;
		}

		private static byte ConvertToByte(float value)
		{
			var convertedValue = (byte)(value * 255f);
			return convertedValue;
		}
	}
}
