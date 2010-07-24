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

namespace Balder.Math
{
	public class Matrix
	{
		public static readonly Matrix ScaleOneMatrix = CreateScale(1f);
		public static readonly Matrix TranslationZeroMatrix = CreateTranslation(0, 0, 0);
		public static readonly Matrix RotationZeroMatrix = CreateRotation(0, 0, 0);

		private float[,] _data;

		public Matrix()
		{
			Initialize();
		}


		private void Initialize()
		{
			_data = new float[4, 4];
			SetIdentity(this);
		}

		public bool IsIdentity { get { return EqualsTo(Identity); } }

		public bool EqualsTo(Matrix matrix)
		{
			for (var i = 0; i < matrix._data.GetLength(0); i++)
			{
				for (var j = 0; j < matrix._data.GetLength(1); j++)
				{
					if( _data[i,j] != matrix._data[i, j] )
					{
						return false;
					}
				}
			}

			return true;
		}
		


		private static void SetIdentity(Matrix matrix)
		{
			for (var i = 0; i < matrix._data.GetLength(0); i++)
			{
				for (var j = 0; j < matrix._data.GetLength(1); j++)
				{
					matrix._data[i, j] = 0;
				}
			}

			for (var i = 0; i < 4; i++)
			{
				matrix._data[i, i] = 1;
			}
		}

		public float this[int i, int j]
		{
			get
			{
				if (null == _data)
				{
					Initialize();
				}
				return _data[i, j];
			}
			set
			{
				if (null == _data)
				{
					Initialize();
				}
				_data[i, j] = value;
			}
		}


		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{
			var matrix = new Matrix();
			for (var i = 0; i < matrix._data.GetLength(0); i++)
			{
				for (var j = 0; j < matrix._data.GetLength(1); j++)
				{
					matrix._data[i,j] = matrix1._data[i,j] + matrix2._data[i,j];
				}
			}

			return matrix;
		}

