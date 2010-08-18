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
#if(SILVERLIGHT)
using Balder.Math;


namespace Balder.Rendering.Silverlight.Drawing
{
	public abstract class Triangle
	{
		protected int[] Framebuffer;
		protected uint[] DepthBuffer;

		protected TextureMipMapLevel Texture1;
		protected TextureMipMapLevel Texture2;
		protected int Texture1Factor;
		protected int Texture2Factor;

		protected int PixelOffset;

		protected int Y1Int;
		protected int Y2Int;

		protected float X1;
		protected float X2;

		protected float XInterpolate1;
		protected float XInterpolate2;

		protected float R1;
		protected float G1;
		protected float B1;
		protected float A1;

		protected float R2;
		protected float G2;
		protected float B2;
		protected float A2;

		protected float RInterpolate1;
		protected float GInterpolate1;
		protected float BInterpolate1;
		protected float AInterpolate1;

		protected float RInterpolate2;
		protected float GInterpolate2;
		protected float BInterpolate2;
		protected float AInterpolate2;

		protected float Z1;
		protected float U1;
		protected float V1;

		protected float U2;
		protected float V2;

		protected float ZInterpolateX;
		protected float ZInterpolateY;
		protected float ZInterpolateY1;

		protected float U1zInterpolateX;
		protected float V1zInterpolateX;
		protected float U1zInterpolateY;
		protected float V1InterpolateY;
		protected float U1InterpolateY1;
		protected float V1InterpolateY1;

		protected float U2zInterpolateX;
		protected float V2zInterpolateX;
		protected float U2zInterpolateY;
		protected float V2InterpolateY;
		protected float U2InterpolateY1;
		protected float V2InterpolateY1;

		protected float RScanline;
		protected float GScanline;
		protected float BScanline;
		protected float AScanline;
		protected float RScanlineInterpolate;
		protected float GScanlineInterpolate;
		protected float BScanlineInterpolate;
		protected float AScanlineInterpolate;


		protected int ColorAsInt;

		protected static int RedMask;
		protected static int GreenMask;
		protected static int BlueMask;

		static Triangle()
		{
			uint g = 0xff000000;
			GreenMask = (int)g;
			RedMask = 0x00ff0000;
			BlueMask = 0x00ff0000;
		}

		protected static void SetSphericalEnvironmentMapTextureCoordinate(RenderVertex vertex)
		{
			var u = vertex.TransformedVectorNormalized;
			var n = vertex.TransformedNormal;
			var r = Vector.Reflect(n, u);
			var m = MathHelper.Sqrt((r.X * r.X) + (r.Y * r.Y) +
									((r.Z + 0f) * (r.Z + 0f)));

			var m1 = 1f/m;
			var s = (r.X * m1);
			var t = (r.Y * m1);
			vertex.U2 = -(s * 0.5f) + 0.5f;
			vertex.V2 = (t * 0.5f) + 0.5f;
		}


		protected void GetSortedPoints(ref RenderVertex vertexA,
										ref RenderVertex vertexB,
										ref RenderVertex vertexC)
		{
			var point1 = vertexA;
			var point2 = vertexB;
			var point3 = vertexC;

			if (point1.ProjectedVector.Y > point2.ProjectedVector.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;
			}

			if (point1.ProjectedVector.Y > point3.ProjectedVector.Y)
			{
				var p = point1;
				point1 = point3;
				point3 = p;
			}


			if (point2.ProjectedVector.Y > point3.ProjectedVector.Y)
			{
				var p = point2;
				point2 = point3;
				point3 = p;
			}

			vertexA = point1;
			vertexB = point2;
			vertexC = point3;
		}

