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
using Balder.Math;

namespace Balder.Rendering.Silverlight.Drawing
{
	// Based upon : http://www.lysator.liu.se/~mikaelk/doc/perspectivetexture/
	public class TextureTrianglePerspectiveCorrected : Triangle
	{
		private int[] _framebuffer;
		private uint[] _depthBuffer;

		private int _pixelOffset;

		private int _y1Int;
		private int _y2Int;

		private float _xInterpolate1;
		private float _xInterpolate2;

		private TextureMipMapLevel _texture;
		private int[] _texels;

		private float _x1;
		private float _x2;
		private float _z1;
		private float _u1;
		private float _v1;

		private float _zInterpolationX;
		private float _uzInerpolationX;
		private float _vzInterpolationX;
		private float _zInterpolationY;
		private float _uzInterpolationY;
		private float _vInterpolationY;
		private float _zInterpolateY1;
		private float _uInterpolateY1;
		private float _vInterpolateY1;

	
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


			_texture = null;
			if (null != face.DiffuseTexture)
			{
				_texture = face.DiffuseTexture.FullDetailLevel;
			}
			else if (null != face.ReflectionTexture)
			{
				_texture = face.ReflectionTexture.FullDetailLevel;
				SetSphericalEnvironmentMapTextureCoordinate(vertexA);
				SetSphericalEnvironmentMapTextureCoordinate(vertexB);
				SetSphericalEnvironmentMapTextureCoordinate(vertexC);
			}

			_texels = _texture.OriginalPixels;

			GetSortedPoints(ref vertexA, ref vertexB, ref vertexC);

			var xa = vertexA.TranslatedScreenCoordinates.X;
			var ya = vertexA.TranslatedScreenCoordinates.Y;
			var za = vertexA.DepthBufferAdjustedZ;
			var ua = vertexA.U * _texture.Width;
			var va = vertexA.V * _texture.Height;

			var xb = vertexB.TranslatedScreenCoordinates.X;
			var yb = vertexB.TranslatedScreenCoordinates.Y;
			var zb = vertexB.DepthBufferAdjustedZ;
			var ub = vertexB.U * _texture.Width;
			var vb = vertexB.V * _texture.Height;

			var xc = vertexC.TranslatedScreenCoordinates.X;
			var yc = vertexC.TranslatedScreenCoordinates.Y;
			var zc = vertexC.DepthBufferAdjustedZ;
			var uc = vertexC.U * _texture.Width;
			var vc = vertexC.V * _texture.Height;

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
			_zInterpolationX = ((oneOverZC - oneOverZA) * deltaYA - (oneOverZB - oneOverZA) * deltaYB) * denominator;
			_uzInerpolationX = ((uOneOverZC - uOneOverZA) * deltaYA - (uOneOverZB - uOneOverZA) * deltaYB) * denominator;
			_vzInterpolationX = ((vOneOverZC - vOneOverZA) * deltaYA - (vOneOverZB - vOneOverZA) * deltaYB) * denominator;
			_zInterpolationY = ((oneOverZB - oneOverZA) * deltaXB - (oneOverZC - oneOverZA) * deltaXA) * denominator;
			_uzInterpolationY = ((uOneOverZB - uOneOverZA) * deltaXB - (uOneOverZC - uOneOverZA) * deltaXA) * denominator;
			_vInterpolationY = ((vOneOverZB - vOneOverZA) * deltaXB - (vOneOverZC - vOneOverZA) * deltaXA) * denominator;

			_framebuffer = BufferContainer.Framebuffer;
			_depthBuffer = BufferContainer.DepthBuffer;

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
				_xInterpolate1 = xInterpolateB;

				_zInterpolateY1 = xInterpolateB * _zInterpolationX + _zInterpolationY;
				_uInterpolateY1 = xInterpolateB * _uzInerpolationX + _uzInterpolationY;
				_vInterpolateY1 = xInterpolateB * _vzInterpolationX + _vInterpolationY;


				// Subpixling
				_x1 = xa + dy * _xInterpolate1;

				_z1 = oneOverZA + dy * _zInterpolateY1;
				_u1 = uOneOverZA + dy * _uInterpolateY1;
				_v1 = vOneOverZA + dy * _vInterpolateY1;


				if (yaInt < ybInt)
				{
					_x2 = xa + dy * xInterpolateA;
					_xInterpolate2 = xInterpolateA;

					_y1Int = yaInt;
					_y2Int = ybInt;

					DrawSubTriangleSegment();
				}

