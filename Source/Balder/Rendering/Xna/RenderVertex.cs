using Balder.Objects.Geometries;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Balder.Rendering.Xna
{
    public struct RenderVertex : IVertexType
    {
        private Vector3 _position;
        private Vector3 _normal;
        private Microsoft.Xna.Framework.Color _color;

        public RenderVertex(Vertex vertex)
        {
            _position = new Vector3(vertex.X,vertex.Y,vertex.Z);
            _normal = new Vector3(vertex.NormalX, vertex.NormalY, vertex.NormalZ);
            _color = Microsoft.Xna.Framework.Color.Blue;
        }

        public RenderVertex(Vertex vertex, Color color)
        {
            _position = new Vector3(vertex.X, vertex.Y, vertex.Z);
            _normal = new Vector3(vertex.NormalX, vertex.NormalY, vertex.NormalZ);
            _color = color;
        }

        public RenderVertex(Vector3 position, Microsoft.Xna.Framework.Color color)
        {
            _position = position;
            _color = color;
            _normal = Vector3.Zero;
        }


        public static VertexElement[] VertexElements = {
                                                           new VertexElement(0, VertexElementFormat.Vector3,VertexElementUsage.Position, 0), 
                                                           new VertexElement(sizeof(float)*3, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                                                           new VertexElement(sizeof(float)*6,VertexElementFormat.Color,VertexElementUsage.Color,0), 
                                                       };

        public static VertexDeclaration Declaration = new VertexDeclaration(VertexElements);

        public VertexDeclaration VertexDeclaration
        {
            get { return Declaration; }
        }
    }
}