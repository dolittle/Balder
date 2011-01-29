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
	public class Matrix
	{
		public static readonly Matrix ScaleOneMatrix = CreateScale(1f);
		public static readonly Matrix TranslationZeroMatrix = CreateTranslation(0, 0, 0);
		public static readonly Matrix RotationZeroMatrix = CreateRotation(0, 0, 0);

		public float M11;
		public float M12;
		public float M13;
		public float M14;

		public float M21;
		public float M22;
		public float M23;
		public float M24;

		public float M31;
		public float M32;
		public float M33;
		public float M34;

		public float M41;
		public float M42;
		public float M43;
		public float M44;


		public Matrix()
		{
			SetIdentity(this);
		}


		public bool IsIdentity { get { return EqualsTo(Identity); } }

		public bool EqualsTo(Matrix matrix)
		{
			var isEqual =
				matrix.M11 == M11 && matrix.M12 == M12 && matrix.M13 == M13 && matrix.M14 == M14 &&
				matrix.M21 == M21 && matrix.M22 == M22 && matrix.M23 == M23 && matrix.M24 == M24 &&
				matrix.M31 == M31 && matrix.M32 == M32 && matrix.M33 == M33 && matrix.M34 == M34 &&
				matrix.M41 == M41 && matrix.M42 == M42 && matrix.M43 == M43 && matrix.M44 == M44;
			return isEqual;
		}

		public Matrix Clone()
		{
			var newMatrix = new Matrix();

			newMatrix.M11 = M11;
			newMatrix.M12 = M12;
			newMatrix.M13 = M13;
			newMatrix.M14 = M14;

			newMatrix.M21 = M21;
			newMatrix.M22 = M22;
			newMatrix.M23 = M23;
			newMatrix.M24 = M24;

			newMatrix.M31 = M31;
			newMatrix.M32 = M32;
			newMatrix.M33 = M33;
			newMatrix.M34 = M34;

			newMatrix.M41 = M41;
			newMatrix.M42 = M42;
			newMatrix.M43 = M43;
			newMatrix.M44 = M44;

			return newMatrix;

		}

		public void SetTranslation(float x, float y, float z)
		{
			M41 = x;
			M42 = y;
			M43 = z;
		}


		private static void SetIdentity(Matrix matrix)
		{
			matrix.M11 = 1f;
			matrix.M12 = matrix.M13 = matrix.M14 = 0;

			matrix.M22 = 1f;
			matrix.M21 = matrix.M23 = matrix.M24 = 0;

			matrix.M33 = 1f;
			matrix.M31 = matrix.M32 = matrix.M34 = 0;

			matrix.M44 = 1f;
			matrix.M41 = matrix.M42 = matrix.M43 = 0;
		}


		public static Matrix operator +(Matrix matrix1, Matrix matrix2)
		{
			var matrix = new Matrix();
			matrix.M11 = matrix1.M11 + matrix2.M11;
			matrix.M12 = matrix1.M12 + matrix2.M12;
			matrix.M13 = matrix1.M13 + matrix2.M13;
			matrix.M14 = matrix1.M14 + matrix2.M14;

			matrix.M21 = matrix1.M21 + matrix2.M21;
			matrix.M22 = matrix1.M22 + matrix2.M22;
			matrix.M23 = matrix1.M23 + matrix2.M23;
			matrix.M24 = matrix1.M24 + matrix2.M24;

			matrix.M31 = matrix1.M31 + matrix2.M31;
			matrix.M32 = matrix1.M32 + matrix2.M32;
			matrix.M33 = matrix1.M33 + matrix2.M33;
			matrix.M34 = matrix1.M34 + matrix2.M34;

			matrix.M41 = matrix1.M41 + matrix2.M41;
			matrix.M42 = matrix1.M42 + matrix2.M42;
			matrix.M43 = matrix1.M43 + matrix2.M43;
			matrix.M44 = matrix1.M44 + matrix2.M44;

			return matrix;
		}

		// [ ][ ][ ][ ]   [ ][ ][ ][ ]
		// [ ][ ][ ][ ] x [ ][ ][ ][ ]
		// [ ][ ][ ][ ]   [ ][ ][ ][ ]
		// [ ][ ][ ][ ]   [ ][ ][ ][ ]
		public static Matrix operator *(Matrix matrix1, Matrix matrix2)
		{
			var matrix = Identity;
			matrix.M11 = (((matrix1.M11 * matrix2.M11) + (matrix1.M12 * matrix2.M21)) + (matrix1.M13 * matrix2.M31)) + (matrix1.M14 * matrix2.M41);
			matrix.M12 = (((matrix1.M11 * matrix2.M12) + (matrix1.M12 * matrix2.M22)) + (matrix1.M13 * matrix2.M32)) + (matrix1.M14 * matrix2.M42);
			matrix.M13 = (((matrix1.M11 * matrix2.M13) + (matrix1.M12 * matrix2.M23)) + (matrix1.M13 * matrix2.M33)) + (matrix1.M14 * matrix2.M43);
			matrix.M14 = (((matrix1.M11 * matrix2.M14) + (matrix1.M12 * matrix2.M24)) + (matrix1.M13 * matrix2.M34)) + (matrix1.M14 * matrix2.M44);
			matrix.M21 = (((matrix1.M21 * matrix2.M11) + (matrix1.M22 * matrix2.M21)) + (matrix1.M23 * matrix2.M31)) + (matrix1.M24 * matrix2.M41);
			matrix.M22 = (((matrix1.M21 * matrix2.M12) + (matrix1.M22 * matrix2.M22)) + (matrix1.M23 * matrix2.M32)) + (matrix1.M24 * matrix2.M42);
			matrix.M23 = (((matrix1.M21 * matrix2.M13) + (matrix1.M22 * matrix2.M23)) + (matrix1.M23 * matrix2.M33)) + (matrix1.M24 * matrix2.M43);
			matrix.M24 = (((matrix1.M21 * matrix2.M14) + (matrix1.M22 * matrix2.M24)) + (matrix1.M23 * matrix2.M34)) + (matrix1.M24 * matrix2.M44);
			matrix.M31 = (((matrix1.M31 * matrix2.M11) + (matrix1.M32 * matrix2.M21)) + (matrix1.M33 * matrix2.M31)) + (matrix1.M34 * matrix2.M41);
			matrix.M32 = (((matrix1.M31 * matrix2.M12) + (matrix1.M32 * matrix2.M22)) + (matrix1.M33 * matrix2.M32)) + (matrix1.M34 * matrix2.M42);
			matrix.M33 = (((matrix1.M31 * matrix2.M13) + (matrix1.M32 * matrix2.M23)) + (matrix1.M33 * matrix2.M33)) + (matrix1.M34 * matrix2.M43);
			matrix.M34 = (((matrix1.M31 * matrix2.M14) + (matrix1.M32 * matrix2.M24)) + (matrix1.M33 * matrix2.M34)) + (matrix1.M34 * matrix2.M44);
			matrix.M41 = (((matrix1.M41 * matrix2.M11) + (matrix1.M42 * matrix2.M21)) + (matrix1.M43 * matrix2.M31)) + (matrix1.M44 * matrix2.M41);
			matrix.M42 = (((matrix1.M41 * matrix2.M12) + (matrix1.M42 * matrix2.M22)) + (matrix1.M43 * matrix2.M32)) + (matrix1.M44 * matrix2.M42);
			matrix.M43 = (((matrix1.M41 * matrix2.M13) + (matrix1.M42 * matrix2.M23)) + (matrix1.M43 * matrix2.M33)) + (matrix1.M44 * matrix2.M43);
			matrix.M44 = (((matrix1.M41 * matrix2.M14) + (matrix1.M42 * matrix2.M24)) + (matrix1.M43 * matrix2.M34)) + (matrix1.M44 * matrix2.M44);
			return matrix;
		}


		public static Vector operator *(Vector vector, Matrix matrix)
		{
			var returnVector = Vector.Transform(vector, matrix);
			return returnVector;
		}

		public static explicit operator Vector(Matrix matrix)
		{
			return new Vector(matrix.M41, matrix.M42, matrix.M43);
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
			matrix.M11 = xaxis.X;
			matrix.M12 = yaxis.X;
			matrix.M13 = zaxis.X;
			matrix.M14 = 0f;
			matrix.M21 = xaxis.Y;
			matrix.M22 = yaxis.Y;
			matrix.M23 = zaxis.Y;
			matrix.M24 = 0f;
			matrix.M31 = xaxis.Z;
			matrix.M32 = yaxis.Z;
			matrix.M33 = zaxis.Z;
			matrix.M34 = 0f;
			matrix.M41 = -Vector.Dot(xaxis, cameraPosition);
			matrix.M42 = -Vector.Dot(yaxis, cameraPosition);
			matrix.M43 = -Vector.Dot(zaxis, cameraPosition);
			matrix.M44 = 1f;
			return matrix;
		}


		public static Matrix CreateScreenTranslation(int width, int height)
		{
			var centerX = ((float)width) * 0.5f;
			var centerY = ((float)height) * 0.5f;
			var matrix = new Matrix();
			matrix.M11 = centerX;
			matrix.M12 = matrix.M13 = matrix.M14 = 0f;

			matrix.M22 = -centerY;
			matrix.M21 = matrix.M23 = matrix.M24 = 0f;

			matrix.M31 = matrix.M32 = matrix.M33 = 0f;
			matrix.M34 = 1f;

			matrix.M41 = centerX;
			matrix.M42 = centerY;
			matrix.M43 = 0;
			matrix.M44 = 1f;
			return matrix;
		}

		public static Matrix CreateRotationX(float degrees)
		{
			Matrix matrix = new Matrix();

			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));
			matrix.M11 = 1f;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = num2;
			matrix.M23 = num;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = -num;
			matrix.M33 = num2;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		public static Matrix CreateRotationY(float degrees)
		{
			var matrix = new Matrix();
			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));
			matrix.M11 = num2;
			matrix.M12 = 0f;
			matrix.M13 = -num;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = 1f;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = num;
			matrix.M32 = 0f;
			matrix.M33 = num2;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		public static Matrix CreateRotationZ(float degrees)
		{
			var matrix = new Matrix();
			var num2 = (float)System.Math.Cos(MathHelper.ToRadians(degrees));
			var num = (float)System.Math.Sin(MathHelper.ToRadians(degrees));

			matrix.M11 = num2;
			matrix.M12 = num;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = -num;
			matrix.M22 = num2;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = 1f;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
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
			matrix.M11 = 1f;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = 1f;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = 1f;
			matrix.M34 = 0f;
			matrix.M41 = x;
			matrix.M42 = y;
			matrix.M43 = z;
			matrix.M44 = 1f;
			return matrix;
		}

		public static Matrix CreatePerspectiveFieldOfView(float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance)
		{
			var matrix = new Matrix();
			var viewAngle = (float)System.Math.Tan((double)(fieldOfView * 0.5f));
			var yscale = 1f / viewAngle;
			var xscale = 1f / aspectRatio * yscale;

			matrix.M11 = xscale;
			matrix.M12 = matrix.M13 = matrix.M14 = 0f;

			matrix.M22 = yscale;
			matrix.M21 = matrix.M23 = matrix.M24 = 0f;

			matrix.M31 = matrix.M32 = 0f;
			matrix.M33 = farPlaneDistance / (farPlaneDistance - nearPlaneDistance);
			matrix.M34 = 1f;

			matrix.M41 = matrix.M42 = matrix.M44 = 0f;
			matrix.M43 = -nearPlaneDistance * farPlaneDistance / (farPlaneDistance - nearPlaneDistance);

			return matrix;
		}

		public static Matrix CreateOrthographic(float width, float height, float nearPlane, float farPlane)
		{
			var matrix = new Matrix
			             	{
			             		M11 = 2f/width,
			             		M22 = 2f/height,
			             		M33 = 1f/(farPlane - nearPlane),
			             		M43 = -nearPlane/(farPlane - nearPlane),
			             		M44 = 1f
			             	};
			return matrix;
		}

		public static Matrix CreateScale(float scale)
		{
			return CreateScale(new Vector(scale, scale, scale));
		}

		public static Matrix CreateScale(Vector scales)
		{
			var matrix = Identity;
			var x = scales.X;
			var y = scales.Y;
			var z = scales.Z;
			matrix.M11 = x;
			matrix.M12 = 0f;
			matrix.M13 = 0f;
			matrix.M14 = 0f;
			matrix.M21 = 0f;
			matrix.M22 = y;
			matrix.M23 = 0f;
			matrix.M24 = 0f;
			matrix.M31 = 0f;
			matrix.M32 = 0f;
			matrix.M33 = z;
			matrix.M34 = 0f;
			matrix.M41 = 0f;
			matrix.M42 = 0f;
			matrix.M43 = 0f;
			matrix.M44 = 1f;
			return matrix;
		}

		public static Matrix Invert(Matrix matrix)
		{
			var matrix2 = new Matrix();
			var num5 = matrix.M11;
			var num4 = matrix.M12;
			var num3 = matrix.M13;
			var num2 = matrix.M14;
			var num9 = matrix.M21;
			var num8 = matrix.M22;
			var num7 = matrix.M23;
			var num6 = matrix.M24;
			var num17 = matrix.M31;
			var num16 = matrix.M32;
			var num15 = matrix.M33;
			var num14 = matrix.M34;
			var num13 = matrix.M41;
			var num12 = matrix.M42;
			var num11 = matrix.M43;
			var num10 = matrix.M44;
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
			matrix2.M11 = num39 * num;
			matrix2.M21 = num38 * num;
			matrix2.M31 = num37 * num;
			matrix2.M41 = num36 * num;
			matrix2.M12 = -(((num4 * num23) - (num3 * num22)) + (num2 * num21)) * num;
			matrix2.M22 = (((num5 * num23) - (num3 * num20)) + (num2 * num19)) * num;
			matrix2.M32 = -(((num5 * num22) - (num4 * num20)) + (num2 * num18)) * num;
			matrix2.M42 = (((num5 * num21) - (num4 * num19)) + (num3 * num18)) * num;
			var num35 = (num7 * num10) - (num6 * num11);
			var num34 = (num8 * num10) - (num6 * num12);
			var num33 = (num8 * num11) - (num7 * num12);
			var num32 = (num9 * num10) - (num6 * num13);
			var num31 = (num9 * num11) - (num7 * num13);
			var num30 = (num9 * num12) - (num8 * num13);
			matrix2.M13 = (((num4 * num35) - (num3 * num34)) + (num2 * num33)) * num;
			matrix2.M23 = -(((num5 * num35) - (num3 * num32)) + (num2 * num31)) * num;
			matrix2.M33 = (((num5 * num34) - (num4 * num32)) + (num2 * num30)) * num;
			matrix2.M43 = -(((num5 * num33) - (num4 * num31)) + (num3 * num30)) * num;
			var num29 = (num7 * num14) - (num6 * num15);
			var num28 = (num8 * num14) - (num6 * num16);
			var num27 = (num8 * num15) - (num7 * num16);
			var num26 = (num9 * num14) - (num6 * num17);
			var num25 = (num9 * num15) - (num7 * num17);
			var num24 = (num9 * num16) - (num8 * num17);
			matrix2.M14 = -(((num4 * num29) - (num3 * num28)) + (num2 * num27)) * num;
			matrix2.M24 = (((num5 * num29) - (num3 * num26)) + (num2 * num25)) * num;
			matrix2.M34 = -(((num5 * num28) - (num4 * num26)) + (num2 * num24)) * num;
			matrix2.M44 = (((num5 * num27) - (num4 * num25)) + (num3 * num24)) * num;
			return matrix2;
		}


		public override string ToString()
		{
			var format = "[ {0:##.##} - {1:##.##} - {2:##.##} - {3:##.##} ])";
			var row1 = string.Format(format, M11, M12, M13, M14);
			var row2 = string.Format(format, M21, M22, M23, M24);
			var row3 = string.Format(format, M31, M32, M33, M34);
			var row4 = string.Format(format, M41, M42, M43, M44);

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
                matrix.M11, matrix.M12, matrix.M13, matrix.M14, 
                matrix.M21, matrix.M22, matrix.M23, matrix.M24, 
                matrix.M31, matrix.M32, matrix.M33, matrix.M34, 
                matrix.M41, matrix.M42, matrix.M43, matrix.M44
            );
            return outMatrix;
        }
#endif

#if(SILVERLIGHT)
		public System.Windows.Media.Media3D.Matrix3D	ToMatrix3D()
		{
			var m3d = new System.Windows.Media.Media3D.Matrix3D
				(
				M11, M12, M13, M14,
				M21, M22, M23, M24,
				M31, M32, M33, M34,
				M41, M42, M43, M44
				);
			return m3d;
		}
#endif
	}
}