		protected void GetSortedPoints(RenderFace face,
										ref RenderVertex vertexA,
										ref RenderVertex vertexB,
										ref RenderVertex vertexC)
		{
			var point1 = vertexA;
			var point2 = vertexB;
			var point3 = vertexC;

			if (point1.ProjectedVector.Y > point2.ProjectedVector.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;

				var ca = face.CalculatedColorA;
				var cb = face.CalculatedColorB;
				face.CalculatedColorB = ca;
				face.CalculatedColorA = cb;
			}

			if (point1.ProjectedVector.Y > point3.ProjectedVector.Y)
			{
				var p = point1;
				point1 = point3;
				point3 = p;

				var ca = face.CalculatedColorA;
				var cc = face.CalculatedColorC;
				face.CalculatedColorC = ca;
				face.CalculatedColorA = cc;
			}


			if (point2.ProjectedVector.Y > point3.ProjectedVector.Y)
			{
				var p = point2;
				point2 = point3;
				point3 = p;

				var cb = face.CalculatedColorB;
				var cc = face.CalculatedColorC;
				face.CalculatedColorC = cb;
				face.CalculatedColorB = cc;
			}

			vertexA = point1;
			vertexB = point2;
			vertexC = point3;
		}


		protected int Bilerp(TextureMipMapLevel map, int x, int y, float u, float v)
		{
			var h = ((int)(u * 256f)) & 0xff;
			var i = ((int)(v * 256f)) & 0xff;

			var rightOffset = x + 1;
			var belowOffset = y + 1;

			if (rightOffset >= map.Width)
			{
				rightOffset = map.Width - 1;
			}
			if (belowOffset >= map.Height)
			{
				belowOffset = map.Height - 1;
			}

			var cr1 = map.Pixels[x, y] | Color.AlphaFull;
			var cr2 = map.Pixels[rightOffset, y] | Color.AlphaFull;
			var cr3 = map.Pixels[rightOffset, belowOffset] | Color.AlphaFull;
			var cr4 = map.Pixels[x, belowOffset] | Color.AlphaFull;

			var a = (0x100 - h) * (0x100 - i);
			var b = (0x000 + h) * (0x100 - i);
			var c = (0x000 + h) * (0x000 + i);
			var d = 65536 - a - b - c;

			var red = RedMask & (((cr1 >> 16) * a) + ((cr2 >> 16) * b) + ((cr3 >> 16) * c) + ((cr4 >> 16) * d));
			var green = GreenMask & (((cr1 & 0x0000ff00) * a) + ((cr2 & 0x000000ff00) * b) + ((cr3 & 0x0000ff00) * c) + ((cr4 & 0x0000ff00) * d));
			var blue = BlueMask & (((cr1 & 0x000000ff) * a) + ((cr2 & 0x000000ff) * b) + ((cr3 & 0x000000ff) * c) + ((cr4 & 0x000000ff) * d));

			var pixel = red | (((green | blue) >> 16) & 0xffff) | Color.AlphaFull;
			return pixel;
		}


