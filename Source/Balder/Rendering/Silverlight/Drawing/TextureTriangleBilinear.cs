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
	public class TextureTriangleBilinear : Triangle
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


			TextureMipMapLevel texture = null;
			if (null != face.Texture1)
			{
				texture = face.Texture1.FullDetailLevel;
			}
			else if (null != face.Texture2)
			{
				texture = face.Texture2.FullDetailLevel;
				SetSphericalEnvironmentMapTextureCoordinate(vertexA);
				SetSphericalEnvironmentMapTextureCoordinate(vertexB);
				SetSphericalEnvironmentMapTextureCoordinate(vertexC);
			}


			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var za = vertexA.DepthBufferAdjustedZ;
			var ua = vertexA.U * texture.Width;
			var va = vertexA.V * texture.Height;

			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var zb = vertexB.DepthBufferAdjustedZ;
			var ub = vertexB.U * texture.Width;
			var vb = vertexB.V * texture.Height;


			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;
			var zc = vertexC.DepthBufferAdjustedZ;
			var uc = vertexC.U * texture.Width;
			var vc = vertexC.V * texture.Height;


			var deltaX1 = xb - xa;
			var deltaX2 = xc - xb;
			var deltaX3 = xc - xa;

			var deltaY1 = yb - ya;
			var deltaY2 = yc - yb;
			var deltaY3 = yc - ya;

			var deltaZ1 = zb - za;
			var deltaZ2 = zc - zb;
			var deltaZ3 = zc - za;

			var deltaU1 = ub - ua;
			var deltaU2 = uc - ub;
			var deltaU3 = uc - ua;

			var deltaV1 = vb - va;
			var deltaV2 = vc - vb;
			var deltaV3 = vc - va;

			var x1 = xa;
			var x2 = xa;

			var z1 = za;
			var z2 = za;

			var u1 = ua;
			var u2 = ua;

			var v1 = va;
			var v2 = va;

			var xInterpolate1 = deltaX3 / deltaY3;
			var xInterpolate2 = deltaX1 / deltaY1;
			var xInterpolate3 = deltaX2 / deltaY2;

			var zInterpolate1 = deltaZ3 / deltaY3;
			var zInterpolate2 = deltaZ1 / deltaY1;
			var zInterpolate3 = deltaZ2 / deltaY2;

			var uInterpolate1 = deltaU3 / deltaY3;
			var uInterpolate2 = deltaU1 / deltaY1;
			var uInterpolate3 = deltaU2 / deltaY2;

			var vInterpolate1 = deltaV3 / deltaY3;
			var vInterpolate2 = deltaV1 / deltaY1;
			var vInterpolate3 = deltaV2 / deltaY2;

			var framebuffer = BufferContainer.Framebuffer;
			var depthBuffer = BufferContainer.DepthBuffer;
			var frameBufferWidth = BufferContainer.Width;
			var frameBufferHeight = BufferContainer.Height;

			var yStart = (int)ya;
			var yEnd = (int)yc;
			var yClipTop = 0;

			if (yStart < 0)
			{
				yClipTop = -yStart;
				yStart = 0;
			}

			if (yEnd >= frameBufferHeight)
			{
				yEnd = frameBufferHeight - 1;
			}

			var height = yEnd - yStart;
			if (height == 0)
			{
				return;
			}

			if (yClipTop > 0)
			{
				var yClipTopAsFloat = (float)yClipTop;
				x1 = xa + xInterpolate1 * yClipTopAsFloat;
				z1 = za + zInterpolate1 * yClipTopAsFloat;
				u1 = ua + uInterpolate1 * yClipTopAsFloat;
				v1 = va + vInterpolate1 * yClipTopAsFloat;

				if (yb < 0)
				{
					var ySecondClipTop = -yb;

					x2 = xb + (xInterpolate3 * ySecondClipTop);
					xInterpolate2 = xInterpolate3;

					z2 = zb + (zInterpolate3 * ySecondClipTop);
					zInterpolate2 = zInterpolate3;

					u2 = ub + (uInterpolate3 * ySecondClipTop);
					uInterpolate2 = uInterpolate3;

					v2 = vb + (vInterpolate3 * ySecondClipTop);
					vInterpolate2 = vInterpolate3;

				}
				else
				{
					x2 = xa + xInterpolate2 * yClipTopAsFloat;
					z2 = za + zInterpolate2 * yClipTopAsFloat;
					u2 = ua + uInterpolate2 * yClipTopAsFloat;
					v2 = va + vInterpolate2 * yClipTopAsFloat;
				}
			}

			var yoffset = BufferContainer.Width * yStart;

			var offset = 0;
			var length = 0;
			var originalLength = 0;

			var xStart = 0;
			var xEnd = 0;

			var zStart = 0f;
			var zEnd = 0f;
			var zAdd = 0f;

			var xClipStart = 0;

			var uStart = 0f;
			var uEnd = 0f;
			var uAdd = 0f;

			var vStart = 0f;
			var vEnd = 0f;
			var vAdd = 0f;


			for (var y = yStart; y <= yEnd; y++)
			{
				if (x2 < x1)
				{
					xStart = (int)x2;
					xEnd = (int)x1;

					zStart = z2;
					zEnd = z1;

					uStart = u2;
					uEnd = u1;

					vStart = v2;
					vEnd = v1;
				}
				else
				{
					offset = yoffset + (int)x1;

					xStart = (int)x1;
					xEnd = (int)x2;

					zStart = z1;
					zEnd = z2;

					uStart = u1;
					uEnd = u2;

					vStart = v1;
					vEnd = v2;
				}
				originalLength = xEnd - xStart;

				if (xStart < 0)
				{
					xClipStart = -xStart;
					xStart = 0;
				}
				if (xEnd >= frameBufferWidth)
				{
					xEnd = frameBufferWidth - 1;
				}


				length = xEnd - xStart;

				if (length != 0)
				{
					var xClipStartAsFloat = (float)xClipStart;
					var lengthAsFloat = (float)originalLength;
					zAdd = (zEnd - zStart) / lengthAsFloat;
					uAdd = (uEnd - uStart) / lengthAsFloat;
					vAdd = (vEnd - vStart) / lengthAsFloat;

					if (xClipStartAsFloat > 0)
					{
						zStart += (zAdd * xClipStartAsFloat);
						uStart += (uAdd * xClipStartAsFloat);
						vStart += (vAdd * xClipStartAsFloat);
					}

					offset = yoffset + xStart;
					DrawSpan(length,
							 zStart,
							 zAdd,
							 uStart,
							 uAdd,
							 vStart,
							 vAdd,
							 depthBuffer,
							 offset,
							 framebuffer,
							 texture);
				}

				if (y == (int)yb)
				{
					x2 = xb;
					xInterpolate2 = xInterpolate3;

					z2 = zb;
					zInterpolate2 = zInterpolate3;

					u2 = ub;
					uInterpolate2 = uInterpolate3;

					v2 = vb;
					vInterpolate2 = vInterpolate3;
				}


				x1 += xInterpolate1;
				x2 += xInterpolate2;

				z1 += zInterpolate1;
				z2 += zInterpolate2;

				u1 += uInterpolate1;
				u2 += uInterpolate2;

				v1 += vInterpolate1;
				v2 += vInterpolate2;

				yoffset += BufferContainer.Width;
			}

		}

		protected virtual void DrawSpan(
			int length,
			float zStart,
			float zAdd,
			float uStart,
			float uAdd,
			float vStart,
			float vAdd,
			uint[] depthBuffer,
			int offset,
			int[] framebuffer,
			TextureMipMapLevel texture)
		{
			for (var x = 0; x <= length; x++)
			{
				var bufferZ = (UInt32)((1.0f - zStart) * (float)UInt32.MaxValue);
				if (bufferZ > depthBuffer[offset] &&
					zStart >= 0f &&
					zStart < 1f
					)
				{
					var intu = ((int)uStart) & (texture.Width - 1);
					var intv = ((int)vStart) & (texture.Height - 1);

					framebuffer[offset] = Bilerp(texture, intu, intv, uStart, vStart);
					depthBuffer[offset] = bufferZ;
				}

				offset++;
				zStart += zAdd;
				uStart += uAdd;
				vStart += vAdd;
			}
		}


		private static int redMask;
		private static int greenMask;
		private static int blueMask;
		private static int alphaFull;

		static TextureTriangleBilinear()
		{
			uint g = 0xff000000;
			greenMask = (int)g;
			redMask = 0x00ff0000;
			blueMask = 0x00ff0000;

			uint a = 0xff000000;
			alphaFull = (int) a;
		}


		// http://cboard.cprogramming.com/game-programming/19926-super-fast-bilinear-interpolation.html

		// http://en.wikipedia.org/wiki/Bilinear_filtering

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

			var cr1 = map.Pixels[x, y]| alphaFull;
			var cr2 = map.Pixels[rightOffset, y] | alphaFull;
			var cr3 = map.Pixels[rightOffset, belowOffset] | alphaFull;
			var cr4 = map.Pixels[x, belowOffset] | alphaFull;

			var a = (0x100 - h) * (0x100 - i);
			var b = (0x000 + h) * (0x100 - i);
			var c = (0x000 + h) * (0x000 + i);
			var d = 65536 - a - b - c;

			int red = redMask & (((cr1 >> 16)*a) + ((cr2 >> 16)*b) + ((cr3 >> 16)*c) + ((cr4 >> 16)*d));
			int green = greenMask & (((cr1 & 0x0000ff00) * a) + ((cr2 & 0x000000ff00) * b) + ((cr3 & 0x0000ff00) * c) + ((cr4 & 0x0000ff00) * d));
			int blue = blueMask & (((cr1 & 0x000000ff)*a) + ((cr2 & 0x000000ff)*b) + ((cr3 & 0x000000ff)*c) + ((cr4 & 0x000000ff)*d));

			var pixel = red | (((green | blue)>>16)&0xffff) | alphaFull ;
			return pixel;
		}




	}
}
#endif