				if (ybInt < ycInt)
				{
					_x2 = xb + (1f - (yb - ybInt)) * xInterpolateC;
					_xInterpolate2 = xInterpolateC;

					_y1Int = ybInt;
					_y2Int = ycInt;

					DrawSubTriangleSegment();
				}
			}
			else // Hypotenuse is to the right
			{
				_xInterpolate2 = xInterpolateB;

				_x2 = xa + dy * xInterpolateB;

				if (yaInt < ybInt)
				{
					_xInterpolate1 = xInterpolateA;

					_zInterpolateY1 = _xInterpolate1 * _zInterpolationX + _zInterpolationY;
					_uInterpolateY1 = _xInterpolate1 * _uzInerpolationX + _uzInterpolationY;
					_vInterpolateY1 = _xInterpolate1 * _vzInterpolationX + _vInterpolationY;

					// Subpixling
					_x1 = xa + dy * _xInterpolate1;

					_z1 = oneOverZA + dy * _zInterpolateY1;
					_u1 = uOneOverZA + dy * _uInterpolateY1;
					_v1 = vOneOverZA + dy * _vInterpolateY1;

					_y1Int = yaInt;
					_y2Int = ybInt;

					DrawSubTriangleSegment();
				}
				if (ybInt < ycInt)
				{
					_xInterpolate1 = xInterpolateC;

					_zInterpolateY1 = xInterpolateC * _zInterpolationX + _zInterpolationY;
					_uInterpolateY1 = xInterpolateC * _uzInerpolationX + _uzInterpolationY;
					_vInterpolateY1 = xInterpolateC * _vzInterpolationX + _vInterpolationY;

					dy = 1 - (yb - ybInt);

					// Subpixling
					_x1 = xb + dy * _xInterpolate1;

					_z1 = oneOverZB + dy * _zInterpolateY1;
					_u1 = uOneOverZB + dy * _uInterpolateY1;
					_v1 = vOneOverZB + dy * _vInterpolateY1;

					_y1Int = ybInt;
					_y2Int = ycInt;

					DrawSubTriangleSegment();
				}
			}
		}



		private void DrawSubTriangleSegment()
		{
			var yoffset = BufferContainer.Width * _y1Int;

			for (var y = _y1Int; y < _y2Int; y++)
			{
				if (y >= 0 && y < BufferContainer.Height)
				{
					if ((int)_x1 < (int)_x2)
					{
						_pixelOffset = yoffset + (int)_x1;
						DrawSpan(_depthBuffer,
								 _pixelOffset,
								 _framebuffer,
								 _texture,
								 _texels);
					}
				}
				_x1 += _xInterpolate1;
				_x2 += _xInterpolate2;

				_z1 += _zInterpolateY1;
				_u1 += _uInterpolateY1;
				_v1 += _vInterpolateY1;

				yoffset += BufferContainer.Width;
			}
		}


		protected virtual void DrawSpan(
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

			var dx = 1f - (_x1 - (int)_x1);
			var zz = _z1 + dx * _zInterpolationX;
			var uu = _u1 + dx * _uzInerpolationX;
			var vv = _v1 + dx * _vzInterpolationX;

			var color = (uint)0xff000000;
			var colorAsInt = (int)color;

			var x1Int = (int)_x1;
			var x2Int = (int)_x2;

			for (var x = x1Int; x < x2Int; x++)
			{
				if (x >= 0 && x < BufferContainer.Width)
				{
					z = 1f / zz;
					var bufferZ = (UInt32)((1.0f - z) * (float)UInt32.MaxValue);
					if (bufferZ > depthBuffer[offset] &&
						z >= 0f &&
						z < 1f
						)
					{
						u = uu * z;
						v = vv * z;

						var intu = (int)(u) & (textureWidth - 1);
						var intv = (int)(v) & (textureHeight - 1);

						//var texel = ((intv << texture.WidthBitCount) + intu);

						//framebuffer[offset] = texels[texel] | colorAsInt;
						framebuffer[offset] = texture.Pixels[intu, intv] | colorAsInt;
						//framebuffer[offset] = Bilerp(texture, intu, intv, u, v);
						depthBuffer[offset] = bufferZ;
					}
				}


				offset++;

				zz += _zInterpolationX;
				uu += _uzInerpolationX;
				vv += _vzInterpolationX;
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

			var cr1 = map.Pixels[x, y] | alphaFull;
			var cr2 = map.Pixels[rightOffset, y] | alphaFull;
			var cr3 = map.Pixels[rightOffset, belowOffset] | alphaFull;
			var cr4 = map.Pixels[x, belowOffset] | alphaFull;

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