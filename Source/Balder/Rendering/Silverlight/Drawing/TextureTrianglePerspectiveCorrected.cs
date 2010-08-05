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
using System;
using Balder.Materials;
using Balder.Math;

namespace Balder.Rendering.Silverlight.Drawing
{
	public class TextureTrianglePerspectiveCorrected : Triangle
	{
		private static void SetSphericalEnvironmentMapTextureCoordinate(RenderVertex vertex)
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

		public override void Draw(RenderFace face, RenderVertex[] vertices)
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


			texture = null;
			if (null != face.DiffuseTexture)
			{
				texture = face.DiffuseTexture.FullDetailLevel;
			}
			else if (null != face.ReflectionTexture)
			{
				texture = face.ReflectionTexture.FullDetailLevel;
				SetSphericalEnvironmentMapTextureCoordinate(vertexA);
				SetSphericalEnvironmentMapTextureCoordinate(vertexB);
				SetSphericalEnvironmentMapTextureCoordinate(vertexC);
			}

			texels = texture.OriginalPixels;

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var za = vertexA.DepthBufferAdjustedZ;
			var ua = vertexA.U; // * texture.Width;
			var va = vertexA.V; // * texture.Height;

			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var zb = vertexB.DepthBufferAdjustedZ;
			var ub = vertexB.U; // *texture.Width;
			var vb = vertexB.V; // * texture.Height;

			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;
			var zc = vertexC.DepthBufferAdjustedZ;
			var uc = vertexC.U; // * texture.Width;
			var vc = vertexC.V; // * texture.Height;

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

			var deltaZA = zb - za;
			var deltaZB = zc - za;
			var deltaZC = zc - zb;

			var xInterpolateA = deltaXA / deltaYA;
			var xInterpolateB = deltaXB / deltaYB;
			var xInterpolateC = deltaXC / deltaYC;

			var zInterpolateA = deltaZA / deltaYA;
			var zInterpolateB = deltaZB / deltaYB;
			var zInterpolateC = deltaZC / deltaYC;

			var oneOverZA = 1f/za;
			var oneOverZB = 1f/zb;
			var oneOverZC = 1f/zc;

			var uiza = ua * oneOverZA;
			var viza = va * oneOverZA;

			var uizb = ub * oneOverZB;
			var vizb = vb * oneOverZB;

			var uizc = uc * oneOverZC;
			var vizc = vc * oneOverZC;

			var denom = ((xc - xa) * (yb - ya) - (xb - xa) * (yc - ya));
			if (float.IsInfinity(denom) || denom == 0)
			{
				return;
			}

			denom = 1f / denom;	// Reciprocal for speeding up
			dizdx = ((oneOverZC - oneOverZA) * deltaYA - (oneOverZB - oneOverZA) * deltaYB) * denom;
			duizdx = ((uizc - uiza) * deltaYA - (uizb - uiza) * deltaYB) * denom;
			dvizdx = ((vizc - viza) * deltaYA - (vizb - viza) * deltaYB) * denom;
			dizdy = ((oneOverZB - oneOverZA) * deltaXB - (oneOverZC - oneOverZA) * deltaXA) * denom;
			duizdy = ((uizb - uiza) * deltaXB - (uizc - uiza) * deltaXA) * denom;
			dvizdy = ((vizb - viza) * deltaXB - (vizc - viza) * deltaXA) * denom;

			framebuffer = BufferContainer.Framebuffer;
			depthBuffer = BufferContainer.DepthBuffer;


			var hypotenuseRight = xInterpolateB > xInterpolateA;
			if (ya == yb)
			{
				hypotenuseRight = xa > xb;
			}
			if (yb == yc)
			{
				hypotenuseRight = xc > xb;
			}

