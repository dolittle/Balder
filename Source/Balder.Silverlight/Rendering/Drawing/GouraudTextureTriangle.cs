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
using Balder.Core;
using Balder.Core.Materials;
using Balder.Core.Math;

namespace Balder.Silverlight.Rendering.Drawing
{
	public class GouraudTextureTriangle : Triangle
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

		public override void Draw(RenderFace face, RenderVertex[] vertices, UInt32 nodeIdentifier)
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


			IMap image = null;

			if (null != face.Material.DiffuseMap)
			{
				image = face.Material.DiffuseMap;

			}
			else if (null != face.Material.ReflectionMap)
			{
				image = face.Material.ReflectionMap;

				SetSphericalEnvironmentMapTextureCoordinate(vertexA);
				SetSphericalEnvironmentMapTextureCoordinate(vertexB);
				SetSphericalEnvironmentMapTextureCoordinate(vertexC);
			}
			if (null == image)
			{
				return;
			}
			var texels = image.GetPixelsAs32BppARGB();



			GetSortedPoints(face, ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var za = vertexA.DepthBufferAdjustedZ;
			var ua = vertexA.U * image.Width;
			var va = vertexA.V * image.Height;
			var ra = ((float)face.CalculatedColorA.Red) / 255f;
			var ga = ((float)face.CalculatedColorA.Green) / 255f;
			var ba = ((float)face.CalculatedColorA.Blue) / 255f;
			var aa = ((float)face.CalculatedColorA.Alpha) / 255f;

			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var zb = vertexB.DepthBufferAdjustedZ;
			var ub = vertexB.U * image.Width;
			var vb = vertexB.V * image.Height;
			var rb = ((float)face.CalculatedColorB.Red) / 255f;
			var gb = ((float)face.CalculatedColorB.Green) / 255f;
			var bb = ((float)face.CalculatedColorB.Blue) / 255f;
			var ab = ((float)face.CalculatedColorB.Alpha) / 255f;


			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;
			var zc = vertexC.DepthBufferAdjustedZ;
			var uc = vertexC.U * image.Width;
			var vc = vertexC.V * image.Height;
			var rc = ((float)face.CalculatedColorC.Red) / 255f;
			var gc = ((float)face.CalculatedColorC.Green) / 255f;
			var bc = ((float)face.CalculatedColorC.Blue) / 255f;
			var ac = ((float)face.CalculatedColorC.Alpha) / 255f;


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

			var deltaR1 = rb - ra;
			var deltaR2 = rc - rb;
			var deltaR3 = rc - ra;

			var deltaG1 = gb - ga;
			var deltaG2 = gc - gb;
			var deltaG3 = gc - ga;

			var deltaB1 = bb - ba;
			var deltaB2 = bc - bb;
			var deltaB3 = bc - ba;

			var deltaA1 = ab - aa;
			var deltaA2 = ac - ab;
			var deltaA3 = ac - aa;

			var x1 = xa;
			var x2 = xa;

			var z1 = za;
			var z2 = za;

			var u1 = ua;
			var u2 = ua;

			var v1 = va;
			var v2 = va;

			var r1 = ra;
			var r2 = ra;

			var g1 = ga;
			var g2 = ga;

			var b1 = ba;
			var b2 = ba;

			var a1 = aa;
			var a2 = aa;

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

			var rInterpolate1 = deltaR3 / deltaY3;
			var rInterpolate2 = deltaR1 / deltaY1;
			var rInterpolate3 = deltaR2 / deltaY2;

			var gInterpolate1 = deltaG3 / deltaY3;
			var gInterpolate2 = deltaG1 / deltaY1;
			var gInterpolate3 = deltaG2 / deltaY2;

			var bInterpolate1 = deltaB3 / deltaY3;
			var bInterpolate2 = deltaB1 / deltaY1;
			var bInterpolate3 = deltaB2 / deltaY2;

			var aInterpolate1 = deltaA3 / deltaY3;
			var aInterpolate2 = deltaA1 / deltaY1;
			var aInterpolate3 = deltaA2 / deltaY2;

			var framebuffer = BufferContainer.Framebuffer;
			var depthBuffer = BufferContainer.DepthBuffer;
			var nodeBuffer = BufferContainer.NodeBuffer;
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
				r1 = ra + rInterpolate1 * yClipTopAsFloat;
				g1 = ga + gInterpolate1 * yClipTopAsFloat;
				b1 = ba + bInterpolate1 * yClipTopAsFloat;
				a1 = aa + aInterpolate1 * yClipTopAsFloat;

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

					r2 = rb + (rInterpolate3 * ySecondClipTop);
					rInterpolate2 = rInterpolate3;

					g2 = gb + (gInterpolate3 * ySecondClipTop);
					gInterpolate2 = gInterpolate3;

					b2 = bb + (bInterpolate3 * ySecondClipTop);
					bInterpolate2 = bInterpolate3;

					a2 = ab + (aInterpolate3 * ySecondClipTop);
					aInterpolate2 = aInterpolate3;
				}
				else
				{
					x2 = xa + xInterpolate2 * yClipTopAsFloat;
					z2 = za + zInterpolate2 * yClipTopAsFloat;
					u2 = ua + uInterpolate2 * yClipTopAsFloat;
					v2 = va + vInterpolate2 * yClipTopAsFloat;
					r2 = ra + rInterpolate2 * yClipTopAsFloat;
					g2 = ga + gInterpolate2 * yClipTopAsFloat;
					b2 = ba + bInterpolate2 * yClipTopAsFloat;
					a2 = aa + aInterpolate2 * yClipTopAsFloat;
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

			var rStart = 0f;
			var rEnd = 0f;
			var rAdd = 0f;
			var gStart = 0f;
			var gEnd = 0f;
			var gAdd = 0f;
			var bStart = 0f;
			var bEnd = 0f;
			var bAdd = 0f;
			var aStart = 0f;
			var aEnd = 0f;
			var aAdd = 0f;


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

					rStart = r2;
					rEnd = r1;

					gStart = g2;
					gEnd = g1;

					bStart = b2;
					bEnd = b1;

					aStart = a2;
					aEnd = a1;
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

					rStart = r1;
					rEnd = r2;

					gStart = g1;
					gEnd = g2;

					bStart = b1;
					bEnd = b2;

					aStart = a1;
					aEnd = a2;
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
					rAdd = (rEnd - rStart) / lengthAsFloat;
					gAdd = (gEnd - gStart) / lengthAsFloat;
					bAdd = (bEnd - bStart) / lengthAsFloat;
					aAdd = (aEnd - aStart) / lengthAsFloat;

					if (xClipStartAsFloat > 0)
					{
						zStart += (zAdd * xClipStartAsFloat);
						uStart += (uAdd * xClipStartAsFloat);
						vStart += (vAdd * xClipStartAsFloat);
						rStart += (rAdd * xClipStartAsFloat);
						gStart += (gAdd * xClipStartAsFloat);
						bStart += (bAdd * xClipStartAsFloat);
						aStart += (aAdd * xClipStartAsFloat);
					}

					var rStartInt = ((int)(rStart * 255f)) << 8;
					var rAddInt = (int)(rAdd * 65535f);

					var gStartInt = ((int)(gStart * 255f)) << 8;
					var gAddInt = (int)(gAdd * 65535f);

					var bStartInt = ((int)(bStart * 255f)) << 8;
					var bAddInt = (int)(bAdd * 65535f);

					var aStartInt = ((int)(aStart * 255f)) << 8;
					var aAddInt = (int)(aAdd * 65535f);

					offset = yoffset + xStart;
					DrawSpan(length,
							 zStart,
							 zAdd,
							 uStart,
							 uAdd,
							 vStart,
							 vAdd,
							 rStartInt,
							 rAddInt,
							 gStartInt,
							 gAddInt,
							 bStartInt,
							 bAddInt,
							 aStartInt,
							 aAddInt,
							 depthBuffer,
							 offset,
							 framebuffer,
							 image,
							 texels,
							 nodeBuffer,
							 nodeIdentifier);
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

					r2 = rb;
					rInterpolate2 = rInterpolate3;

					g2 = gb;
					gInterpolate2 = gInterpolate3;

					b2 = bb;
					bInterpolate2 = bInterpolate3;

					a2 = ab;
					aInterpolate2 = aInterpolate3;
				}


