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
using Balder.Core.Execution;

namespace Balder.Core.Math
{
	public partial class Coordinate : ICloneable, ICopyable
	{
		public Coordinate()
		{
		}

		public Coordinate(Coordinate coordinate)
		{
			Set(coordinate);
		}

		public Coordinate(double x, double y, double z)
		{
			if (x != 0d)
			{
				X = x;
			}

			if (y != 0d)
			{
				Y = y;
			}

			if (z != 0d)
			{
				Z = z;
			}
		}

		public void Set(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public void Set(Coordinate coordinate)
		{
			if (null != coordinate)
			{
				X = coordinate.X;
				Y = coordinate.Y;
				Z = coordinate.Z;
			}
		}



		public Vector ToVector()
		{
			var vector = new Vector((float)X, (float)Y, (float)Z);
			return vector;
		}

		public static implicit operator Coordinate(Vector vector)
		{
			var coordinate = new Coordinate { X = vector.X, Y = vector.Y, Z = vector.Z };
			return coordinate;
		}

		public static implicit operator Vector(Coordinate coordinate)
		{
			var vector = coordinate.ToVector();
			return vector;
		}

		public static Vector operator +(Coordinate c1, Vector v2)
		{
			Vector v1 = c1;
			return v1 + v2;
		}

		public static Vector operator +(Vector v1, Coordinate c2)
		{
			Vector v2 = c2;
			return v1 + v2;
		}


		public static Vector operator +(Coordinate c1, Coordinate c2)
		{
			Vector v1 = c1;
			Vector v2 = c2;
			return v1 + v2;
		}

		public static Vector operator -(Coordinate c1, Vector v2)
		{
			Vector v1 = c1;
			return v1 - v2;
		}

		public static Vector operator -(Vector v1, Coordinate c2)
		{
			Vector v2 = c2;
			return v1 - v2;
		}

		public static Vector operator -(Coordinate c1, Coordinate c2)
		{
			Vector v1 = c1;
			Vector v2 = c2;
			return v1 - v2;
		}


		public object Clone()
		{
			return new Coordinate(X, Y, Z);
		}

		public object Clone(bool unique)
		{
			return new Coordinate(X, Y, Z);
		}

		public override string ToString()
		{
			return ToString(null, null);
		}


		public string ToString(string format, IFormatProvider formatProvider)
		{
			// If no format is passed
			if (string.IsNullOrEmpty(format))
			{
				return string.Format("({0}, {1}, {2})", X, Y, Z);
			}

			var firstChar = format[0];
			string remainder = null;

			if (format.Length > 1)
			{
				remainder = format.Substring(1);
			}

			switch (firstChar)
			{
				case 'x':
					{
						return X.ToString(remainder, formatProvider);
					}
				case 'y':
					{
						return Y.ToString(remainder, formatProvider);
					}
				case 'z':
					{
						return Z.ToString(remainder, formatProvider);
					}
				default:
					return string.Format
						(
							"({0}, {1}, {2})",
							X.ToString(format, formatProvider),
							Y.ToString(format, formatProvider),
							Z.ToString(format, formatProvider)
						);
			}
		}

		public override bool Equals(object obj)
		{
			var coordinate = obj as Coordinate;
			if (null != coordinate)
			{
				return X == coordinate.X &&
					   Y == coordinate.Y &&
					   Z == coordinate.Z;
			}
			return false;
		}

		public void CopyTo(object destination)
		{
			var coordinate = destination as Coordinate;
			if (null != coordinate)
			{
				coordinate.X = X;
				coordinate.Y = Y;
				coordinate.Z = Z;
			}
		}
	}
}
