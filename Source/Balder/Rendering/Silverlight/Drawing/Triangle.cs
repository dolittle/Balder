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

		protected int PixelOffset;

		protected int Y1Int;
		protected int Y2Int;

		protected float XInterpolate1;
		protected float XInterpolate2;

		protected TextureMipMapLevel Texture;

		protected float X1;
		protected float X2;
		protected float Z1;
		protected float U1;
		protected float V1;

		protected float ZInterpolationX;
		protected float UzInerpolationX;
		protected float VzInterpolationX;
		protected float ZInterpolationY;
		protected float UzInterpolationY;
		protected float VInterpolationY;
		protected float ZInterpolateY1;
		protected float UInterpolateY1;
		protected float VInterpolateY1;

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
			var s = (r.X / m);
			var t = (r.Y / m);
			vertex.U = (s * 0.5f) + 0.5f;
			vertex.V = -(t * 0.5f) + 0.5f;
		}


		protected void GetSortedPoints(ref RenderVertex vertexA,
										ref RenderVertex vertexB,
										ref RenderVertex vertexC)
		{
			var point1 = vertexA;
			var point2 = vertexB;
			var point3 = vertexC;

			if (point1.TranslatedScreenCoordinates.Y > point2.TranslatedScreenCoordinates.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;
			}

			if (point1.TranslatedScreenCoordinates.Y > point3.TranslatedScreenCoordinates.Y)
			{
				var p = point1;
				point1 = point3;
				point3 = p;
			}


			if (point2.TranslatedScreenCoordinates.Y > point3.TranslatedScreenCoordinates.Y)
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

			if (point1.TranslatedScreenCoordinates.Y > point2.TranslatedScreenCoordinates.Y)
			{
				var p = point1;
				point1 = point2;
				point2 = p;

				var ca = face.CalculatedColorA;
				var cb = face.CalculatedColorB;
				face.CalculatedColorB = ca;
				face.CalculatedColorA = cb;
			}

			if (point1.TranslatedScreenCoordinates.Y > point3.TranslatedScreenCoordinates.Y)
			{
				var p = point1;
				point1 = point3;
				point3 = p;

				var ca = face.CalculatedColorA;
				var cc = face.CalculatedColorC;
				face.CalculatedColorC = ca;
				face.CalculatedColorA = cc;
			}


			if (point2.TranslatedScreenCoordinates.Y > point3.TranslatedScreenCoordinates.Y)
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

			int red = RedMask & (((cr1 >> 16) * a) + ((cr2 >> 16) * b) + ((cr3 >> 16) * c) + ((cr4 >> 16) * d));
			int green = GreenMask & (((cr1 & 0x0000ff00) * a) + ((cr2 & 0x000000ff00) * b) + ((cr3 & 0x0000ff00) * c) + ((cr4 & 0x0000ff00) * d));
			int blue = BlueMask & (((cr1 & 0x000000ff) * a) + ((cr2 & 0x000000ff) * b) + ((cr3 & 0x000000ff) * c) + ((cr4 & 0x000000ff) * d));

			var pixel = red | (((green | blue) >> 16) & 0xffff) | AlphaFull;
			return pixel;
		}


		public virtual void Draw(RenderFace face, RenderVertex[] vertices)
		{
			var vertexA = vertices[face.A];
			var vertexB = vertices[face.B];
			var vertexC = vertices[face.C];

			if (null != face.DiffuseTextureCoordinateA)
			{
				vertexA.U = face.DiffuseTextureCoordinateA.U;
				vertexA.V = face.DiffuseTextureCoordinateA.V;
			}
			if (null != face.DiffuseTextureCoordinateB)
			{
				vertexB.U = face.DiffuseTextureCoordinateB.U;
				vertexB.V = face.DiffuseTextureCoordinateB.V;
			}
			if (null != face.DiffuseTextureCoordinateC)
			{
				vertexC.U = face.DiffuseTextureCoordinateC.U;
				vertexC.V = face.DiffuseTextureCoordinateC.V;
			}

			
			Texture = null;
			if (null != face.DiffuseTexture)
			{
				Texture = face.DiffuseTexture.FullDetailLevel;
			}
			else if (null != face.ReflectionTexture)
			{
				Texture = face.ReflectionTexture.FullDetailLevel;
				SetSphericalEnvironmentMapTextureCoordinate(vertexA);
				SetSphericalEnvironmentMapTextureCoordinate(vertexB);
				SetSphericalEnvironmentMapTextureCoordinate(vertexC);
			}
			var textureWidth = 0;
			var textureHeight = 0;

			if( null != Texture )
			{
				textureWidth = Texture.Width;
				textureHeight = Texture.Height;
			}

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var za = vertexA.DepthBufferAdjustedZ;
			var ua = vertexA.U * textureWidth;
			var va = vertexA.V * textureHeight;

			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var zb = vertexB.DepthBufferAdjustedZ;
			var ub = vertexB.U * textureWidth;
			var vb = vertexB.V * textureHeight;

			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;
			var zc = vertexC.DepthBufferAdjustedZ;
			var uc = vertexC.U * textureWidth;
			var vc = vertexC.V * textureHeight;

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

			var xInterpolateA = deltaXA / deltaYA;
			var xInterpolateB = deltaXB / deltaYB;
			var xInterpolateC = deltaXC / deltaYC;

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
			ZInterpolationX = ((oneOverZC - oneOverZA) * deltaYA - (oneOverZB - oneOverZA) * deltaYB) * denominator;
			UzInerpolationX = ((uOneOverZC - uOneOverZA) * deltaYA - (uOneOverZB - uOneOverZA) * deltaYB) * denominator;
			VzInterpolationX = ((vOneOverZC - vOneOverZA) * deltaYA - (vOneOverZB - vOneOverZA) * deltaYB) * denominator;
			ZInterpolationY = ((oneOverZB - oneOverZA) * deltaXB - (oneOverZC - oneOverZA) * deltaXA) * denominator;
			UzInterpolationY = ((uOneOverZB - uOneOverZA) * deltaXB - (uOneOverZC - uOneOverZA) * deltaXA) * denominator;
			VInterpolationY = ((vOneOverZB - vOneOverZA) * deltaXB - (vOneOverZC - vOneOverZA) * deltaXA) * denominator;

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

				ZInterpolateY1 = xInterpolateB * ZInterpolationX + ZInterpolationY;
				UInterpolateY1 = xInterpolateB * UzInerpolationX + UzInterpolationY;
				VInterpolateY1 = xInterpolateB * VzInterpolationX + VInterpolationY;


				// Subpixling
				X1 = xa + subPixelY * XInterpolate1;

				Z1 = oneOverZA + subPixelY * ZInterpolateY1;
				U1 = uOneOverZA + subPixelY * UInterpolateY1;
				V1 = vOneOverZA + subPixelY * VInterpolateY1;


				if (yaInt < ybInt)
				{
					X2 = xa + subPixelY * xInterpolateA;
					XInterpolate2 = xInterpolateA;

					Y1Int = yaInt;
					Y2Int = ybInt;

					DrawSubTriangleSegment();
				}

				if (ybInt < ycInt)
				{
					X2 = xb + (1f - (yb - ybInt)) * xInterpolateC;
					XInterpolate2 = xInterpolateC;

					Y1Int = ybInt;
					Y2Int = ycInt;

					DrawSubTriangleSegment();
				}
			}
			else // Hypotenuse is to the right
			{
				XInterpolate2 = xInterpolateB;

				X2 = xa + subPixelY * xInterpolateB;

				if (yaInt < ybInt)
				{
					XInterpolate1 = xInterpolateA;

					ZInterpolateY1 = XInterpolate1 * ZInterpolationX + ZInterpolationY;
					UInterpolateY1 = XInterpolate1 * UzInerpolationX + UzInterpolationY;
					VInterpolateY1 = XInterpolate1 * VzInterpolationX + VInterpolationY;

					// Subpixling
					X1 = xa + subPixelY * XInterpolate1;

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

					ZInterpolateY1 = xInterpolateC * ZInterpolationX + ZInterpolationY;
					UInterpolateY1 = xInterpolateC * UzInerpolationX + UzInterpolationY;
					VInterpolateY1 = xInterpolateC * VzInterpolationX + VInterpolationY;

					subPixelY = 1 - (yb - ybInt);

					// Subpixling
					X1 = xb + subPixelY * XInterpolate1;

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
						DrawSpan(PixelOffset);
					}
				}
				X1 += XInterpolate1;
				X2 += XInterpolate2;

				Z1 += ZInterpolateY1;
				U1 += UInterpolateY1;
				V1 += VInterpolateY1;

				yoffset += BufferContainer.Width;
			}
		}

		protected virtual void DrawSpan(int offset)
		{
		}
	}
}
#endif