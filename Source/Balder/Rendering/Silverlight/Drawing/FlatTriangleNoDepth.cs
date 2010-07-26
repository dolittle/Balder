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

namespace Balder.Rendering.Silverlight.Drawing
{
	public class FlatTriangleNoDepth : Triangle
	{
		public override void Draw(RenderFace face, RenderVertex[] vertices, UInt32 nodeIdentifier)
		{
			var vertexA = vertices[face.A];
			var vertexB = vertices[face.B];
			var vertexC = vertices[face.C];

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;

			var deltaX1 = xb - xa;
			var deltaX2 = xc - xb;
			var deltaX3 = xc - xa;

			var deltaY1 = yb - ya;
			var deltaY2 = yc - yb;
			var deltaY3 = yc - ya;

			var x1 = xa;
			var x2 = xa;

			var xInterpolate1 = deltaX3 / deltaY3;
			var xInterpolate2 = deltaX1 / deltaY1;
			var xInterpolate3 = deltaX2 / deltaY2;

			var framebuffer = BufferContainer.Framebuffer;
			var depthBuffer = BufferContainer.DepthBuffer;
			var nodeBuffer = BufferContainer.NodeBuffer;
			var frameBufferWidth = BufferContainer.Width;
			var frameBufferHeight = BufferContainer.Height;

			var yStart = (int)ya;
			var yEnd = (int) yc;
			var yClipTop = 0;
			
			if( yStart < 0 )
			{
				yClipTop = -yStart;
				yStart = 0;
			}

			if( yEnd >= frameBufferHeight )
			{
				yEnd = frameBufferHeight-1;
			}

			var height = yEnd - yStart;
			if (height == 0)
			{
				return;
			}

			if( yClipTop > 0 )
			{
				x1 = xa+xInterpolate1*(float) yClipTop;

				if( yb < 0 )
				{
					var ySecondClipTop = -yb;

					x2 = xb + (xInterpolate3*ySecondClipTop);
					xInterpolate2 = xInterpolate3;

				} else
				{
					x2 = xa + xInterpolate2*(float) yClipTop;
				}
			}

			var yoffset = BufferContainer.Width * yStart;

			var colorAsInt = (int) face.Color.ToUInt32();

			var offset = 0;
			var length = 0;
			var originalLength = 0;

			var xStart = 0;
			var xEnd = 0;

			var xClipStart = 0;

			for (var y = yStart; y <= yEnd; y++)
			{
				if (x2 < x1)
				{
					xStart = (int)x2;
					xEnd = (int)x1;
				}
				else
				{
					xStart = (int)x1;
					xEnd = (int)x2;

					offset = yoffset + (int)x1;
				}
				originalLength = xEnd - xStart;

				if( xStart < 0 )
				{
					xClipStart = -xStart;
					xStart = 0;
				}
				if( xEnd >= frameBufferWidth )
				{
					xEnd = frameBufferWidth - 1;
				}


				length = xEnd - xStart;

				if (length != 0)
				{
					offset = yoffset + xStart;

					DrawSpan(length, offset, framebuffer, colorAsInt, nodeBuffer, nodeIdentifier);
				}

				if (y == (int)yb)
				{
					x2 = xb;
					xInterpolate2 = xInterpolate3;
				}


				x1 += xInterpolate1;
				x2 += xInterpolate2;

				yoffset += BufferContainer.Width;
			}
		}

		protected virtual void DrawSpan(
			int length, 
			int offset, 
			int[] framebuffer, 
			int colorAsInt, 
			UInt32[] nodeBuffer, 
			UInt32 nodeIdentifier)
		{
			for (var x = 0; x <= length; x++)
			{
				framebuffer[offset] = colorAsInt;
				nodeBuffer[offset] = nodeIdentifier;
						
				offset++;
			}
		}
	}
}
#endif