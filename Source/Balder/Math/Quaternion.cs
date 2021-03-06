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
	public struct Quaternion
	{
		public static Quaternion	Identity = new Quaternion(0f,0f,0f,1f);
				
		public float W;
		public float X;
		public float Y;
		public float Z;

		public Quaternion(float x, float y, float z, float w)
		{
			X = x;
			Y = y;
			Z = z;
			W = w;
		}

		public Quaternion(Quaternion Quaternion)
		{
			X = Quaternion.X;
			Y = Quaternion.Y;
			Z = Quaternion.Z;
			W = Quaternion.W;
		}


		/// <summary>
        /// Converts a vector and a w value to a Quaternion
		/// </summary>
		/// <param name="vector"></param>
		/// <param name="w"></param>
		public Quaternion(Vector vector)
		{
			X = vector.X;
			Y = vector.Y;
			Z = vector.Z;
			W = vector.W;
		}

		public static Quaternion CreateFromYawPitchRoll(float yaw, float pitch, float roll)
		{
			var quaternion = new Quaternion();
			var halfRoll = roll * 0.5f;
			var halfPitch = pitch * 0.5f;
			var halfYaw = yaw * 0.5f;
			var rollSin = (float)System.Math.Sin(halfRoll);
			var rollCos = (float)System.Math.Cos(halfRoll);
			var pitchSin = (float)System.Math.Sin(halfPitch);
			var pitchCos = (float)System.Math.Cos(halfPitch);
			var yawSin = (float)System.Math.Sin(halfYaw);
			var yawCos = (float)System.Math.Cos(halfYaw);

			quaternion.X = ((yawCos * pitchSin) * rollCos) + ((yawSin * pitchCos) * rollSin);
			quaternion.Y = ((yawSin * pitchCos) * rollCos) - ((yawCos * pitchSin) * rollSin);
			quaternion.Z = ((yawCos * pitchSin) * rollSin) - ((yawSin * pitchSin) * rollCos);
			quaternion.W = ((yawCos * pitchSin) * rollCos) + ((yawSin * pitchSin) * rollSin);

			return quaternion;
		}


		/// <summary>
		/// Quaternion -> matrix
		/// </summary>
		/// <param name="Quaternion"></param>
		/// <returns></returns>
		public static explicit operator Matrix(Quaternion Quaternion)
		{
			Matrix Result = new Matrix();
			var x2 = Quaternion.X + Quaternion.X;
			var y2 = Quaternion.Y + Quaternion.Y;
			var z2 = Quaternion.Z + Quaternion.Z;
			var xx = Quaternion.X * x2;
			var xy = Quaternion.X * y2;
			var xz = Quaternion.X * z2;
			var yy = Quaternion.Y * y2;
			var yz = Quaternion.Y * z2;
			var zz = Quaternion.Z * z2;
			var wx = Quaternion.W * x2;
			var wy = Quaternion.W * y2;
			var wz = Quaternion.W * z2;

			Result.M11 = 1 - yy - zz;
			Result.M21 = xy - wz;
			Result.M31 = xz + wy;
			Result.M41 = 0;

			Result.M12 = xy + wz;
			Result.M22 = 1 - xx - zz;
			Result.M32 = yz - wx;
			Result.M42 = 0;

			Result.M13 = xz - wy;
			Result.M23 = yz + wx;
			Result.M33 = 1 - xx - yy;
			Result.M43 = 0;

			Result.M14 = 0;
			Result.M24 = 0;
			Result.M34 = 0;
			Result.M44 = 1;

			return Result;
		}

		public static Vector operator *(Quaternion q, Vector v)
		{
			// From NVIDIA SDK
			var qvec = new Vector(q.X, q.Y, q.Z);
			var uv = qvec.Cross(v);
			var uuv = qvec.Cross(uv);
			uv *= (2.0f * q.W);
			uuv *= 2.0f;
			return v + uv + uuv;
		}

		public static Quaternion operator *(Quaternion A, Quaternion B)
		{
			return new Quaternion
				(
				/*C.x = */A.W * B.X + A.X * B.W + A.Y * B.Z - A.Z * B.Y,
				/*C.y = */A.W * B.Y - A.X * B.Z + A.Y * B.W + A.Z * B.X,
				/*C.z = */A.W * B.Z + A.X * B.Y - A.Y * B.X + A.Z * B.W,
				/*C.w = */A.W * B.W - A.X * B.X - A.Y * B.Y - A.Z * B.Z
			);
		}

		public Quaternion Conjugate()
		{
			return new Quaternion(-X, -Y, -Z, W);
		}

		public float Magnitude()
		{
			return Core.Sqrt(W * W + X * X + Y * Y + Z * Z);
		}

		public Quaternion Normalize()
		{
			float square = Magnitude();

			return new Quaternion(X / square, Y / square, Z / square, W / square);
		}


		public static Quaternion Slerp(Quaternion Start, Quaternion Dest, float Time)
		{
			// calc cosine
			float cosom = Start.X * Dest.X +
					 Start.Y * Dest.Y +
					 Start.Z * Dest.Z +
					 Start.W * Dest.W;

			Quaternion to1;
			// adjust signs (if necessary)
			if (cosom < 0.0)
			{
				cosom = -cosom;
				to1.X = -Dest.X;
				to1.Y = -Dest.Y;
				to1.Z = -Dest.Z;
				to1.W = -Dest.W;
			}
			else
			{
				to1.X = Dest.X;
				to1.Y = Dest.Y;
				to1.Z = Dest.Z;
				to1.W = Dest.W;
			}

			float scale0;
			float scale1;
			// calculate coefficients
			if (1 - cosom > 1e-12)
			{
				// standard case (slerp)
				float omega = (float)System.Math.Acos(cosom);
				float sinom = (float)System.Math.Sin(omega);
				scale0 = (float)(System.Math.Sin((1 - Time) * omega) / sinom);
				scale1 = (float)(System.Math.Sin(Time * omega) / sinom);
			}
			else
			{
				// "from" and "to" quaternions are very close
				// so we can do a linear interpolation
				scale0 = 1 - Time;
				scale1 = Time;
			}

			// calculate final values
			Quaternion q2;
			q2.X = (scale0 * Start.X) + (scale1 * to1.X);
			q2.Y = (scale0 * Start.Y) + (scale1 * to1.Y);
			q2.Z = (scale0 * Start.Z) + (scale1 * to1.Z);
			q2.W = (scale0 * Start.W) + (scale1 * to1.W);


			return q2.Normalize();
		}

		public static Quaternion IdentityMultiplication()
		{
			return new Quaternion(0f, 0f, 0f, 1f);
		}

		public static Quaternion FromAngleAxis(float angle, Vector axis)
		{
			var halfAngle = 0.5f * angle;
			var sin = (float)System.Math.Sin(halfAngle);
			var w = (float)System.Math.Cos(halfAngle);
			var x = sin * axis.X;
			var y = sin * axis.Y;
			var z = sin * axis.Z;

			return new Quaternion(x, y, z, w);
		}
	}
}

