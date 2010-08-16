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
		protected TextureMipMapLevel Texture;

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

		protected float ZInterpolateX;
		protected float UzInerpolateX;
		protected float VzInterpolateX;
		protected float ZInterpolateY;
		protected float UzInterpolateY;
		protected float VInterpolateY;
		protected float ZInterpolateY1;
		protected float UInterpolateY1;
		protected float VInterpolateY1;

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
		protected static int AlphaFull;

		static Triangle()
		{
			uint g = 0xff000000;
			GreenMask = (int)g;
			RedMask = 0x00ff0000;
			BlueMask = 0x00ff0000;

			uint a = 0xff000000;
			AlphaFull = (int)a;
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
			vertex.U = -(s * 0.5f) + 0.5f;
			vertex.V = (t * 0.5f) + 0.5f;
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

			var cr1 = map.Pixels[x, y] | AlphaFull;
			var cr2 = map.Pixels[rightOffset, y] | AlphaFull;
			var cr3 = map.Pixels[rightOffset, belowOffset] | AlphaFull;
			var cr4 = map.Pixels[x, belowOffset] | AlphaFull;

			var a = (0x100 - h) * (0x100 - i);
			var b = (0x000 + h) * (0x100 - i);
			var c = (0x000 + h) * (0x000 + i);
			var d = 65536 - a - b - c;

			var red = RedMask & (((cr1 >> 16) * a) + ((cr2 >> 16) * b) + ((cr3 >> 16) * c) + ((cr4 >> 16) * d));
			var green = GreenMask & (((cr1 & 0x0000ff00) * a) + ((cr2 & 0x000000ff00) * b) + ((cr3 & 0x0000ff00) * c) + ((cr4 & 0x0000ff00) * d));
			var blue = BlueMask & (((cr1 & 0x000000ff) * a) + ((cr2 & 0x000000ff) * b) + ((cr3 & 0x000000ff) * c) + ((cr4 & 0x000000ff) * d));

			var pixel = red | (((green | blue) >> 16) & 0xffff) | AlphaFull;
			return pixel;
		}


		public virtual void Draw(RenderFace face, RenderVertex[] vertices)
		{
			var vertexA = vertices[face.A];
			var vertexB = vertices[face.B];
			var vertexC = vertices[face.C];

			if (null != face.Texture1TextureCoordinateA)
			{
				vertexA.U = face.Texture1TextureCoordinateA.U;
				vertexA.V = face.Texture1TextureCoordinateA.V;
			}
			if (null != face.Texture1TextureCoordinateB)
			{
				vertexB.U = face.Texture1TextureCoordinateB.U;
				vertexB.V = face.Texture1TextureCoordinateB.V;
			}
			if (null != face.Texture1TextureCoordinateC)
			{
				vertexC.U = face.Texture1TextureCoordinateC.U;
				vertexC.V = face.Texture1TextureCoordinateC.V;
			}


			Texture = null;
			if (null != face.Texture1)
			{
				Texture = face.Texture1.FullDetailLevel;
			}
			else if (null != face.Texture2)
			{
				Texture = face.Texture2.FullDetailLevel;
				SetSphericalEnvironmentMapTextureCoordinate(vertexA);
				SetSphericalEnvironmentMapTextureCoordinate(vertexB);
				SetSphericalEnvironmentMapTextureCoordinate(vertexC);
			}
			var textureWidth = 0;
			var textureHeight = 0;

			if (null != Texture)
			{
				textureWidth = Texture.Width;
				textureHeight = Texture.Height;
			}

			GetSortedPoints(face, ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.ProjectedVector.X;
			var ya = vertexA.ProjectedVector.Y;
			var za = vertexA.ProjectedVector.Z;
			var ua = vertexA.U * textureWidth;
			var va = vertexA.V * textureHeight;
			var ra = ((float)face.CalculatedColorA.Red) / 255f;
			var ga = ((float)face.CalculatedColorA.Green) / 255f;
			var ba = ((float)face.CalculatedColorA.Blue) / 255f;
			var aa = ((float)face.CalculatedColorA.Alpha) / 255f;

			var xb = vertexB.ProjectedVector.X;
			var yb = vertexB.ProjectedVector.Y;
			var zb = vertexB.ProjectedVector.Z;
			var ub = vertexB.U * textureWidth;
			var vb = vertexB.V * textureHeight;
			var rb = ((float)face.CalculatedColorB.Red) / 255f;
			var gb = ((float)face.CalculatedColorB.Green) / 255f;
			var bb = ((float)face.CalculatedColorB.Blue) / 255f;
			var ab = ((float)face.CalculatedColorB.Alpha) / 255f;

			var xc = vertexC.ProjectedVector.X;
			var yc = vertexC.ProjectedVector.Y;
			var zc = vertexC.ProjectedVector.Z;
			var uc = vertexC.U * textureWidth;
			var vc = vertexC.V * textureHeight;
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

			var uOneOverZA = ua * oneOverZA;
			var vOneOverZA = va * oneOverZA;

			var uOneOverZB = ub * oneOverZB;
			var vOneOverZB = vb * oneOverZB;

			var uOneOverZC = uc * oneOverZC;
			var vOneOverZC = vc * oneOverZC;

			var denominator = ((xc - xa) * (yb - ya) - (xb - xa) * (yc - ya));
			if (float.IsInfinity(denominator) || denominator == 0)
			{
				return;
			}

			denominator = 1f / denominator;
			ZInterpolateX = ((oneOverZC - oneOverZA) * deltaYA - (oneOverZB - oneOverZA) * deltaYB) * denominator;
			UzInerpolateX = ((uOneOverZC - uOneOverZA) * deltaYA - (uOneOverZB - uOneOverZA) * deltaYB) * denominator;
			VzInterpolateX = ((vOneOverZC - vOneOverZA) * deltaYA - (vOneOverZB - vOneOverZA) * deltaYB) * denominator;
			ZInterpolateY = ((oneOverZB - oneOverZA) * deltaXB - (oneOverZC - oneOverZA) * deltaXA) * denominator;
			UzInterpolateY = ((uOneOverZB - uOneOverZA) * deltaXB - (uOneOverZC - uOneOverZA) * deltaXA) * denominator;
			VInterpolateY = ((vOneOverZB - vOneOverZA) * deltaXB - (vOneOverZC - vOneOverZA) * deltaXA) * denominator;

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
				UInterpolateY1 = xInterpolateB * UzInerpolateX + UzInterpolateY;
				VInterpolateY1 = xInterpolateB * VzInterpolateX + VInterpolateY;


				// Subpixling
				X1 = xa + subPixelY * XInterpolate1;
				R1 = ra + subPixelY * RInterpolate1;
				G1 = ga + subPixelY * GInterpolate1;
				B1 = ba + subPixelY * BInterpolate1;
				A1 = aa + subPixelY * AInterpolate1;

				Z1 = oneOverZA + subPixelY * ZInterpolateY1;
				U1 = uOneOverZA + subPixelY * UInterpolateY1;
				V1 = vOneOverZA + subPixelY * VInterpolateY1;


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
					UInterpolateY1 = XInterpolate1 * UzInerpolateX + UzInterpolateY;
					VInterpolateY1 = XInterpolate1 * VzInterpolateX + VInterpolateY;

					// Subpixling
					X1 = xa + subPixelY * XInterpolate1;
					R1 = ra + subPixelY * RInterpolate1;
					G1 = ga + subPixelY * GInterpolate1;
					B1 = ba + subPixelY * BInterpolate1;
					A1 = aa + subPixelY * AInterpolate1;


					Z1 = oneOverZA + subPixelY * ZInterpolateY1;
					U1 = uOneOverZA + subPixelY * UInterpolateY1;
					V1 = vOneOverZA + subPixelY * VInterpolateY1;

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
					UInterpolateY1 = xInterpolateC * UzInerpolateX + UzInterpolateY;
					VInterpolateY1 = xInterpolateC * VzInterpolateX + VInterpolateY;

					subPixelY = 1 - (yb - ybInt);

					// Subpixling
					X1 = xb + subPixelY * XInterpolate1;
					R1 = rb + subPixelY * RInterpolate1;
					G1 = gb + subPixelY * GInterpolate1;
					B1 = bb + subPixelY * BInterpolate1;
					A1 = ab + subPixelY * AInterpolate1;

					Z1 = oneOverZB + subPixelY * ZInterpolateY1;
					U1 = uOneOverZB + subPixelY * UInterpolateY1;
					V1 = vOneOverZB + subPixelY * VInterpolateY1;

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
				U1 += UInterpolateY1;
				V1 += VInterpolateY1;

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