		// [ ][ ][ ][ ]   [ ][ ][ ][ ]
		// [ ][ ][ ][ ] x [ ][ ][ ][ ]
		// [ ][ ][ ][ ]   [ ][ ][ ][ ]
		// [ ][ ][ ][ ]   [ ][ ][ ][ ]
		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{
			var matrix = Identity;
			matrix._data[0, 0] = (((matrix1._data[0, 0] * matrix2._data[0, 0]) + (matrix1._data[0, 1] * matrix2._data[1, 0])) + (matrix1._data[0, 2] * matrix2._data[2, 0])) + (matrix1._data[0, 3] * matrix2._data[3, 0]);
			matrix._data[0, 1] = (((matrix1._data[0, 0] * matrix2._data[0, 1]) + (matrix1._data[0, 1] * matrix2._data[1, 1])) + (matrix1._data[0, 2] * matrix2._data[2, 1])) + (matrix1._data[0, 3] * matrix2._data[3, 1]);
			matrix._data[0, 2] = (((matrix1._data[0, 0] * matrix2._data[0, 2]) + (matrix1._data[0, 1] * matrix2._data[1, 2])) + (matrix1._data[0, 2] * matrix2._data[2, 2])) + (matrix1._data[0, 3] * matrix2._data[3, 2]);
			matrix._data[0, 3] = (((matrix1._data[0, 0] * matrix2._data[0, 3]) + (matrix1._data[0, 1] * matrix2._data[1, 3])) + (matrix1._data[0, 2] * matrix2._data[2, 3])) + (matrix1._data[0, 3] * matrix2._data[3, 3]);
			matrix._data[1, 0] = (((matrix1._data[1, 0] * matrix2._data[0, 0]) + (matrix1._data[1, 1] * matrix2._data[1, 0])) + (matrix1._data[1, 2] * matrix2._data[2, 0])) + (matrix1._data[1, 3] * matrix2._data[3, 0]);
			matrix._data[1, 1] = (((matrix1._data[1, 0] * matrix2._data[0, 1]) + (matrix1._data[1, 1] * matrix2._data[1, 1])) + (matrix1._data[1, 2] * matrix2._data[2, 1])) + (matrix1._data[1, 3] * matrix2._data[3, 1]);
			matrix._data[1, 2] = (((matrix1._data[1, 0] * matrix2._data[0, 2]) + (matrix1._data[1, 1] * matrix2._data[1, 2])) + (matrix1._data[1, 2] * matrix2._data[2, 2])) + (matrix1._data[1, 3] * matrix2._data[3, 2]);
			matrix._data[1, 3] = (((matrix1._data[1, 0] * matrix2._data[0, 3]) + (matrix1._data[1, 1] * matrix2._data[1, 3])) + (matrix1._data[1, 2] * matrix2._data[2, 3])) + (matrix1._data[1, 3] * matrix2._data[3, 3]);
			matrix._data[2, 0] = (((matrix1._data[2, 0] * matrix2._data[0, 0]) + (matrix1._data[2, 1] * matrix2._data[1, 0])) + (matrix1._data[2, 2] * matrix2._data[2, 0])) + (matrix1._data[2, 3] * matrix2._data[3, 0]);
			matrix._data[2, 1] = (((matrix1._data[2, 0] * matrix2._data[0, 1]) + (matrix1._data[2, 1] * matrix2._data[1, 1])) + (matrix1._data[2, 2] * matrix2._data[2, 1])) + (matrix1._data[2, 3] * matrix2._data[3, 1]);
			matrix._data[2, 2] = (((matrix1._data[2, 0] * matrix2._data[0, 2]) + (matrix1._data[2, 1] * matrix2._data[1, 2])) + (matrix1._data[2, 2] * matrix2._data[2, 2])) + (matrix1._data[2, 3] * matrix2._data[3, 2]);
			matrix._data[2, 3] = (((matrix1._data[2, 0] * matrix2._data[0, 3]) + (matrix1._data[2, 1] * matrix2._data[1, 3])) + (matrix1._data[2, 2] * matrix2._data[2, 3])) + (matrix1._data[2, 3] * matrix2._data[3, 3]);
			matrix._data[3, 0] = (((matrix1._data[3, 0] * matrix2._data[0, 0]) + (matrix1._data[3, 1] * matrix2._data[1, 0])) + (matrix1._data[3, 2] * matrix2._data[2, 0])) + (matrix1._data[3, 3] * matrix2._data[3, 0]);
			matrix._data[3, 1] = (((matrix1._data[3, 0] * matrix2._data[0, 1]) + (matrix1._data[3, 1] * matrix2._data[1, 1])) + (matrix1._data[3, 2] * matrix2._data[2, 1])) + (matrix1._data[3, 3] * matrix2._data[3, 1]);
			matrix._data[3, 2] = (((matrix1._data[3, 0] * matrix2._data[0, 2]) + (matrix1._data[3, 1] * matrix2._data[1, 2])) + (matrix1._data[3, 2] * matrix2._data[2, 2])) + (matrix1._data[3, 3] * matrix2._data[3, 2]);
			matrix._data[3, 3] = (((matrix1._data[3, 0] * matrix2._data[0, 3]) + (matrix1._data[3, 1] * matrix2._data[1, 3])) + (matrix1._data[3, 2] * matrix2._data[2, 3])) + (matrix1._data[3, 3] * matrix2._data[3, 3]);
			return matrix;
		}


		public static Vector operator *(Vector vector, Matrix matrix)
		{
			var returnVector = Vector.Transform(vector, matrix);
			return returnVector;
		}

		public static explicit operator Vector(Matrix matrix)
		{
			return new Vector(matrix._data[3, 0], matrix._data[3, 1], matrix._data[3, 2]);
		}


		public static Matrix Identity
		{
			get
			{
				var identityMatrix = new Matrix();
				SetIdentity(identityMatrix);
				return identityMatrix;
			}
		}

