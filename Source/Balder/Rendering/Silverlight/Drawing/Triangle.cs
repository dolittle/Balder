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
#if(SILVERLIGHT)
using System;
using Balder.Display;
using Balder.Math;


namespace Balder.Rendering.Silverlight.Drawing
{
	public abstract class Triangle
	{
		protected int[] Framebuffer;
		protected uint[] DepthBuffer;

		protected int Opacity;

		protected TextureMipMapLevel Texture1;
		protected TextureMipMapLevel Texture2;
		protected int Texture1Factor;
		protected int Texture2Factor;

		protected int PixelOffset;

		protected int X1Int;
		protected int X2Int;

		protected int Y1Int;
		protected int Y2Int;

		protected float X1;
		protected float X2;

		protected float Y1;
		protected float Y2;

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

		protected float DiffuseR1;
		protected float DiffuseG1;
		protected float DiffuseB1;
		protected float DiffuseA1;

		protected float DiffuseR2;
		protected float DiffuseG2;
		protected float DiffuseB2;
		protected float DiffuseA2;

		protected float DiffuseRInterpolate1;
		protected float DiffuseGInterpolate1;
		protected float DiffuseBInterpolate1;
		protected float DiffuseAInterpolate1;

		protected float DiffuseRInterpolate2;
		protected float DiffuseGInterpolate2;
		protected float DiffuseBInterpolate2;
		protected float DiffuseAInterpolate2;

		protected float SpecularR1;
		protected float SpecularG1;
		protected float SpecularB1;
		protected float SpecularA1;

		protected float SpecularR2;
		protected float SpecularG2;
		protected float SpecularB2;
		protected float SpecularA2;

		protected float SpecularRInterpolate1;
		protected float SpecularGInterpolate1;
		protected float SpecularBInterpolate1;
		protected float SpecularAInterpolate1;

		protected float SpecularRInterpolate2;
		protected float SpecularGInterpolate2;
		protected float SpecularBInterpolate2;
		protected float SpecularAInterpolate2;



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

		protected float DiffuseRScanline;
		protected float DiffuseGScanline;
		protected float DiffuseBScanline;
		protected float DiffuseAScanline;
		protected float DiffuseRScanlineInterpolate;
		protected float DiffuseGScanlineInterpolate;
		protected float DiffuseBScanlineInterpolate;
		protected float DiffuseAScanlineInterpolate;

		protected float SpecularRScanline;
		protected float SpecularGScanline;
		protected float SpecularBScanline;
		protected float SpecularAScanline;
		protected float SpecularRScanlineInterpolate;
		protected float SpecularGScanlineInterpolate;
		protected float SpecularBScanlineInterpolate;
		protected float SpecularAScanlineInterpolate;


		protected int ColorAsInt;
		protected int DiffuseAsInt;
		protected int SpecularAsInt;
		protected int MaterialAmbientAsInt;
		protected int MaterialDiffuseAsInt;

		protected static int RedMask;
		protected static int GreenMask;
		protected static int BlueMask;

		protected float Near;
		protected float Far;
		protected float DepthMultiplier;
		protected float ZMultiplier;

		static Triangle()
		{
			uint g = 0xff000000;
			GreenMask = (int)g;
			RedMask = 0x00ff0000;
			BlueMask = 0x00ff0000;
		}