				x1 += xInterpolate1;
				x2 += xInterpolate2;

				z1 += zInterpolate1;
				z2 += zInterpolate2;

				u1 += uInterpolate1;
				u2 += uInterpolate2;

				v1 += vInterpolate1;
				v2 += vInterpolate2;

				r1 += rInterpolate1;
				r2 += rInterpolate2;

				g1 += gInterpolate1;
				g2 += gInterpolate2;

				b1 += bInterpolate1;
				b2 += bInterpolate2;

				a1 += aInterpolate1;
				a2 += aInterpolate2;

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
			int rStart,
			int rAdd,
			int gStart,
			int gAdd,
			int bStart,
			int bAdd,
			int aStart,
			int aAdd,
			uint[] depthBuffer,
			int offset,
			int[] framebuffer,
			IMap image,
			int[] texels,
			UInt32[] nodeBuffer,
			UInt32 nodeIdentifier)
		{
			

			for (var x = 0; x <= length; x++)
			{
				var bufferZ = (UInt32)((1.0f - zStart) * (float)UInt32.MaxValue);
				if (bufferZ > depthBuffer[offset] &&
					zStart >= 0f &&
					zStart < 1f
					)
				{
					var intu = ((int)uStart) & (image.Width - 1);
					var intv = ((int)vStart) & (image.Height - 1);

					var texel = ((intv << image.WidthBitCount) + intu);

					var red = (uint)(rStart >> 8) & 0xff;
					var green = (uint)(gStart >> 8) & 0xff;
					var blue = (uint)(bStart >> 8) & 0xff;
					//var alpha = (uint)(aStart >> 8) & 0xff;

					uint colorAsInt = 0xff000000 |
									  (red << 16) |
									  (green << 8) |
									  blue;


					framebuffer[offset] = Cluts.Multiply(texels[texel],(int)colorAsInt);
					depthBuffer[offset] = bufferZ;
					nodeBuffer[offset] = nodeIdentifier;
				}

				offset++;
				zStart += zAdd;
				uStart += uAdd;
				vStart += vAdd;
				rStart += rAdd;
				gStart += gAdd;
				bStart += bAdd;
				aStart += aAdd;
			}
		}

	}
}