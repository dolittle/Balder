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


			TextureMipMapLevel texture = null;
			if( null != face.DiffuseTexture )
			{
				texture = face.DiffuseTexture.FullDetailLevel;
			} else if( null != face.ReflectionTexture )
			{
				texture = face.ReflectionTexture.FullDetailLevel;
				SetSphericalEnvironmentMapTextureCoordinate(vertexA);
				SetSphericalEnvironmentMapTextureCoordinate(vertexB);
				SetSphericalEnvironmentMapTextureCoordinate(vertexC);
			}

			var texels = texture.OriginalPixels;

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

					//var texel = ((intv << texture.WidthBitCount) + intu);

					framebuffer[offset] = Bilerp(texture, intu, intv, uStart, vStart);
					depthBuffer[offset] = bufferZ;
				}

				offset++;
				zStart += zAdd;
				uStart += uAdd;
				vStart += vAdd;
			}
		}

		private int Bilerp(TextureMipMapLevel map, int x, int y, float u, float v)
		{
			var deltaX = ((int)((u - x) * 255f)) & 0xff;
			var deltaY = ((int)((v - y) * 255f)) & 0xff;
			
			var inverseDeltaX = 0xff - deltaX;
			var inverseDeltaY = 0xff - deltaY;

			int rightPixel;
			if (x < map.Width - 1)
			{
				rightPixel = map.Pixels[x + 1,y];
			}
			else
			{
				rightPixel = map.Pixels[x,y];
			}


			int belowPixel;
			if (y < map.Height - 1)
			{
				belowPixel = map.Pixels[x, y + 1];
			}
			else
			{
				belowPixel = map.Pixels[x, y];
			}

			int belowRightPixel;
			if (y < map.Height - 1 && x < map.Width - 1)
			{
				belowRightPixel = map.Pixels[x + 1, y + 1];
			}
			else
			{
				belowRightPixel = map.Pixels[x, y];
			}


			var pixel = map.Pixels[x, y];
			var currentAlpha = ((pixel >> 24) & 0xff);
			var currentRed = ((pixel >> 16) & 0xff);
			var currentGreen = ((pixel >> 8) & 0xff);
			var currentBlue = ((pixel) & 0xff);
			

			var rightAlpha = ((rightPixel >> 24) & 0xff);
			var rightRed = ((rightPixel >> 16) & 0xff);
			var rightGreen = ((rightPixel >> 8) & 0xff);
			var rightBlue = ((rightPixel) & 0xff);


			var belowAlpha = ((belowPixel >> 24) & 0xff);
			var belowRed = ((belowPixel >> 16) & 0xff);
			var belowGreen = ((belowPixel >> 8) & 0xff);
			var belowBlue = ((belowPixel) & 0xff);

			var belowRightAlpha = ((belowRightPixel >> 24) & 0xff);
			var belowRightRed = ((belowRightPixel >> 16) & 0xff);
			var belowRightGreen = ((belowRightPixel>> 8) & 0xff);
			var belowRightBlue = ((belowRightPixel) & 0xff);

			
			var multipliedCurrentRed = Cluts.Multiply(Cluts.Multiply(currentRed, inverseDeltaY), inverseDeltaX);
			var multipliedBelowRed = Cluts.Multiply(Cluts.Multiply(belowRed, deltaY), inverseDeltaX);
			var multipliedRightRed = Cluts.Multiply(Cluts.Multiply(rightRed, inverseDeltaY), deltaX);
			var multipliedBelowRightRed = Cluts.Multiply(Cluts.Multiply(belowRightRed, deltaY), deltaX);
			var red = Cluts.Add(
				Cluts.Add(multipliedCurrentRed, multipliedBelowRed),
				Cluts.Add(multipliedRightRed, multipliedBelowRightRed));
			
			var multipliedCurrentGreen = Cluts.Multiply(Cluts.Multiply(currentGreen, inverseDeltaY), inverseDeltaX);
			var multipliedBelowGreen = Cluts.Multiply(Cluts.Multiply(belowGreen, deltaY), inverseDeltaX);
			var multipliedRightGreen = Cluts.Multiply(Cluts.Multiply(rightGreen, inverseDeltaY), deltaX);
			var multipliedBelowRightGreen = Cluts.Multiply(Cluts.Multiply(belowRightGreen, deltaY), deltaX);
			var green = Cluts.Add(
				Cluts.Add(multipliedCurrentGreen, multipliedBelowGreen),
				Cluts.Add(multipliedRightGreen, multipliedBelowRightGreen));

			var multipliedCurrentBlue = Cluts.Multiply(Cluts.Multiply(currentBlue, inverseDeltaY), inverseDeltaX);
			var multipliedBelowBlue = Cluts.Multiply(Cluts.Multiply(belowBlue, deltaY), inverseDeltaX);
			var multipliedRightBlue = Cluts.Multiply(Cluts.Multiply(rightBlue, inverseDeltaY), deltaX);
			var multipliedBelowRightBlue = Cluts.Multiply(Cluts.Multiply(belowRightBlue, deltaY), deltaX);
			var blue = Cluts.Add(
				Cluts.Add(multipliedCurrentBlue, multipliedBelowBlue),
				Cluts.Add(multipliedRightBlue, multipliedBelowRightBlue));



			return Cluts.Compose(red, green, blue, 0xff);
		}

	}
}
#endif