			var dy = 0f;
			dy = 1f - (ya - yaInt);
			if (!hypotenuseRight)
			{
				xInterpolate1 = xInterpolateB;
				zInterpolate1 = zInterpolateB;

				dizdy1 = xInterpolateB * dizdx + dizdy;
				duizdy1 = xInterpolateB * duizdx + duizdy;
				dvizdy1 = xInterpolateB * dvizdx + dvizdy;


				// Subpixling
				x1 = xa + dy * xInterpolate1;
				z1 = za + dy * zInterpolate1;

				iz1 = oneOverZA + dy * dizdy1;
				uiz1 = uiza + dy * duizdy1;
				viz1 = viza + dy * dvizdy1;


				if (yaInt < ybInt)
				{
					x2 = xa + dy * xInterpolateA;
					xInterpolate2 = xInterpolateA;

					y1Int = yaInt;
					y2Int = ybInt;

					DrawSubTriangleSegment();
				}

				if (ybInt < ycInt)
				{
					x2 = xb + (1f - (yb - ybInt)) * xInterpolateC;
					xInterpolate2 = xInterpolateC;

					y1Int = ybInt;
					y2Int = ycInt;

					DrawSubTriangleSegment();
				}
			}
			else // Hypotenuse is to the right
			{
				xInterpolate2 = xInterpolateB;

				x2 = xa + dy * xInterpolateB;

				if (yaInt < ybInt)
				{
					xInterpolate1 = xInterpolateA;
					zInterpolate1 = zInterpolateA;

					dizdy1 = xInterpolate1 * dizdx + dizdy;
					duizdy1 = xInterpolate1 * duizdx + duizdy;
					dvizdy1 = xInterpolate1 * dvizdx + dvizdy;

					// Subpixling
					x1 = xa + dy * xInterpolate1;
					z1 = za + dy * zInterpolate1;

					iz1 = oneOverZA + dy * dizdy1;
					uiz1 = uiza + dy * duizdy1;
					viz1 = viza + dy * dvizdy1;

					y1Int = yaInt;
					y2Int = ybInt;

					DrawSubTriangleSegment();
				}
				if (ybInt < ycInt)
				{
					xInterpolate1 = xInterpolateC;
					zInterpolate1 = zInterpolateC;

					dizdy1 = xInterpolateC * dizdx + dizdy;
					duizdy1 = xInterpolateC * duizdx + duizdy;
					dvizdy1 = xInterpolateC * dvizdx + dvizdy;

					dy = 1 - (yb - ybInt);

					// Subpixling
					x1 = xb + dy * xInterpolate1;
					z1 = zb + dy * zInterpolate1;

					iz1 = oneOverZB + dy * dizdy1;
					uiz1 = uizb + dy * duizdy1;
					viz1 = vizb + dy * dvizdy1;

					y1Int = ybInt;
					y2Int = ycInt;

					DrawSubTriangleSegment();
				}
			}
		}

		private int[] framebuffer;
		private uint[] depthBuffer;

		int offset = 0;
		int length = 0;

		int xStart = 0;
		int xEnd = 0;

		float zStart = 0f;
		float zAdd = 0f;

		private float x1;
		private float z1;

		private float x2;
		private float z2;

		private int y1Int;
		private int y2Int;

		private float xInterpolate1;
		private float zInterpolate1;

		private float xInterpolate2;
		private float zInterpolate2;

		private TextureMipMapLevel texture;
		private int[] texels;

		private float dizdx;
		private float duizdx;
		private float dvizdx;
		private float dizdy;
		private float duizdy;
		private float dvizdy;
		private float dizdy1;
		private float duizdy1;
		private float dvizdy1;
		private float iz1;
		private float uiz1;
		private float viz1;


		private void DrawSubTriangleSegment()
		{
			/*
			var yClipTop = 0;

			if (y1 < 0)
			{
				yClipTop = -(int)y1;
				y1 = 0;
			}

			if (y2 >= frameBufferHeight)
			{
				y2 = frameBufferHeight - 1;
			}

			var height = y2 - y1;
			if (height == 0)
			{
				return;
			}

			if (yClipTop > 0)
			{
				var yClipTopAsFloat = (float)yClipTop;
				x1 = x1 + xInterpolate1 * yClipTopAsFloat;
				z1 = z1 + zInterpolate1 * yClipTopAsFloat;
				u1 = u1 + uInterpolate1 * yClipTopAsFloat;
				v1 = v1 + vInterpolate1 * yClipTopAsFloat;

				x2 = x2 + xInterpolate2 * yClipTopAsFloat;
				z2 = z2 + zInterpolate2 * yClipTopAsFloat;
				u2 = u2 + uInterpolate2 * yClipTopAsFloat;
				v2 = v2 + vInterpolate2 * yClipTopAsFloat;
			}
			*/
			var yoffset = BufferContainer.Width * y1Int;

			for (var y = y1Int; y < y2Int; y++)
			{
				if (y > 0 && y < BufferContainer.Height)
				{
					xStart = (int)x1;
					xEnd = (int)x2;
					zStart = z1;
					length = xEnd - xStart;

					if (length != 0)
					{
						offset = yoffset + xStart;
						DrawSpan(length,
								 zStart,
								 zAdd,
								 depthBuffer,
								 offset,
								 framebuffer,
								 texture,
								 texels);
					}
				}
				x1 += xInterpolate1;
				x2 += xInterpolate2;

				z1 += zInterpolate1;
				z2 += zInterpolate2;

				iz1 += dizdy1;
				uiz1 += duizdy1;
				viz1 += dvizdy1;

				yoffset += BufferContainer.Width;

			}

		}


