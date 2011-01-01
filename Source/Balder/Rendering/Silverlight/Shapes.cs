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
using Balder;
using Balder.Display;

namespace Balder.Rendering.Silverlight
{
	public static class Shapes
	{
		public static void DrawLine(Viewport viewport, float xstart, float ystart, float xend, float yend, int color)
		{
			var stride = BufferContainer.Width;
			var framebuffer = BufferContainer.Framebuffer;
			if( null == framebuffer )
			{
				return;
			}

			var deltaX = xend - xstart;
			var deltaY = yend - ystart;

			var lengthX = deltaX >= 0 ? deltaX : -deltaX;
			var lengthY = deltaY >= 0 ? deltaY : -deltaY;

			var actualLength = lengthX > lengthY ? lengthX : lengthY;

			if( actualLength != 0 )
			{
				var slopeX =  deltaX/ actualLength;
				var slopeY =  deltaY/ actualLength;

				var currentX = xstart;
				var currentY = ystart;

				for( var pixel=0; pixel<actualLength; pixel++)
				{
					if (currentX > 0 && currentY > 0 && currentX < viewport.Width && currentY < viewport.Height)
					{
						var bufferOffset = (stride*(int) currentY) + (int) currentX;
						framebuffer[bufferOffset] = color;
					}

					currentX += slopeX;
					currentY += slopeY;
				}
			}
		}


		public static void DrawLine(Viewport viewport, 
			float xstart, 
			float ystart,
			float zstart,
			float xend, 
			float yend, 
			float zend,
			int color)
		{
			var stride = BufferContainer.Width;
			var framebuffer = BufferContainer.Framebuffer;
			var depthbuffer = BufferContainer.DepthBuffer;
			if (null == framebuffer)
			{
				return;
			}

			var deltaX = xend - xstart;
			var deltaY = yend - ystart;
			var deltaZ = zend - zstart;

			var lengthX = deltaX >= 0 ? deltaX : -deltaX;
			var lengthY = deltaY >= 0 ? deltaY : -deltaY;

			var actualLength = lengthX > lengthY ? lengthX : lengthY;

			if (actualLength != 0)
			{
				var slopeX = deltaX / actualLength;
				var slopeY = deltaY / actualLength;
				var slopeZ = deltaZ / actualLength;

				var currentX = xstart;
				var currentY = ystart;
				var currentZ = zstart;

				for (var pixel = 0; pixel < actualLength; pixel++)
				{
					if (currentX > 0 && currentY > 0 && currentX < viewport.Width && currentY < viewport.Height)
					{
						
						var bufferOffset = (stride * (int)currentY) + (int)currentX;
						var bufferZ = (UInt32)((1.0f - (currentZ*viewport.View.DepthMultiplier)) * (float)UInt32.MaxValue);
						if (bufferZ > depthbuffer[bufferOffset] &&
							currentZ >= viewport.View.Near &&
							currentZ < viewport.View.Far
							)
						{
							depthbuffer[bufferOffset] = bufferZ;
							framebuffer[bufferOffset] = color;
						}
					}

					currentX += slopeX;
					currentY += slopeY;
					currentZ += slopeZ;
				}
			}
		}

	}
}
#endif