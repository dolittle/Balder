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
using Balder.Display;
using Balder.Imaging;
using Balder.Math;
using Balder.Objects.Flat;

namespace Balder.Rendering.Silverlight
{
	public class SpriteContext : ISpriteContext
	{
		private static readonly Interpolator XScalingInterpolator;
		private static readonly Interpolator YScalingInterpolator;
		private RenderVertex _vertex;

		static SpriteContext()
		{
			XScalingInterpolator = new Interpolator();
			XScalingInterpolator.SetNumberOfInterpolationPoints(1);
			YScalingInterpolator = new Interpolator();
			YScalingInterpolator.SetNumberOfInterpolationPoints(1);
		}

		public SpriteContext()
		{
			_vertex = new RenderVertex(0,0,0);
		}

		public void Render(Viewport viewport, Sprite sprite, Matrix view, Matrix projection, Matrix world, float xScale, float yScale, float rotation)
		{
			var image = sprite.CurrentFrame;
			if( null == image )
			{
				return;
			}

			var worldView = world*view;
			var worldViewProjection = worldView*projection;

			_vertex.TransformAndProject(viewport, worldView, worldViewProjection);

			if (_vertex.ProjectedVector.Z < viewport.View.Near || _vertex.ProjectedVector.Z >= viewport.View.Far)
			{
				return;
			}
			var bufferSize = BufferContainer.Width * BufferContainer.Height;
			var bufferZ = (UInt32)((1.0f - (_vertex.ProjectedVector.Z*viewport.View.DepthMultiplier)) * (float)UInt32.MaxValue);

			var xOriginOffset = (int)-((sprite.CurrentFrame.Width / 2f) * xScale);
			var yOriginOffset = (int)-((sprite.CurrentFrame.Height / 2f) * yScale);

			var actualX = ((int)_vertex.ProjectedVector.X) + xOriginOffset;
			var actualY = ((int)_vertex.ProjectedVector.Y) + yOriginOffset;

			var positionOffset = actualX + (actualY * BufferContainer.Width);

			if (xScale != 1f || yScale != 1f)
			{
				RenderScaled(viewport, positionOffset, actualX, actualY, sprite.CurrentFrame, _vertex.ProjectedVector, bufferSize, bufferZ, xScale, yScale);
			}
			else
			{
				RenderUnscaled(viewport, positionOffset, actualX, actualY, sprite.CurrentFrame, _vertex.ProjectedVector, bufferSize, bufferZ);
			}
		}

		private static void RenderUnscaled(
			Viewport viewport, 
			int positionOffset, 
			int actualX, 
			int actualY, 
			Image image, 
			Vector translatedPosition, 
			int bufferSize, 
			UInt32 bufferZ)
		{
			var imageContext = image.ImageContext as ImageContext;
			var spriteOffset = 0;

			var currentY = actualY;

			

			for (var y = 0; y < image.Height; y++)
			{
				if (currentY >= viewport.Height)
				{
					break;
				}

				if (currentY >= 0)
				{
					var offset = y*BufferContainer.Width;
					var currentX = actualX;

					for (var x = 0; x < image.Width; x++)
					{
						if (currentX >= viewport.Width)
						{
							break;
						}
						if (currentX >= 0)
						{
							var actualOffset = offset + positionOffset;

							if (actualOffset >= 0 && actualOffset < bufferSize &&
							    bufferZ < BufferContainer.DepthBuffer[actualOffset])
							{
								var pixel = BlendPixel(imageContext.Pixels[spriteOffset], BufferContainer.Framebuffer[actualOffset]);
								if ((pixel & 0xff000000) != 0)
								{
									BufferContainer.Framebuffer[actualOffset] = pixel;
									BufferContainer.DepthBuffer[actualOffset] = bufferZ;
								}
							}
						}
						offset++;
						spriteOffset++;
						currentX++;
					}
				}

				currentY++;
			}
		}

		private static void RenderScaled(
			Viewport viewport, 
			int positionOffset, 
			int actualX, 
			int actualY, 
			Image image, 
			Vector translatedPosition, 
			int bufferSize, 
			UInt32 bufferZ, 
			float xScale, 
			float yScale)
		{
			var imageContext = image.ImageContext as ImageContext;

			var actualWidth = (int)(((float)image.Width) * xScale);
			var actualHeight = (int)(((float)image.Height) * yScale);

			if (actualWidth <= 0 || actualHeight <= 0)
			{
				return;
			}

			var spriteOffset = 0;

			XScalingInterpolator.SetPoint(0, 0f, image.Width);
			XScalingInterpolator.Interpolate(actualWidth);

			YScalingInterpolator.SetPoint(0, 0f, image.Height);
			YScalingInterpolator.Interpolate(actualHeight);

			var currentY = actualY;

			for (var y = 0; y < actualHeight; y++)
			{
				if (currentY >= viewport.Height)
				{
					break;
				}
				if (currentY >= 0)
				{
					var currentX = actualX;
					var offset = y*BufferContainer.Width;

					var spriteY = (int) YScalingInterpolator.Points[0].InterpolatedValues[y];

					for (var x = 0; x < actualWidth; x++)
					{
						if (currentX >= viewport.Width)
						{
							break;
						}
						if (currentX >= 0)
						{
							var actualOffset = offset + positionOffset;


							var spriteX = (int) XScalingInterpolator.Points[0].InterpolatedValues[x];
							spriteOffset = (int) ((spriteY*image.Width) + spriteX);

							if (actualOffset >= 0 && actualOffset < bufferSize &&
							    bufferZ > BufferContainer.DepthBuffer[actualOffset])
							{

								var pixel = BlendPixel(imageContext.Pixels[spriteOffset], BufferContainer.Framebuffer[actualOffset]);
								if ((pixel & 0xff000000) != 0)
								{
									BufferContainer.Framebuffer[actualOffset] = pixel;
									BufferContainer.DepthBuffer[actualOffset] = bufferZ;
								}
							}
						}
						offset++;

						currentX++;
					}
				}
				currentY++;
			}
		}

		private static int BlendPixel(int sourcePixel, int destinationPixel)
		{
			var sa = ((sourcePixel >> 24) & 0xff);
			var sr = ((sourcePixel >> 16) & 0xff);
			var sg = ((sourcePixel >> 8) & 0xff);
			var sb = ((sourcePixel) & 0xff);

			var dr = ((destinationPixel >> 16) & 0xff);
			var dg = ((destinationPixel >> 8) & 0xff);
			var db = ((destinationPixel) & 0xff);
			var da = ((destinationPixel >> 24) & 0xff);

			destinationPixel = ((sa + (((da * (255 - sa)) * 0x8081) >> 23)) << 24) |
			                   ((sr + (((dr * (255 - sa)) * 0x8081) >> 23)) << 16) |
			                   ((sg + (((dg * (255 - sa)) * 0x8081) >> 23)) << 8) |
			                   ((sb + (((db * (255 - sa)) * 0x8081) >> 23)));
			return destinationPixel;
		}
	}
}
#endif