		protected virtual void DrawSpan(
			int length,
			float zStart,
			float zAdd,
			uint[] depthBuffer,
			int offset,
			int[] framebuffer,
			TextureMipMapLevel texture,
			int[] texels)
		{
			var textureWidth = texture.Width;
			var textureHeight = texture.Height;

			float u;
			float v;
			float z;

			var dx = 1f / (x1 - (int)x1);
			var iz = iz1; // +dx * dizdx;
			var uiz = uiz1; // +dx * duizdx;
			var viz = viz1; // +dx * dvizdx;

			var color = (uint)0xff000000;
			var colorAsInt = (int)color;

			var actualU = 0f;
			var actualV = 0f;

			for (var x = 0; x <= length; x++)
			{
				var bufferZ = (UInt32)((1.0f - iz) * (float)UInt32.MaxValue);
				if (bufferZ > depthBuffer[offset] &&
					zStart >= 0f &&
					zStart < 1f
					)
				{
					z = 1f / iz;
					u = uiz * z;
					v = viz * z;

					actualU = u * textureWidth;
					actualV = v * textureHeight;


					var intu = (int)(actualU) & (textureWidth - 1);
					var intv = (int)(actualV) & (textureHeight - 1);

					var texel = ((intv << texture.WidthBitCount) + intu);

					framebuffer[offset] = texels[texel] | colorAsInt;
					//framebuffer[offset] = Bilerp(texture, intu, intv, actualU, actualV);
					depthBuffer[offset] = bufferZ;
				}

				offset++;

				iz += dizdx;
				uiz += duizdx;
				viz += dvizdx;
			}
		}

		private static int redMask;
		private static int greenMask;
		private static int blueMask;
		private static int alphaFull;

		static TextureTrianglePerspectiveCorrected()
		{
			uint g = 0xff000000;
			greenMask = (int)g;
			redMask = 0x00ff0000;
			blueMask = 0x00ff0000;

			uint a = 0xff000000;
			alphaFull = (int)a;
		}

		private int Bilerp(TextureMipMapLevel map, int x, int y, float u, float v)
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

			var cr1 = map.Pixels[x, y];
			var cr2 = map.Pixels[rightOffset, y];
			var cr3 = map.Pixels[rightOffset, belowOffset];
			var cr4 = map.Pixels[x, belowOffset];

			var a = (0x100 - h) * (0x100 - i);
			var b = (0x000 + h) * (0x100 - i);
			var c = (0x000 + h) * (0x000 + i);
			var d = 65536 - a - b - c;

			int red = redMask & (((cr1 >> 16) * a) + ((cr2 >> 16) * b) + ((cr3 >> 16) * c) + ((cr4 >> 16) * d));
			int green = greenMask & (((cr1 & 0x0000ff00) * a) + ((cr2 & 0x000000ff00) * b) + ((cr3 & 0x0000ff00) * c) + ((cr4 & 0x0000ff00) * d));
			int blue = blueMask & (((cr1 & 0x000000ff) * a) + ((cr2 & 0x000000ff) * b) + ((cr3 & 0x000000ff) * c) + ((cr4 & 0x000000ff) * d));

			var pixel = red | (((green | blue) >> 16) & 0xffff) | alphaFull;
			return pixel;
		}


	}
}
#endif