		public static Matrix CreateLookAt(Vector cameraPosition, Vector cameraTarget, Vector cameraUpVector)
		{
			var matrix = Matrix.Identity;
			var direction = cameraTarget - cameraPosition;
			var zaxis = Vector.Normalize(direction);

			var zCross = Vector.Cross(cameraUpVector, zaxis);
			var xaxis = Vector.Normalize(zCross);

			var yaxis = Vector.Cross(zaxis, xaxis);
			matrix._data[0, 0] = xaxis.X;
			matrix._data[0, 1] = yaxis.X;
			matrix._data[0, 2] = zaxis.X;
			matrix._data[0, 3] = 0f;
			matrix._data[1, 0] = xaxis.Y;
			matrix._data[1, 1] = yaxis.Y;
			matrix._data[1, 2] = zaxis.Y;
			matrix._data[1, 3] = 0f;
			matrix._data[2, 0] = xaxis.Z;
			matrix._data[2, 1] = yaxis.Z;
			matrix._data[2, 2] = zaxis.Z;
			matrix._data[2, 3] = 0f;
			matrix._data[3, 0] = -Vector.Dot(xaxis, cameraPosition);
			matrix._data[3, 1] = -Vector.Dot(yaxis, cameraPosition);
			matrix._data[3, 2] = -Vector.Dot(zaxis, cameraPosition);
			matrix._data[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			var matrix = new Matrix();
			var viewAngle = (float)System.Math.Tan((double)(fieldOfView * 0.5f));
			var yscale = 1f / viewAngle;
			var xscale = 1f / aspectRatio * yscale;

			matrix._data[0, 0] = xscale;
			matrix._data[0, 1] = matrix._data[0, 2] = matrix._data[0, 3] = 0f;

			matrix._data[1, 1] = yscale;
			matrix._data[1, 0] = matrix._data[1, 2] = matrix._data[1, 3] = 0f;

			matrix._data[2, 0] = matrix._data[2, 1] = 0f;
			matrix._data[2, 2] = farPlaneDistance / (farPlaneDistance - nearPlaneDistance);
			matrix._data[2, 3] = 1f;

			matrix._data[3, 0] = matrix._data[3, 1] = matrix._data[3, 3] = 0f;
			matrix._data[3, 2] = -nearPlaneDistance * farPlaneDistance / (farPlaneDistance - nearPlaneDistance);

			return matrix;
		}

		public static Matrix CreateScreenTranslation(int width, int height)
		{
			var centerX = ((float)width) / 2f;
			var centerY = ((float)height) / 2f;
			var matrix = new Matrix();
			matrix._data[0, 0] = centerX;
			matrix._data[0, 1] = matrix._data[0, 2] = matrix._data[0, 3] = 0f;

			matrix._data[1, 1] = centerY;
			matrix._data[1, 0] = matrix._data[1, 2] = matrix._data[1, 3] = 0f;

			matrix._data[2, 0] = matrix._data[2, 1] = matrix._data[2, 2] = 0f;
			matrix._data[2, 3] = 1f;

			matrix._data[3, 0] = centerX;
			matrix._data[3, 1] = centerY;
			matrix._data[3, 2] = 0;
			matrix._data[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreateRotationX(float degrees)
		{
			Matrix matrix = new Matrix();

			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));
			matrix._data[0, 0] = 1f;
			matrix._data[0, 1] = 0f;
			matrix._data[0, 2] = 0f;
			matrix._data[0, 3] = 0f;
			matrix._data[1, 0] = 0f;
			matrix._data[1, 1] = num2;
			matrix._data[1, 2] = num;
			matrix._data[1, 3] = 0f;
			matrix._data[2, 0] = 0f;
			matrix._data[2, 1] = -num;
			matrix._data[2, 2] = num2;
			matrix._data[2, 3] = 0f;
			matrix._data[3, 0] = 0f;
			matrix._data[3, 1] = 0f;
			matrix._data[3, 2] = 0f;
			matrix._data[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreateRotationY(float degrees)
		{
			var matrix = new Matrix();
			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));
			matrix._data[0, 0] = num2;
			matrix._data[0, 1] = 0f;
			matrix._data[0, 2] = -num;
			matrix._data[0, 3] = 0f;
			matrix._data[1, 0] = 0f;
			matrix._data[1, 1] = 1f;
			matrix._data[1, 2] = 0f;
			matrix._data[1, 3] = 0f;
			matrix._data[2, 0] = num;
			matrix._data[2, 1] = 0f;
			matrix._data[2, 2] = num2;
			matrix._data[2, 3] = 0f;
			matrix._data[3, 0] = 0f;
			matrix._data[3, 1] = 0f;
			matrix._data[3, 2] = 0f;
			matrix._data[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreateRotationZ(float degrees)
		{
			var matrix = new Matrix();
			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));

			matrix._data[0, 0] = num2;
			matrix._data[0, 1] = num;
			matrix._data[0, 2] = 0f;
			matrix._data[0, 3] = 0f;
			matrix._data[1, 0] = -num;
			matrix._data[1, 1] = num2;
			matrix._data[1, 2] = 0f;
			matrix._data[1, 3] = 0f;
			matrix._data[2, 0] = 0f;
			matrix._data[2, 1] = 0f;
			matrix._data[2, 2] = 1f;
			matrix._data[2, 3] = 0f;
			matrix._data[3, 0] = 0f;
			matrix._data[3, 1] = 0f;
			matrix._data[3, 2] = 0f;
			matrix._data[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreateRotation(float xDegrees, float yDegrees, float zDegrees)
		{
			var xRotation = CreateRotationX(xDegrees);
			var yRotation = CreateRotationY(yDegrees);
			var zRotation = CreateRotationZ(zDegrees);

			var matrix = xRotation * yRotation * zRotation;

			return matrix;

		}

		public static Matrix CreateTranslation(Vector position)
		{
			var matrix = CreateTranslation(position.X, position.Y, position.Z);
			return matrix;
		}

		public static Matrix CreateTranslation(float x, float y, float z)
		{
			var matrix = new Matrix();
			matrix._data[0, 0] = 1f;
			matrix._data[0, 1] = 0f;
			matrix._data[0, 2] = 0f;
			matrix._data[0, 3] = 0f;
			matrix._data[1, 0] = 0f;
			matrix._data[1, 1] = 1f;
			matrix._data[1, 2] = 0f;
			matrix._data[1, 3] = 0f;
			matrix._data[2, 0] = 0f;
			matrix._data[2, 1] = 0f;
			matrix._data[2, 2] = 1f;
			matrix._data[2, 3] = 0f;
			matrix._data[3, 0] = x;
			matrix._data[3, 1] = y;
			matrix._data[3, 2] = z;
			matrix._data[3, 3] = 1f;
			return matrix;
		}


		public static Matrix CreateOrthographic(float width, float height, float nearPlane, float farPlane)
		{
			var matrix = new Matrix();
			matrix._data[0, 0] = 2f/width;
			matrix._data[1, 1] = 2f/height;
			matrix._data[2, 2] = 1f/(nearPlane - farPlane);
			matrix._data[3, 2] = nearPlane/(nearPlane - farPlane);
			matrix._data[3, 3] = 1f;
			return matrix;
		}

		public static Matrix CreateScale(float scale)
		{
			return CreateScale(new Vector(scale, scale, scale));
		}

		public static Matrix CreateScale(Vector scales)
		{
			var matrix = Matrix.Identity;
			var x = scales.X;
			var y = scales.Y;
			var z = scales.Z;
			matrix._data[0, 0] = x;
			matrix._data[0, 1] = 0f;
			matrix._data[0, 2] = 0f;
			matrix._data[0, 3] = 0f;
			matrix._data[1, 0] = 0f;
			matrix._data[1, 1] = y;
			matrix._data[1, 2] = 0f;
			matrix._data[1, 3] = 0f;
			matrix._data[2, 0] = 0f;
			matrix._data[2, 1] = 0f;
			matrix._data[2, 2] = z;
			matrix._data[2, 3] = 0f;
			matrix._data[3, 0] = 0f;
			matrix._data[3, 1] = 0f;
			matrix._data[3, 2] = 0f;
			matrix._data[3, 3] = 1f;
			return matrix;
		}

		public static Matrix Invert(Matrix matrix)
		{
			var matrix2 = new Matrix();
			var num5 = matrix._data[0, 0];
			var num4 = matrix._data[0, 1];
			var num3 = matrix._data[0, 2];
			var num2 = matrix._data[0, 3];
			var num9 = matrix._data[1, 0];
			var num8 = matrix._data[1, 1];
			var num7 = matrix._data[1, 2];
			var num6 = matrix._data[1, 3];
			var num17 = matrix._data[2, 0];
			var num16 = matrix._data[2, 1];
			var num15 = matrix._data[2, 2];
			var num14 = matrix._data[2, 3];
			var num13 = matrix._data[3, 0];
			var num12 = matrix._data[3, 1];
			var num11 = matrix._data[3, 2];
			var num10 = matrix._data[3, 3];
			var num23 = (num15 * num10) - (num14 * num11);
			var num22 = (num16 * num10) - (num14 * num12);
			var num21 = (num16 * num11) - (num15 * num12);
			var num20 = (num17 * num10) - (num14 * num13);
			var num19 = (num17 * num11) - (num15 * num13);
			var num18 = (num17 * num12) - (num16 * num13);
			var num39 = ((num8 * num23) - (num7 * num22)) + (num6 * num21);
			var num38 = -(((num9 * num23) - (num7 * num20)) + (num6 * num19));
			var num37 = ((num9 * num22) - (num8 * num20)) + (num6 * num18);
			var num36 = -(((num9 * num21) - (num8 * num19)) + (num7 * num18));
			var num = 1f / ((((num5 * num39) + (num4 * num38)) + (num3 * num37)) + (num2 * num36));
			matrix2._data[0, 0] = num39 * num;
			matrix2._data[1, 0] = num38 * num;
			matrix2._data[2, 0] = num37 * num;
			matrix2._data[3, 0] = num36 * num;
			matrix2._data[0, 1] = -(((num4 * num23) - (num3 * num22)) + (num2 * num21)) * num;
			matrix2._data[1, 1] = (((num5 * num23) - (num3 * num20)) + (num2 * num19)) * num;
			matrix2._data[2, 1] = -(((num5 * num22) - (num4 * num20)) + (num2 * num18)) * num;
			matrix2._data[3, 1] = (((num5 * num21) - (num4 * num19)) + (num3 * num18)) * num;
			var num35 = (num7 * num10) - (num6 * num11);
			var num34 = (num8 * num10) - (num6 * num12);
			var num33 = (num8 * num11) - (num7 * num12);
			var num32 = (num9 * num10) - (num6 * num13);
			var num31 = (num9 * num11) - (num7 * num13);
			var num30 = (num9 * num12) - (num8 * num13);
			matrix2._data[0, 2] = (((num4 * num35) - (num3 * num34)) + (num2 * num33)) * num;
			matrix2._data[1, 2] = -(((num5 * num35) - (num3 * num32)) + (num2 * num31)) * num;
			matrix2._data[2, 2] = (((num5 * num34) - (num4 * num32)) + (num2 * num30)) * num;
			matrix2._data[3, 2] = -(((num5 * num33) - (num4 * num31)) + (num3 * num30)) * num;
			var num29 = (num7 * num14) - (num6 * num15);
			var num28 = (num8 * num14) - (num6 * num16);
			var num27 = (num8 * num15) - (num7 * num16);
			var num26 = (num9 * num14) - (num6 * num17);
			var num25 = (num9 * num15) - (num7 * num17);
			var num24 = (num9 * num16) - (num8 * num17);
			matrix2._data[0, 3] = -(((num4 * num29) - (num3 * num28)) + (num2 * num27)) * num;
			matrix2._data[1, 3] = (((num5 * num29) - (num3 * num26)) + (num2 * num25)) * num;
			matrix2._data[2, 3] = -(((num5 * num28) - (num4 * num26)) + (num2 * num24)) * num;
			matrix2._data[3, 3] = (((num5 * num27) - (num4 * num25)) + (num3 * num24)) * num;
			return matrix2;
		}


		public override string ToString()
		{
			var format = "[ {0:##.##} - {1:##.##} - {2:##.##} - {3:##.##} ])";
			var row1 = string.Format(format,
										this[0, 0], this[0, 1], this[0, 2], this[0, 3]
				);
			var row2 = string.Format(format,
										this[1, 0], this[1, 1], this[1, 2], this[1, 3]
				);
			var row3 = string.Format(format,
										this[2, 0], this[2, 1], this[2, 2], this[2, 3]
				);
			var row4 = string.Format(format,
										this[3, 0], this[3, 1], this[3, 2], this[3, 3]
				);

			return string.Format("{0}\n{1}\n{2}\n{3}\n",
								 row1,
								 row2,
								 row3,
								 row4);
		}

#if(XNA)
        public static implicit operator Microsoft.Xna.Framework.Matrix(Matrix matrix)
        {
            var outMatrix = new Microsoft.Xna.Framework.Matrix(
                matrix._data[0, 0], matrix._data[0, 1], matrix._data[0, 2], matrix._data[0, 3], 
                matrix._data[1, 0], matrix._data[1, 1], matrix._data[1, 2], matrix._data[1, 3], 
                matrix._data[2, 0], matrix._data[2, 1], matrix._data[2, 2], matrix._data[2, 3], 
                matrix._data[3, 0], matrix._data[3, 1], matrix._data[3, 2], matrix._data[3, 3]
            );
            return outMatrix;
        }
#endif
	}
}