		protected void SetSphericalEnvironmentMapTextureCoordinate(RenderVertex vertex)
		{
			var u = vertex.TransformedVectorNormalized;
			var n = vertex.TransformedNormal;
			var r = Vector.Reflect(n, u);
			var m = MathHelper.Sqrt((r.X * r.X) + (r.Y * r.Y) +
									((r.Z + 0f) * (r.Z + 0f)));

			var m1 = 1f / m;
			var s = (r.X * m1);
			var t = (r.Y * m1);

			if (null == Texture2)
			{
				vertex.U1 = -(s * 0.5f) + 0.5f;
				vertex.V1 = (t * 0.5f) + 0.5f;

			}
			else
			{
				vertex.U2 = -(s * 0.5f) + 0.5f;
				vertex.V2 = (t * 0.5f) + 0.5f;

			}
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


		public virtual void Draw(Viewport viewport, RenderFace face, RenderVertex vertexA, RenderVertex vertexB, RenderVertex vertexC)
		{
			Opacity = face.Opacity;

			Near = viewport.View.Near;
			Far = viewport.View.Far;
			DepthMultiplier = viewport.View.DepthMultiplier;
			ZMultiplier = DepthMultiplier * (float)UInt32.MaxValue;

			Texture1 = null;
			if (null != face.Texture1 && face.Texture1Factor > 0)
			{
				Texture1 = face.Texture1.FullDetailLevel;
			}

			if (null != face.Texture2)
			{
				Texture2 = face.Texture2.FullDetailLevel;

				if (null == Texture1)
				{
					Texture1 = Texture2;
					Texture2 = null;
				}


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



			var xa = vertexA.ProjectedVector.X + 0.5f;
			var ya = vertexA.ProjectedVector.Y + 0.5f;
			var za = vertexA.ProjectedVector.Z;
			var u1a = vertexA.U1 * texture1Width;
			var v1a = vertexA.V1 * texture1Height;
			var u2a = vertexA.U2 * texture2Width;
			var v2a = vertexA.V2 * texture2Height;
			var ra = ((float)vertexA.CalculatedColor.Red) / 255f;
			var ga = ((float)vertexA.CalculatedColor.Green) / 255f;
			var ba = ((float)vertexA.CalculatedColor.Blue) / 255f;
			var aa = ((float)vertexA.CalculatedColor.Alpha) / 255f;
			var diffuseRa = ((float)vertexA.DiffuseColor.Red) / 255f;
			var diffuseGa = ((float)vertexA.DiffuseColor.Green) / 255f;
			var diffuseBa = ((float)vertexA.DiffuseColor.Blue) / 255f;
			var diffuseAa = ((float)vertexA.DiffuseColor.Alpha) / 255f;
			var specularRa = ((float)vertexA.SpecularColor.Red) / 255f;
			var specularGa = ((float)vertexA.SpecularColor.Green) / 255f;
			var specularBa = ((float)vertexA.SpecularColor.Blue) / 255f;
			var specularAa = ((float)vertexA.SpecularColor.Alpha) / 255f;

			var xb = vertexB.ProjectedVector.X + 0.5f;
			var yb = vertexB.ProjectedVector.Y + 0.5f;
			var zb = vertexB.ProjectedVector.Z;
			var u1b = vertexB.U1 * texture1Width;
			var v1b = vertexB.V1 * texture1Height;
			var u2b = vertexB.U2 * texture2Width;
			var v2b = vertexB.V2 * texture2Height;
			var rb = ((float)vertexB.CalculatedColor.Red) / 255f;
			var gb = ((float)vertexB.CalculatedColor.Green) / 255f;
			var bb = ((float)vertexB.CalculatedColor.Blue) / 255f;
			var ab = ((float)vertexB.CalculatedColor.Alpha) / 255f;
			var diffuseRb = ((float)vertexB.DiffuseColor.Red) / 255f;
			var diffuseGb = ((float)vertexB.DiffuseColor.Green) / 255f;
			var diffuseBb = ((float)vertexB.DiffuseColor.Blue) / 255f;
			var diffuseAb = ((float)vertexB.DiffuseColor.Alpha) / 255f;
			var specularRb = ((float)vertexB.SpecularColor.Red) / 255f;
			var specularGb = ((float)vertexB.SpecularColor.Green) / 255f;
			var specularBb = ((float)vertexB.SpecularColor.Blue) / 255f;
			var specularAb = ((float)vertexB.SpecularColor.Alpha) / 255f;

			var xc = vertexC.ProjectedVector.X + 0.5f;
			var yc = vertexC.ProjectedVector.Y + 0.5f;
			var zc = vertexC.ProjectedVector.Z;
			var u1c = vertexC.U1 * texture1Width;
			var v1c = vertexC.V1 * texture1Height;
			var u2c = vertexC.U2 * texture2Width;
			var v2c = vertexC.V2 * texture2Height;
			var rc = ((float)vertexC.CalculatedColor.Red) / 255f;
			var gc = ((float)vertexC.CalculatedColor.Green) / 255f;
			var bc = ((float)vertexC.CalculatedColor.Blue) / 255f;
			var ac = ((float)vertexC.CalculatedColor.Alpha) / 255f;
			var diffuseRc = ((float)vertexC.DiffuseColor.Red) / 255f;
			var diffuseGc = ((float)vertexC.DiffuseColor.Green) / 255f;
			var diffuseBc = ((float)vertexC.DiffuseColor.Blue) / 255f;
			var diffuseAc = ((float)vertexC.DiffuseColor.Alpha) / 255f;
			var specularRc = ((float)vertexC.SpecularColor.Red) / 255f;
			var specularGc = ((float)vertexC.SpecularColor.Green) / 255f;
			var specularBc = ((float)vertexC.SpecularColor.Blue) / 255f;
			var specularAc = ((float)vertexC.SpecularColor.Alpha) / 255f;

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

			var deltaDiffuseRA = diffuseRb - diffuseRa;
			var deltaDiffuseRB = diffuseRc - diffuseRa;
			var deltaDiffuseRC = diffuseRc - diffuseRb;

			var deltaDiffuseGA = diffuseGb - diffuseGa;
			var deltaDiffuseGB = diffuseGc - diffuseGa;
			var deltaDiffuseGC = diffuseGc - diffuseGb;

			var deltaDiffuseBA = diffuseBb - diffuseBa;
			var deltaDiffuseBB = diffuseBc - diffuseBa;
			var deltaDiffuseBC = diffuseBc - diffuseBb;

			var deltaDiffuseAA = diffuseAb - diffuseAa;
			var deltaDiffuseAB = diffuseAc - diffuseAa;
			var deltaDiffuseAC = diffuseAc - diffuseAb;

			var deltaSpecularRA = specularRb - specularRa;
			var deltaSpecularRB = specularRc - specularRa;
			var deltaSpecularRC = specularRc - specularRb;

			var deltaSpecularGA = specularGb - specularGa;
			var deltaSpecularGB = specularGc - specularGa;
			var deltaSpecularGC = specularGc - specularGb;

			var deltaSpecularBA = specularBb - specularBa;
			var deltaSpecularBB = specularBc - specularBa;
			var deltaSpecularBC = specularBc - specularBb;

			var deltaSpecularAA = specularAb - specularAa;
			var deltaSpecularAB = specularAc - specularAa;
			var deltaSpecularAC = specularAc - specularAb;

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

			var diffuseRInterpolateA = deltaDiffuseRA / deltaYA;
			var diffuseRInterpolateB = deltaDiffuseRB / deltaYB;
			var diffuseRInterpolateC = deltaDiffuseRC / deltaYC;

			var diffuseGInterpolateA = deltaDiffuseGA / deltaYA;
			var diffuseGInterpolateB = deltaDiffuseGB / deltaYB;
			var diffuseGInterpolateC = deltaDiffuseGC / deltaYC;

			var diffuseBInterpolateA = deltaDiffuseBA / deltaYA;
			var diffuseBInterpolateB = deltaDiffuseBB / deltaYB;
			var diffuseBInterpolateC = deltaDiffuseBC / deltaYC;

			var diffuseAInterpolateA = deltaDiffuseAA / deltaYA;
			var diffuseAInterpolateB = deltaDiffuseAB / deltaYB;
			var diffuseAInterpolateC = deltaDiffuseAC / deltaYC;


			var specularRInterpolateA = deltaSpecularRA / deltaYA;
			var specularRInterpolateB = deltaSpecularRB / deltaYB;
			var specularRInterpolateC = deltaSpecularRC / deltaYC;

			var specularGInterpolateA = deltaSpecularGA / deltaYA;
			var specularGInterpolateB = deltaSpecularGB / deltaYB;
			var specularGInterpolateC = deltaSpecularGC / deltaYC;

			var specularBInterpolateA = deltaSpecularBA / deltaYA;
			var specularBInterpolateB = deltaSpecularBB / deltaYB;
			var specularBInterpolateC = deltaSpecularBC / deltaYC;

			var specularAInterpolateA = deltaSpecularAA / deltaYA;
			var specularAInterpolateB = deltaSpecularAB / deltaYB;
			var specularAInterpolateC = deltaSpecularAC / deltaYC;


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

			ColorAsInt = face.ColorAsInt;
			DiffuseAsInt = face.DiffuseAsInt;
			SpecularAsInt = face.SpecularAsInt;
			MaterialAmbientAsInt = face.MaterialAmbientAsInt;
			MaterialDiffuseAsInt = face.MaterialDiffuseAsInt;


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

				DiffuseRInterpolate1 = diffuseRInterpolateB;
				DiffuseGInterpolate1 = diffuseGInterpolateB;
				DiffuseBInterpolate1 = diffuseBInterpolateB;
				DiffuseAInterpolate1 = diffuseAInterpolateB;

				SpecularRInterpolate1 = specularRInterpolateB;
				SpecularGInterpolate1 = specularGInterpolateB;
				SpecularBInterpolate1 = specularBInterpolateB;
				SpecularAInterpolate1 = specularAInterpolateB;

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

				DiffuseR1 = diffuseRa + subPixelY * DiffuseRInterpolate1;
				DiffuseG1 = diffuseGa + subPixelY * DiffuseGInterpolate1;
				DiffuseB1 = diffuseBa + subPixelY * DiffuseBInterpolate1;
				DiffuseA1 = diffuseAa + subPixelY * DiffuseAInterpolate1;

				SpecularR1 = specularRa + subPixelY * SpecularRInterpolate1;
				SpecularG1 = specularGa + subPixelY * SpecularGInterpolate1;
				SpecularB1 = specularBa + subPixelY * SpecularBInterpolate1;
				SpecularA1 = specularAa + subPixelY * SpecularAInterpolate1;

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

					DiffuseR2 = diffuseRa + subPixelY * diffuseRInterpolateA;
					DiffuseG2 = diffuseGa + subPixelY * diffuseGInterpolateA;
					DiffuseB2 = diffuseBa + subPixelY * diffuseBInterpolateA;
					DiffuseA2 = diffuseAa + subPixelY * diffuseAInterpolateA;

					SpecularR2 = specularRa + subPixelY * specularRInterpolateA;
					SpecularG2 = specularGa + subPixelY * specularGInterpolateA;
					SpecularB2 = specularBa + subPixelY * specularBInterpolateA;
					SpecularA2 = specularAa + subPixelY * specularAInterpolateA;

					XInterpolate2 = xInterpolateA;
					RInterpolate2 = rInterpolateA;
					GInterpolate2 = gInterpolateA;
					BInterpolate2 = bInterpolateA;
					AInterpolate2 = aInterpolateA;

					DiffuseRInterpolate2 = diffuseRInterpolateA;
					DiffuseGInterpolate2 = diffuseGInterpolateA;
					DiffuseBInterpolate2 = diffuseBInterpolateA;
					DiffuseAInterpolate2 = diffuseAInterpolateA;

					SpecularRInterpolate2 = specularRInterpolateA;
					SpecularGInterpolate2 = specularGInterpolateA;
					SpecularBInterpolate2 = specularBInterpolateA;
					SpecularAInterpolate2 = specularAInterpolateA;

					Y1Int = yaInt;
					Y2Int = ybInt;

					Y1 = ya;
					Y2 = yb;

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

					DiffuseR2 = diffuseRb + subPixelY * diffuseRInterpolateC;
					DiffuseG2 = diffuseGb + subPixelY * diffuseGInterpolateC;
					DiffuseB2 = diffuseBb + subPixelY * diffuseBInterpolateC;
					DiffuseA2 = diffuseAb + subPixelY * diffuseAInterpolateC;

					SpecularR2 = specularRb + subPixelY * specularRInterpolateC;
					SpecularG2 = specularGb + subPixelY * specularGInterpolateC;
					SpecularB2 = specularBb + subPixelY * specularBInterpolateC;
					SpecularA2 = specularAb + subPixelY * specularAInterpolateC;

					XInterpolate2 = xInterpolateC;
					RInterpolate2 = rInterpolateC;
					GInterpolate2 = gInterpolateC;
					BInterpolate2 = bInterpolateC;
					AInterpolate2 = aInterpolateC;

					DiffuseRInterpolate2 = diffuseRInterpolateC;
					DiffuseGInterpolate2 = diffuseGInterpolateC;
					DiffuseBInterpolate2 = diffuseBInterpolateC;
					DiffuseAInterpolate2 = diffuseAInterpolateC;

					SpecularRInterpolate2 = specularRInterpolateC;
					SpecularGInterpolate2 = specularGInterpolateC;
					SpecularBInterpolate2 = specularBInterpolateC;
					SpecularAInterpolate2 = specularAInterpolateC;

					Y1Int = ybInt;
					Y2Int = ycInt;

					Y1 = yb;
					Y2 = yc;

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

				DiffuseRInterpolate2 = diffuseRInterpolateB;
				DiffuseGInterpolate2 = diffuseGInterpolateB;
				DiffuseBInterpolate2 = diffuseBInterpolateB;
				DiffuseAInterpolate2 = diffuseAInterpolateB;

				SpecularRInterpolate2 = specularRInterpolateB;
				SpecularGInterpolate2 = specularGInterpolateB;
				SpecularBInterpolate2 = specularBInterpolateB;
				SpecularAInterpolate2 = specularAInterpolateB;

				X2 = xa + subPixelY * xInterpolateB;
				R2 = ra + subPixelY * rInterpolateB;
				G2 = ga + subPixelY * gInterpolateB;
				B2 = ba + subPixelY * bInterpolateB;
				A2 = aa + subPixelY * aInterpolateB;

				DiffuseR2 = diffuseRa + subPixelY * diffuseRInterpolateB;
				DiffuseG2 = diffuseGa + subPixelY * diffuseGInterpolateB;
				DiffuseB2 = diffuseBa + subPixelY * diffuseBInterpolateB;
				DiffuseA2 = diffuseAa + subPixelY * diffuseAInterpolateB;

				SpecularR2 = specularRa + subPixelY * specularRInterpolateB;
				SpecularG2 = specularGa + subPixelY * specularGInterpolateB;
				SpecularB2 = specularBa + subPixelY * specularBInterpolateB;
				SpecularA2 = specularAa + subPixelY * specularAInterpolateB;

				if (yaInt < ybInt)
				{
					XInterpolate1 = xInterpolateA;
					RInterpolate1 = rInterpolateA;
					GInterpolate1 = gInterpolateA;
					BInterpolate1 = bInterpolateA;
					AInterpolate1 = aInterpolateA;

					DiffuseRInterpolate1 = diffuseRInterpolateA;
					DiffuseGInterpolate1 = diffuseGInterpolateA;
					DiffuseBInterpolate1 = diffuseBInterpolateA;
					DiffuseAInterpolate1 = diffuseAInterpolateA;

					SpecularRInterpolate1 = specularRInterpolateA;
					SpecularGInterpolate1 = specularGInterpolateA;
					SpecularBInterpolate1 = specularBInterpolateA;
					SpecularAInterpolate1 = specularAInterpolateA;

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

					DiffuseR1 = diffuseRa + subPixelY * DiffuseRInterpolate1;
					DiffuseG1 = diffuseGa + subPixelY * DiffuseGInterpolate1;
					DiffuseB1 = diffuseBa + subPixelY * DiffuseBInterpolate1;
					DiffuseA1 = diffuseAa + subPixelY * DiffuseAInterpolate1;

					SpecularR1 = specularRa + subPixelY * SpecularRInterpolate1;
					SpecularG1 = specularGa + subPixelY * SpecularGInterpolate1;
					SpecularB1 = specularBa + subPixelY * SpecularBInterpolate1;
					SpecularA1 = specularAa + subPixelY * SpecularAInterpolate1;


					Z1 = oneOverZA + subPixelY * ZInterpolateY1;
					U1 = u1OneOverZA + subPixelY * U1InterpolateY1;
					V1 = v1OneOverZA + subPixelY * V1InterpolateY1;

					U2 = u2OneOverZA + subPixelY * U2InterpolateY1;
					V2 = v2OneOverZA + subPixelY * V2InterpolateY1;

					Y1Int = yaInt;
					Y2Int = ybInt;

					Y1 = ya;
					Y2 = yb;

					DrawSubTriangleSegment();
				}
				if (ybInt < ycInt)
				{
					XInterpolate1 = xInterpolateC;
					RInterpolate1 = rInterpolateC;
					GInterpolate1 = gInterpolateC;
					BInterpolate1 = bInterpolateC;
					AInterpolate1 = aInterpolateC;

					DiffuseRInterpolate1 = diffuseRInterpolateC;
					DiffuseGInterpolate1 = diffuseGInterpolateC;
					DiffuseBInterpolate1 = diffuseBInterpolateC;
					DiffuseAInterpolate1 = diffuseAInterpolateC;

					SpecularRInterpolate1 = specularRInterpolateC;
					SpecularGInterpolate1 = specularGInterpolateC;
					SpecularBInterpolate1 = specularBInterpolateC;
					SpecularAInterpolate1 = specularAInterpolateC;

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

					DiffuseR1 = diffuseRb + subPixelY * DiffuseRInterpolate1;
					DiffuseG1 = diffuseGb + subPixelY * DiffuseGInterpolate1;
					DiffuseB1 = diffuseBb + subPixelY * DiffuseBInterpolate1;
					DiffuseA1 = diffuseAb + subPixelY * DiffuseAInterpolate1;

					SpecularR1 = specularRb + subPixelY * SpecularRInterpolate1;
					SpecularG1 = specularGb + subPixelY * SpecularGInterpolate1;
					SpecularB1 = specularBb + subPixelY * SpecularBInterpolate1;
					SpecularA1 = specularAb + subPixelY * SpecularAInterpolate1;

					Z1 = oneOverZB + subPixelY * ZInterpolateY1;
					U1 = u1OneOverZB + subPixelY * U1InterpolateY1;
					V1 = v1OneOverZB + subPixelY * V1InterpolateY1;

					U2 = u2OneOverZB + subPixelY * U2InterpolateY1;
					V2 = v2OneOverZB + subPixelY * V2InterpolateY1;

					Y1Int = ybInt;
					Y2Int = ycInt;

					Y1 = yb;
					Y2 = yc;

					DrawSubTriangleSegment();
				}
			}
		}



		private void DrawSubTriangleSegment()
		{
			var originalHeight = Y2Int - Y1Int;

			if (
				(Y1Int > BufferContainer.Height || Y2Int < 0) ||
				(Y1Int > BufferContainer.Height && Y2Int > BufferContainer.Height) ||
				(Y1Int < 0 && Y2Int < 0)
				)
			{
				X1 += (XInterpolate1 * originalHeight);
				X2 += (XInterpolate2 * originalHeight);

				R1 += (RInterpolate1 * originalHeight);
				G1 += (GInterpolate1 * originalHeight);
				B1 += (BInterpolate1 * originalHeight);
				A1 += (AInterpolate1 * originalHeight);

				R2 += (RInterpolate2 * originalHeight);
				G2 += (GInterpolate2 * originalHeight);
				B2 += (BInterpolate2 * originalHeight);
				A2 += (AInterpolate2 * originalHeight);

				DiffuseR1 += (DiffuseRInterpolate1 * originalHeight);
				DiffuseG1 += (DiffuseGInterpolate1 * originalHeight);
				DiffuseB1 += (DiffuseBInterpolate1 * originalHeight);
				DiffuseA1 += (DiffuseAInterpolate1 * originalHeight);

				DiffuseR2 += (DiffuseRInterpolate2 * originalHeight);
				DiffuseG2 += (DiffuseGInterpolate2 * originalHeight);
				DiffuseB2 += (DiffuseBInterpolate2 * originalHeight);
				DiffuseA2 += (DiffuseAInterpolate2 * originalHeight);

				SpecularR1 += (SpecularRInterpolate1 * originalHeight);
				SpecularG1 += (SpecularGInterpolate1 * originalHeight);
				SpecularB1 += (SpecularBInterpolate1 * originalHeight);
				SpecularA1 += (SpecularAInterpolate1 * originalHeight);

				SpecularR2 += (SpecularRInterpolate2 * originalHeight);
				SpecularG2 += (SpecularGInterpolate2 * originalHeight);
				SpecularB2 += (SpecularBInterpolate2 * originalHeight);
				SpecularA2 += (SpecularAInterpolate2 * originalHeight);

				Z1 += (ZInterpolateY1 * originalHeight);

				U1 += (U1InterpolateY1 * originalHeight);
				V1 += (V1InterpolateY1 * originalHeight);

				U2 += (U2InterpolateY1 * originalHeight);
				V2 += (V2InterpolateY1 * originalHeight);

				return;
			}

			var amountToClip = 0f;
			var originalY1Int = Y1Int;
			var originalY2Int = Y2Int;


			var exitX1 = X1 + (XInterpolate1 * originalHeight);
			var exitX2 = X2 + (XInterpolate2 * originalHeight);

			if (Y1Int < 0)
			{
				amountToClip = (float)-Y1Int;

				X1 += (XInterpolate1 * amountToClip);
				X2 += (XInterpolate2 * amountToClip);

				R1 += (RInterpolate1 * amountToClip);
				G1 += (GInterpolate1 * amountToClip);
				B1 += (BInterpolate1 * amountToClip);
				A1 += (AInterpolate1 * amountToClip);

				R2 += (RInterpolate2 * amountToClip);
				G2 += (GInterpolate2 * amountToClip);
				B2 += (BInterpolate2 * amountToClip);
				A2 += (AInterpolate2 * amountToClip);

				DiffuseR1 += (DiffuseRInterpolate1 * amountToClip);
				DiffuseG1 += (DiffuseGInterpolate1 * amountToClip);
				DiffuseB1 += (DiffuseBInterpolate1 * amountToClip);
				DiffuseA1 += (DiffuseAInterpolate1 * amountToClip);

				DiffuseR2 += (DiffuseRInterpolate2 * amountToClip);
				DiffuseG2 += (DiffuseGInterpolate2 * amountToClip);
				DiffuseB2 += (DiffuseBInterpolate2 * amountToClip);
				DiffuseA2 += (DiffuseAInterpolate2 * amountToClip);

				SpecularR1 += (SpecularRInterpolate1 * amountToClip);
				SpecularG1 += (SpecularGInterpolate1 * amountToClip);
				SpecularB1 += (SpecularBInterpolate1 * amountToClip);
				SpecularA1 += (SpecularAInterpolate1 * amountToClip);

				SpecularR2 += (SpecularRInterpolate2 * amountToClip);
				SpecularG2 += (SpecularGInterpolate2 * amountToClip);
				SpecularB2 += (SpecularBInterpolate2 * amountToClip);
				SpecularA2 += (SpecularAInterpolate2 * amountToClip);

				Z1 += (ZInterpolateY1 * amountToClip);

				U1 += (U1InterpolateY1 * amountToClip);
				V1 += (V1InterpolateY1 * amountToClip);

				U2 += (U2InterpolateY1 * amountToClip);
				V2 += (V2InterpolateY1 * amountToClip);

				Y1Int = 0;
			}


			if (Y2Int > BufferContainer.Height)
				Y2Int = BufferContainer.Height;

			var yoffset = BufferContainer.Width * Y1Int;

			var leftClip = false;

			var originalX1 = 0f;
			var originalX2 = 0f;
			var originalZ1 = 0f;
			var originalU1 = 0f;
			var originalV1 = 0f;
			var originalU2 = 0f;
			var originalV2 = 0f;
			var clipLength = 0f;

			var scanlineClip = false;

			for (var y = Y1Int; y < Y2Int; y++)
			{
				leftClip = false;
				X1Int = (int)X1;
				X2Int = (int)X2;
				scanlineClip = false;

				
				if (X1Int > BufferContainer.Width && X2Int > BufferContainer.Width)
					scanlineClip = true;

				if (X1Int < 0 && X2Int < 0)
					scanlineClip = true;

				if (((int)X1 < (int)X2) && !scanlineClip)
				{
					originalX1 = X1;
					originalX2 = X2;

					originalZ1 = Z1;

					originalU1 = U1;
					originalV1 = V1;

					originalU2 = U2;
					originalV2 = V2;

					var oneOverLength = 1f / (X2-X1);

					if (X1 < 0)
					{
						clipLength = -X1;
						Z1 += ZInterpolateX * clipLength;
						U1 += U1zInterpolateX * clipLength;
						V1 += V1zInterpolateX * clipLength;

						U2 += U2zInterpolateX * clipLength;
						V2 += V2zInterpolateX * clipLength;

						X1 = 0;
						X1Int = 0;
						leftClip = true;
					}
					
					if (X2 > BufferContainer.Width)
					{
						X2 = BufferContainer.Width;
						X2Int = BufferContainer.Width;
					}

					PixelOffset = yoffset + X1Int;

					RScanlineInterpolate = (R2 - R1) * oneOverLength;
					GScanlineInterpolate = (G2 - G1) * oneOverLength;
					BScanlineInterpolate = (B2 - B1) * oneOverLength;
					AScanlineInterpolate = (A2 - A1) * oneOverLength;

					DiffuseRScanlineInterpolate = (DiffuseR2 - DiffuseR1) * oneOverLength;
					DiffuseGScanlineInterpolate = (DiffuseG2 - DiffuseG1) * oneOverLength;
					DiffuseBScanlineInterpolate = (DiffuseB2 - DiffuseB1) * oneOverLength;
					DiffuseAScanlineInterpolate = (DiffuseA2 - DiffuseA1) * oneOverLength;

					SpecularRScanlineInterpolate = (SpecularR2 - SpecularR1) * oneOverLength;
					SpecularGScanlineInterpolate = (SpecularG2 - SpecularG1) * oneOverLength;
					SpecularBScanlineInterpolate = (SpecularB2 - SpecularB1) * oneOverLength;
					SpecularAScanlineInterpolate = (SpecularA2 - SpecularA1) * oneOverLength;

					RScanline = R1;
					GScanline = G1;
					BScanline = B1;
					AScanline = A1;

					DiffuseRScanline = DiffuseR1;
					DiffuseGScanline = DiffuseG1;
					DiffuseBScanline = DiffuseB1;
					DiffuseAScanline = DiffuseA1;

					SpecularRScanline = SpecularR1;
					SpecularGScanline = SpecularG1;
					SpecularBScanline = SpecularB1;
					SpecularAScanline = SpecularA1;

					if( leftClip )
					{
						RScanline += RScanlineInterpolate * clipLength;
						GScanline += GScanlineInterpolate * clipLength;
						BScanline += BScanlineInterpolate * clipLength;
						AScanline += AScanlineInterpolate * clipLength;

						DiffuseRScanline += DiffuseRScanlineInterpolate * clipLength;
						DiffuseGScanline += DiffuseGScanlineInterpolate * clipLength;
						DiffuseBScanline += DiffuseBScanlineInterpolate * clipLength;
						DiffuseAScanline += DiffuseAScanlineInterpolate * clipLength;

						SpecularRScanline += SpecularRScanlineInterpolate * clipLength;
						SpecularGScanline += SpecularGScanlineInterpolate * clipLength;
						SpecularBScanline += SpecularBScanlineInterpolate * clipLength;
						SpecularAScanline += SpecularAScanlineInterpolate * clipLength;
					}


					DrawSpan(PixelOffset);

					X1 = originalX1;
					X2 = originalX2;

					Z1 = originalZ1;

					U1 = originalU1;
					V1 = originalV1;

					U2 = originalU2;
					V2 = originalV2;
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

				DiffuseR1 += DiffuseRInterpolate1;
				DiffuseG1 += DiffuseGInterpolate1;
				DiffuseB1 += DiffuseBInterpolate1;
				DiffuseA1 += DiffuseAInterpolate1;

				DiffuseR2 += DiffuseRInterpolate2;
				DiffuseG2 += DiffuseGInterpolate2;
				DiffuseB2 += DiffuseBInterpolate2;
				DiffuseA2 += DiffuseAInterpolate2;

				SpecularR1 += SpecularRInterpolate1;
				SpecularG1 += SpecularGInterpolate1;
				SpecularB1 += SpecularBInterpolate1;
				SpecularA1 += SpecularAInterpolate1;

				SpecularR2 += SpecularRInterpolate2;
				SpecularG2 += SpecularGInterpolate2;
				SpecularB2 += SpecularBInterpolate2;
				SpecularA2 += SpecularAInterpolate2;

				Z1 += ZInterpolateY1;
				U1 += U1InterpolateY1;
				V1 += V1InterpolateY1;

				U2 += U2InterpolateY1;
				V2 += V2InterpolateY1;

				yoffset += BufferContainer.Width;
			}

			Y1Int = originalY1Int;
			Y2Int = originalY2Int;

			X1 = exitX1;
			X2 = exitX2;
		}

		protected virtual void DrawSpan(int offset)
		{
		}

		protected void SetPixel(int offset, int pixel)
		{
			if (Opacity < 256)
			{
				Framebuffer[offset] = Color.Blend(pixel, Framebuffer[offset], Opacity);
			}
			else
			{
				Framebuffer[offset] = pixel;
			}

		}
	}
}
#endif