		public virtual void Draw(RenderFace face, RenderVertex[] vertices)
		{
			var vertexA = vertices[face.A];
			var vertexB = vertices[face.B];
			var vertexC = vertices[face.C];

			if (null != face.Texture1TextureCoordinateA)
			{
				vertexA.U1 = face.Texture1TextureCoordinateA.U;
				vertexA.V1 = face.Texture1TextureCoordinateA.V;
			}
			if (null != face.Texture1TextureCoordinateB)
			{
				vertexB.U1 = face.Texture1TextureCoordinateB.U;
				vertexB.V1 = face.Texture1TextureCoordinateB.V;
			}
			if (null != face.Texture1TextureCoordinateC)
			{
				vertexC.U1 = face.Texture1TextureCoordinateC.U;
				vertexC.V1 = face.Texture1TextureCoordinateC.V;
			}

			if (null != face.Texture2TextureCoordinateA)
			{
				vertexA.U2 = face.Texture2TextureCoordinateA.U;
				vertexA.V2 = face.Texture2TextureCoordinateA.V;
			}
			if (null != face.Texture2TextureCoordinateB)
			{
				vertexB.U2 = face.Texture2TextureCoordinateB.U;
				vertexB.V2 = face.Texture2TextureCoordinateB.V;
			}
			if (null != face.Texture2TextureCoordinateC)
			{
				vertexC.U2 = face.Texture2TextureCoordinateC.U;
				vertexC.V2 = face.Texture2TextureCoordinateC.V;
			}

			vertexA.U2 = vertexA.U1;
			vertexA.V2 = vertexA.V1;

			vertexB.U2 = vertexB.U1;
			vertexB.V2 = vertexB.V1;

			vertexC.U2 = vertexC.U1;
			vertexC.V2 = vertexC.V1;

			Texture1 = null;
			if (null != face.Texture1)
			{
				Texture1 = face.Texture1.FullDetailLevel;
			}

			if (null != face.Texture2)
			{
				Texture2 = face.Texture2.FullDetailLevel;
				
				
				SetSphericalEnvironmentMapTextureCoordinate(vertexA);
				
				SetSphericalEnvironmentMapTextureCoordinate(vertexB);
				SetSphericalEnvironmentMapTextureCoordinate(vertexC);
			}

			Texture1Factor = face.Texture1Factor;
			Texture2Factor = face.Texture2Factor;
			var texture1Width = 0;
			var texture1Height = 0;
			var texture2Width = 0;
			var texture2Height = 0;

			if (null != Texture1)
			{
				texture1Width = Texture1.Width;
				texture1Height = Texture1.Height;
			}

			if (null != Texture2)
			{
				texture2Width = Texture2.Width;
				texture2Height = Texture2.Height;
			}

			GetSortedPoints(face, ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.ProjectedVector.X;
			var ya = vertexA.ProjectedVector.Y;
			var za = vertexA.ProjectedVector.Z;
			var u1a = vertexA.U1 * texture1Width;
			var v1a = vertexA.V1 * texture1Height;
			var u2a = vertexA.U2 * texture2Width;
			var v2a = vertexA.V2 * texture2Height;
			var ra = ((float)face.CalculatedColorA.Red) / 255f;
			var ga = ((float)face.CalculatedColorA.Green) / 255f;
			var ba = ((float)face.CalculatedColorA.Blue) / 255f;
			var aa = ((float)face.CalculatedColorA.Alpha) / 255f;

			var xb = vertexB.ProjectedVector.X;
			var yb = vertexB.ProjectedVector.Y;
			var zb = vertexB.ProjectedVector.Z;
			var u1b = vertexB.U1 * texture1Width;
			var v1b = vertexB.V1 * texture1Height;
			var u2b = vertexB.U2 * texture2Width;
			var v2b = vertexB.V2 * texture2Height;
			var rb = ((float)face.CalculatedColorB.Red) / 255f;
			var gb = ((float)face.CalculatedColorB.Green) / 255f;
			var bb = ((float)face.CalculatedColorB.Blue) / 255f;
			var ab = ((float)face.CalculatedColorB.Alpha) / 255f;

			var xc = vertexC.ProjectedVector.X;
			var yc = vertexC.ProjectedVector.Y;
			var zc = vertexC.ProjectedVector.Z;
			var u1c = vertexC.U1 * texture1Width;
			var v1c = vertexC.V1 * texture1Height;
			var u2c = vertexC.U2 * texture2Width;
			var v2c = vertexC.V2 * texture2Height;
			var rc = ((float)face.CalculatedColorC.Red) / 255f;
			var gc = ((float)face.CalculatedColorC.Green) / 255f;
			var bc = ((float)face.CalculatedColorC.Blue) / 255f;
			var ac = ((float)face.CalculatedColorC.Alpha) / 255f;

			var yaInt = (int)ya;
			var ybInt = (int)yb;
			var ycInt = (int)yc;

			if ((yaInt == ybInt && yaInt == ycInt)
				|| ((int)xa == (int)xb && (int)xa == (int)xc))
			{
				return;
			}

			var deltaXA = xb - xa;
			var deltaXB = xc - xa;
			var deltaXC = xc - xb;

			var deltaYA = yb - ya;
			var deltaYB = yc - ya;
			var deltaYC = yc - yb;

			var deltaRA = rb - ra;
			var deltaRB = rc - ra;
			var deltaRC = rc - rb;

			var deltaGA = gb - ga;
			var deltaGB = gc - ga;
			var deltaGC = gc - gb;

			var deltaBA = bb - ba;
			var deltaBB = bc - ba;
			var deltaBC = bc - bb;

			var deltaAA = ab - aa;
			var deltaAB = ac - aa;
			var deltaAC = ac - ab;

			var xInterpolateA = deltaXA / deltaYA;
			var xInterpolateB = deltaXB / deltaYB;
			var xInterpolateC = deltaXC / deltaYC;

			var rInterpolateA = deltaRA / deltaYA;
			var rInterpolateB = deltaRB / deltaYB;
			var rInterpolateC = deltaRC / deltaYC;

			var gInterpolateA = deltaGA / deltaYA;
			var gInterpolateB = deltaGB / deltaYB;
			var gInterpolateC = deltaGC / deltaYC;

			var bInterpolateA = deltaBA / deltaYA;
			var bInterpolateB = deltaBB / deltaYB;
			var bInterpolateC = deltaBC / deltaYC;

			var aInterpolateA = deltaAA / deltaYA;
			var aInterpolateB = deltaAB / deltaYB;
			var aInterpolateC = deltaAC / deltaYC;

			var oneOverZA = 1f / za;
			var oneOverZB = 1f / zb;
			var oneOverZC = 1f / zc;


			var u1OneOverZA = u1a * oneOverZA;
			var v1OneOverZA = v1a * oneOverZA;

			var u1OneOverZB = u1b * oneOverZB;
			var v1OneOverZB = v1b * oneOverZB;

			var u1OneOverZC = u1c * oneOverZC;
			var v1OneOverZC = v1c * oneOverZC;

			var u2OneOverZA = u2a * oneOverZA;
			var v2OneOverZA = v2a * oneOverZA;

			var u2OneOverZB = u2b * oneOverZB;
			var v2OneOverZB = v2b * oneOverZB;

			var u2OneOverZC = u2c * oneOverZC;
			var v2OneOverZC = v2c * oneOverZC;

			var denominator = ((xc - xa) * (yb - ya) - (xb - xa) * (yc - ya));
			if (float.IsInfinity(denominator) || denominator == 0)
			{
				return;
			}

			denominator = 1f / denominator;
			ZInterpolateX = ((oneOverZC - oneOverZA) * deltaYA - (oneOverZB - oneOverZA) * deltaYB) * denominator;
			ZInterpolateY = ((oneOverZB - oneOverZA) * deltaXB - (oneOverZC - oneOverZA) * deltaXA) * denominator;

			U1zInterpolateX = ((u1OneOverZC - u1OneOverZA) * deltaYA - (u1OneOverZB - u1OneOverZA) * deltaYB) * denominator;
			V1zInterpolateX = ((v1OneOverZC - v1OneOverZA) * deltaYA - (v1OneOverZB - v1OneOverZA) * deltaYB) * denominator;
			U1zInterpolateY = ((u1OneOverZB - u1OneOverZA) * deltaXB - (u1OneOverZC - u1OneOverZA) * deltaXA) * denominator;
			V1InterpolateY = ((v1OneOverZB - v1OneOverZA) * deltaXB - (v1OneOverZC - v1OneOverZA) * deltaXA) * denominator;

			U2zInterpolateX = ((u2OneOverZC - u2OneOverZA) * deltaYA - (u2OneOverZB - u2OneOverZA) * deltaYB) * denominator;
			V2zInterpolateX = ((v2OneOverZC - v2OneOverZA) * deltaYA - (v2OneOverZB - v2OneOverZA) * deltaYB) * denominator;
			U2zInterpolateY = ((u2OneOverZB - u2OneOverZA) * deltaXB - (u2OneOverZC - u2OneOverZA) * deltaXA) * denominator;
			V2InterpolateY = ((v2OneOverZB - v2OneOverZA) * deltaXB - (v2OneOverZC - v2OneOverZA) * deltaXA) * denominator;

			Framebuffer = BufferContainer.Framebuffer;
			DepthBuffer = BufferContainer.DepthBuffer;

			ColorAsInt = (int)face.Color.ToUInt32();


			var hypotenuseRight = xInterpolateB > xInterpolateA;
			if (ya == yb)
			{
				hypotenuseRight = xa > xb;
			}
			if (yb == yc)
			{
				hypotenuseRight = xc > xb;
			}

			var subPixelY = 1f - (ya - yaInt);
			if (!hypotenuseRight)
			{
				XInterpolate1 = xInterpolateB;
				RInterpolate1 = rInterpolateB;
				GInterpolate1 = gInterpolateB;
				BInterpolate1 = bInterpolateB;
				AInterpolate1 = aInterpolateB;

				ZInterpolateY1 = xInterpolateB * ZInterpolateX + ZInterpolateY;
				U1InterpolateY1 = xInterpolateB * U1zInterpolateX + U1zInterpolateY;
				V1InterpolateY1 = xInterpolateB * V1zInterpolateX + V1InterpolateY;

				U2InterpolateY1 = xInterpolateB * U2zInterpolateX + U2zInterpolateY;
				V2InterpolateY1 = xInterpolateB * V2zInterpolateX + V2InterpolateY;

				// Subpixling
				X1 = xa + subPixelY * XInterpolate1;
				R1 = ra + subPixelY * RInterpolate1;
				G1 = ga + subPixelY * GInterpolate1;
				B1 = ba + subPixelY * BInterpolate1;
				A1 = aa + subPixelY * AInterpolate1;

				Z1 = oneOverZA + subPixelY * ZInterpolateY1;
				U1 = u1OneOverZA + subPixelY * U1InterpolateY1;
				V1 = v1OneOverZA + subPixelY * V1InterpolateY1;

				U2 = u2OneOverZA + subPixelY * U2InterpolateY1;
				V2 = v2OneOverZA + subPixelY * V2InterpolateY1;

				if (yaInt < ybInt)
				{
					X2 = xa + subPixelY * xInterpolateA;
					R2 = ra + subPixelY * rInterpolateA;
					G2 = ga + subPixelY * gInterpolateA;
					B2 = ba + subPixelY * bInterpolateA;
					A2 = aa + subPixelY * aInterpolateA;

					XInterpolate2 = xInterpolateA;
					RInterpolate2 = rInterpolateA;
					GInterpolate2 = gInterpolateA;
					BInterpolate2 = bInterpolateA;
					AInterpolate2 = aInterpolateA;

					Y1Int = yaInt;
					Y2Int = ybInt;

					DrawSubTriangleSegment();
				}

				if (ybInt < ycInt)
				{
					subPixelY = 1f - (yb - ybInt);
					X2 = xb + subPixelY * xInterpolateC;
					R2 = rb + subPixelY * rInterpolateC;
					G2 = gb + subPixelY * gInterpolateC;
					B2 = bb + subPixelY * bInterpolateC;
					A2 = ab + subPixelY * aInterpolateC;

					XInterpolate2 = xInterpolateC;
					RInterpolate2 = rInterpolateC;
					GInterpolate2 = gInterpolateC;
					BInterpolate2 = bInterpolateC;
					AInterpolate2 = aInterpolateC;

					Y1Int = ybInt;
					Y2Int = ycInt;

					DrawSubTriangleSegment();
				}
			}
			else // Hypotenuse is to the right
			{
				XInterpolate2 = xInterpolateB;
				RInterpolate2 = rInterpolateB;
				GInterpolate2 = gInterpolateB;
				BInterpolate2 = bInterpolateB;
				AInterpolate2 = aInterpolateB;

				X2 = xa + subPixelY * xInterpolateB;
				R2 = ra + subPixelY * rInterpolateB;
				G2 = ga + subPixelY * gInterpolateB;
				B2 = ba + subPixelY * bInterpolateB;
				A2 = aa + subPixelY * aInterpolateB;

				if (yaInt < ybInt)
				{
					XInterpolate1 = xInterpolateA;
					RInterpolate1 = rInterpolateA;
					GInterpolate1 = gInterpolateA;
					BInterpolate1 = bInterpolateA;
					AInterpolate1 = aInterpolateA;

					ZInterpolateY1 = XInterpolate1 * ZInterpolateX + ZInterpolateY;
					U1InterpolateY1 = XInterpolate1 * U1zInterpolateX + U1zInterpolateY;
					V1InterpolateY1 = XInterpolate1 * V1zInterpolateX + V1InterpolateY;

					U2InterpolateY1 = XInterpolate1 * U2zInterpolateX + U2zInterpolateY;
					V2InterpolateY1 = XInterpolate1 * V2zInterpolateX + V2InterpolateY;

					// Subpixling
					X1 = xa + subPixelY * XInterpolate1;
					R1 = ra + subPixelY * RInterpolate1;
					G1 = ga + subPixelY * GInterpolate1;
					B1 = ba + subPixelY * BInterpolate1;
					A1 = aa + subPixelY * AInterpolate1;


					Z1 = oneOverZA + subPixelY * ZInterpolateY1;
					U1 = u1OneOverZA + subPixelY * U1InterpolateY1;
					V1 = v1OneOverZA + subPixelY * V1InterpolateY1;

					U2 = u2OneOverZA + subPixelY * U2InterpolateY1;
					V2 = v2OneOverZA + subPixelY * V2InterpolateY1;

					Y1Int = yaInt;
					Y2Int = ybInt;

					DrawSubTriangleSegment();
				}
				if (ybInt < ycInt)
				{
					XInterpolate1 = xInterpolateC;
					RInterpolate1 = rInterpolateC;
					GInterpolate1 = gInterpolateC;
					BInterpolate1 = bInterpolateC;
					AInterpolate1 = aInterpolateC;

					ZInterpolateY1 = xInterpolateC * ZInterpolateX + ZInterpolateY;
					U1InterpolateY1 = xInterpolateC * U1zInterpolateX + U1zInterpolateY;
					V1InterpolateY1 = xInterpolateC * V1zInterpolateX + V1InterpolateY;

					U2InterpolateY1 = xInterpolateC * U2zInterpolateX + U2zInterpolateY;
					V2InterpolateY1 = xInterpolateC * V2zInterpolateX + V2InterpolateY;

					subPixelY = 1 - (yb - ybInt);

					// Subpixling
					X1 = xb + subPixelY * XInterpolate1;
					R1 = rb + subPixelY * RInterpolate1;
					G1 = gb + subPixelY * GInterpolate1;
					B1 = bb + subPixelY * BInterpolate1;
					A1 = ab + subPixelY * AInterpolate1;

					Z1 = oneOverZB + subPixelY * ZInterpolateY1;
					U1 = u1OneOverZB + subPixelY * U1InterpolateY1;
					V1 = v1OneOverZB + subPixelY * V1InterpolateY1;

					U2 = u2OneOverZB + subPixelY * U2InterpolateY1;
					V2 = v2OneOverZB + subPixelY * V2InterpolateY1;

					Y1Int = ybInt;
					Y2Int = ycInt;

					DrawSubTriangleSegment();
				}
			}
		}



		private void DrawSubTriangleSegment()
		{
			var yoffset = BufferContainer.Width * Y1Int;

			for (var y = Y1Int; y < Y2Int; y++)
			{
				if (y >= 0 && y < BufferContainer.Height)
				{
					if ((int)X1 < (int)X2)
					{
						PixelOffset = yoffset + (int)X1;

						RScanline = R1;
						GScanline = G1;
						BScanline = B1;
						AScanline = A1;

						var length = (X2 - X1);

						var oneOverLength = 1f / length;

						RScanlineInterpolate = (R2 - R1) * oneOverLength;
						GScanlineInterpolate = (G2 - G1) * oneOverLength;
						BScanlineInterpolate = (B2 - B1) * oneOverLength;
						AScanlineInterpolate = (A2 - A1) * oneOverLength;


						DrawSpan(PixelOffset);
					}
				}
				X1 += XInterpolate1;
				X2 += XInterpolate2;

				R1 += RInterpolate1;
				G1 += GInterpolate1;
				B1 += BInterpolate1;
				A1 += AInterpolate1;

				R2 += RInterpolate2;
				G2 += GInterpolate2;
				B2 += BInterpolate2;
				A2 += AInterpolate2;

				Z1 += ZInterpolateY1;
				U1 += U1InterpolateY1;
				V1 += V1InterpolateY1;

				U2 += U2InterpolateY1;
				V2 += V2InterpolateY1;

				yoffset += BufferContainer.Width;
			}
		}

		protected virtual void DrawSpan(int offset)
		{
		}

		/*
		protected abstract bool UsesTexture1 { get; }
		protected abstract bool UsesTexture2 { get; }
		protected abstract bool UsesZ { get; }
		protected abstract bool UsesColoring { get; }
		 */
	}
}
#endif