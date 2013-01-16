using Balder.Math;
using Balder.Display;
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

        protected float V1InterpolateY;
        protected float U1InterpolateY1;
        protected float V1InterpolateY1;

        protected float V2InterpolateY;
        protected float U2InterpolateY1;
        protected float V2InterpolateY1;


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
    }
}
