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
	public abstract class TriangleWithPerspectiveCorrection : Triangle
	{
        protected int _bufferWidth;
        protected int _bufferHeight;

        
		protected float U1zInterpolateX;
		protected float V1zInterpolateX;
		protected float U1zInterpolateY;

		protected float U2zInterpolateX;
		protected float V2zInterpolateX;
		protected float U2zInterpolateY;

        static float UInt32MaxValueAsFloat = (float)UInt32.MaxValue;
        

		public virtual void Draw(Viewport viewport, RenderFace face, RenderVertex vertexA, RenderVertex vertexB, RenderVertex vertexC)
		{
            _bufferWidth = BufferContainer.Width;
            _bufferHeight = BufferContainer.Height;

			Opacity = face.Opacity;

			Near = viewport.View.Near;
			Far = viewport.View.Far;
			DepthMultiplier = viewport.View.DepthMultiplier;
			ZMultiplier = DepthMultiplier * UInt32MaxValueAsFloat;

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
			var texture1Width = 0f;
			var texture1Height = 0f;
			var texture2Width = 0f;
			var texture2Height = 0f;

			if (null != Texture1)
			{
				texture1Width = Texture1.WidthAsFloat;
				texture1Height = Texture1.HeightAsFloat;
			}

			if (null != Texture2)
			{
				texture2Width = Texture2.WidthAsFloat;
				texture2Height = Texture2.HeightAsFloat;
			}

			var xa = vertexA.ProjectedVector.X + 0.5f;
			var ya = vertexA.ProjectedVector.Y + 0.5f;
			var za = vertexA.ProjectedVector.Z;
			var u1a = vertexA.U1 * texture1Width;
			var v1a = vertexA.V1 * texture1Height;
			var u2a = vertexA.U2 * texture2Width;
			var v2a = vertexA.V2 * texture2Height;
			var ra = vertexA.CalculatedColor.RedAsFloat;
			var ga = vertexA.CalculatedColor.GreenAsFloat;
			var ba = vertexA.CalculatedColor.BlueAsFloat;
			var aa = vertexA.CalculatedColor.AlphaAsFloat;
			var diffuseRa = vertexA.DiffuseColor.RedAsFloat;
			var diffuseGa = vertexA.DiffuseColor.GreenAsFloat;
			var diffuseBa = vertexA.DiffuseColor.BlueAsFloat;
			var diffuseAa = vertexA.DiffuseColor.AlphaAsFloat;
			var specularRa = vertexA.SpecularColor.RedAsFloat;
			var specularGa = vertexA.SpecularColor.GreenAsFloat;
			var specularBa = vertexA.SpecularColor.BlueAsFloat;
            var specularAa = vertexA.SpecularColor.AlphaAsFloat;
            
			var xb = vertexB.ProjectedVector.X + 0.5f;
			var yb = vertexB.ProjectedVector.Y + 0.5f;
			var zb = vertexB.ProjectedVector.Z;
			var u1b = vertexB.U1 * texture1Width;
			var v1b = vertexB.V1 * texture1Height;
			var u2b = vertexB.U2 * texture2Width;
			var v2b = vertexB.V2 * texture2Height;
			var rb = vertexB.CalculatedColor.RedAsFloat;
			var gb = vertexB.CalculatedColor.GreenAsFloat;
			var bb = vertexB.CalculatedColor.BlueAsFloat;
			var ab = vertexB.CalculatedColor.AlphaAsFloat;
			var diffuseRb = vertexB.DiffuseColor.RedAsFloat;
			var diffuseGb = vertexB.DiffuseColor.GreenAsFloat;
			var diffuseBb = vertexB.DiffuseColor.BlueAsFloat;
			var diffuseAb = vertexB.DiffuseColor.AlphaAsFloat;
			var specularRb = vertexB.SpecularColor.RedAsFloat;
			var specularGb = vertexB.SpecularColor.GreenAsFloat;
			var specularBb = vertexB.SpecularColor.BlueAsFloat;
            var specularAb = vertexB.SpecularColor.AlphaAsFloat;

            var xc = vertexC.ProjectedVector.X + 0.5f;
			var yc = vertexC.ProjectedVector.Y + 0.5f;
			var zc = vertexC.ProjectedVector.Z;
			var u1c = vertexC.U1 * texture1Width;
			var v1c = vertexC.V1 * texture1Height;
			var u2c = vertexC.U2 * texture2Width;
			var v2c = vertexC.V2 * texture2Height;
			var rc = vertexC.CalculatedColor.RedAsFloat;
			var gc = vertexC.CalculatedColor.GreenAsFloat;
			var bc = vertexC.CalculatedColor.BlueAsFloat;
            var ac = vertexC.CalculatedColor.AlphaAsFloat;
			var diffuseRc = vertexC.DiffuseColor.RedAsFloat;
			var diffuseGc = vertexC.DiffuseColor.GreenAsFloat;
			var diffuseBc = vertexC.DiffuseColor.BlueAsFloat;
            var diffuseAc = vertexC.DiffuseColor.AlphaAsFloat;
			var specularRc = vertexC.SpecularColor.RedAsFloat;
			var specularGc = vertexC.SpecularColor.GreenAsFloat;
			var specularBc = vertexC.SpecularColor.BlueAsFloat;
            var specularAc = vertexC.SpecularColor.AlphaAsFloat;


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

            var oneOverDeltaYA = 1f / deltaYA;
            var oneOverDeltaYB = 1f / deltaYB;
            var oneOverDeltaYC = 1f / deltaYC;


			var xInterpolateA = deltaXA * oneOverDeltaYA;
			var xInterpolateB = deltaXB * oneOverDeltaYB;
			var xInterpolateC = deltaXC * oneOverDeltaYC;

			var rInterpolateA = deltaRA * oneOverDeltaYA;
			var rInterpolateB = deltaRB * oneOverDeltaYB;
			var rInterpolateC = deltaRC * oneOverDeltaYC;

			var gInterpolateA = deltaGA * oneOverDeltaYA;
			var gInterpolateB = deltaGB * oneOverDeltaYB;
			var gInterpolateC = deltaGC * oneOverDeltaYC;

			var bInterpolateA = deltaBA * oneOverDeltaYA;
			var bInterpolateB = deltaBB * oneOverDeltaYB;
			var bInterpolateC = deltaBC * oneOverDeltaYC;

			var aInterpolateA = deltaAA * oneOverDeltaYA;
			var aInterpolateB = deltaAB * oneOverDeltaYB;
			var aInterpolateC = deltaAC * oneOverDeltaYC;

			var diffuseRInterpolateA = deltaDiffuseRA * oneOverDeltaYA;
			var diffuseRInterpolateB = deltaDiffuseRB * oneOverDeltaYB;
			var diffuseRInterpolateC = deltaDiffuseRC * oneOverDeltaYC;

			var diffuseGInterpolateA = deltaDiffuseGA * oneOverDeltaYA;
			var diffuseGInterpolateB = deltaDiffuseGB * oneOverDeltaYB;
			var diffuseGInterpolateC = deltaDiffuseGC * oneOverDeltaYC;

			var diffuseBInterpolateA = deltaDiffuseBA * oneOverDeltaYA;
			var diffuseBInterpolateB = deltaDiffuseBB * oneOverDeltaYB;
			var diffuseBInterpolateC = deltaDiffuseBC * oneOverDeltaYC;

			var diffuseAInterpolateA = deltaDiffuseAA * oneOverDeltaYA;
			var diffuseAInterpolateB = deltaDiffuseAB * oneOverDeltaYB;
			var diffuseAInterpolateC = deltaDiffuseAC * oneOverDeltaYC;


			var specularRInterpolateA = deltaSpecularRA * oneOverDeltaYA;
			var specularRInterpolateB = deltaSpecularRB * oneOverDeltaYB;
			var specularRInterpolateC = deltaSpecularRC * oneOverDeltaYC;

			var specularGInterpolateA = deltaSpecularGA * oneOverDeltaYA;
			var specularGInterpolateB = deltaSpecularGB * oneOverDeltaYB;
			var specularGInterpolateC = deltaSpecularGC * oneOverDeltaYC;

			var specularBInterpolateA = deltaSpecularBA * oneOverDeltaYA;
			var specularBInterpolateB = deltaSpecularBB * oneOverDeltaYB;
			var specularBInterpolateC = deltaSpecularBC * oneOverDeltaYC;

			var specularAInterpolateA = deltaSpecularAA * oneOverDeltaYA;
			var specularAInterpolateB = deltaSpecularAB * oneOverDeltaYB;
			var specularAInterpolateC = deltaSpecularAC * oneOverDeltaYC;


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
				(Y1Int > _bufferHeight || Y2Int < 0) ||
				(Y1Int > _bufferHeight && Y2Int > _bufferHeight) ||
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


			if (Y2Int > _bufferHeight)
				Y2Int = _bufferHeight;

			var yoffset = _bufferWidth * Y1Int;

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

				
				if (X1Int > _bufferWidth && X2Int > _bufferWidth)
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
					
					if (X2 > _bufferWidth)
					{
						X2 = _bufferWidth;
						X2Int = _bufferWidth;
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

				yoffset += _bufferWidth;
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