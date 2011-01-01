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

namespace Balder.Math
{
	public static class MathHelper
	{
		public const float E = 2.718282f;
		public const float Log10E = 0.4342945f;
		public const float Log2E = 1.442695f;
		public const float Pi = 3.141593f;
		public const float PiOver2 = 1.570796f;
		public const float PiOver4 = 0.7853982f;
		public const float TwoPi = 6.283185f;

		
		public static float Barycentric(float planeValue1, float planeValue2, float planeValue3, float u, float v)
		{
			return ((planeValue1 + (u * (planeValue2 - planeValue1))) + (v * (planeValue3 - planeValue1)));
		}

		public static Vector BarycentricToCartesian(Vector vector1, Vector vector2, Vector vector3, float u, float v)
		{
			var w = 1 - (u + v);
			var cartesian = new Vector(
				(u*vector1.X + v*vector2.X + w*vector3.X),
				(u*vector1.Y + v*vector2.Y + w*vector3.Y),
				(u*vector1.Z + v*vector2.Z + w*vector3.Z));

			return cartesian;
		}

		public static void InterpolateBarycentric(float u0, float v0, float u1, float v1, float u2, float v2, float barycentricU, float barycentricV, out float u, out float v)
		{
			var w = 1 - (barycentricU + barycentricV);

			u = w * u0 + barycentricU * u1 + barycentricV * u2;
			v = w * v0 + barycentricU * v1 + barycentricV * v2;
		}

		public static float CatmullRom(float value1, float value2, float value3, float value4, float amount)
		{
			var num = amount * amount;
			var num2 = amount * num;
			return (0.5f * ((((2f * value2) + ((-value1 + value3) * amount)) + (((((2f * value1) - (5f * value2)) + (4f * value3)) - value4) * num)) + ((((-value1 + (3f * value2)) - (3f * value3)) + value4) * num2)));
		}

		public static float Clamp(float value, float min, float max)
		{
			value = (value > max) ? max : value;
			value = (value < min) ? min : value;
			return value;
		}

		public static float Distance(float value1, float value2)
		{
			return System.Math.Abs(value1 - value2);
		}

		public static float Abs(float value)
		{
			return System.Math.Abs(value);
		}

		public static float Hermite(float value1, float tangent1, float value2, float tangent2, float amount)
		{
			var num3 = amount;
			var num = num3 * num3;
			var num2 = num3 * num;
			var num7 = ((2f * num2) - (3f * num)) + 1f;
			var num6 = (-2f * num2) + (3f * num);
			var num5 = (num2 - (2f * num)) + num3;
			var num4 = num2 - num;
			return ((((value1 * num7) + (value2 * num6)) + (tangent1 * num5)) + (tangent2 * num4));
		}

		public static float Lerp(float value1, float value2, float amount)
		{
			return (value1 + ((value2 - value1) * amount));
		}

		public static float Max(float value1, float value2)
		{
			return System.Math.Max(value1, value2);
		}

		public static float Min(float value1, float value2)
		{
			return System.Math.Min(value1, value2);
		}

		public static float SmoothStep(float value1, float value2, float amount)
		{
			var num = Clamp(amount, 0f, 1f);
			return Lerp(value1, value2, (num * num) * (3f - (2f * num)));
		}

		public static float ToDegrees(float radians)
		{
			return (radians * 57.29578f);
		}


		/// <summary>
		/// Saturates a value to be between 0 and 1.
		/// 
		/// Any values below 0 will be 0, any value above 1 will be 1.
		/// </summary>
		/// <param name="value">Value to saturate</param>
		/// <returns>Saturated value</returns>
		public static float Saturate(float value)
		{
			if (value > 1)
			{
				value = 1;
			}
			if (value < 0)
			{
				value = 0;
			}

			return value;
		}

		public static float ToRadians(float degrees)
		{
			return (degrees * 0.01745329f);
		}



		public static float Sqrt(float value)
		{
			return (float)System.Math.Sqrt(value);
		}
	}
}
