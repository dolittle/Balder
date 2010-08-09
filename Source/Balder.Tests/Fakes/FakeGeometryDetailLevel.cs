using Balder.Display;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Tests.Fakes
{
	public class FakeGeometryDetailLevel : IGeometryDetailLevel
	{
		private Face[] _faces;
		private Vertex[] _vertices;
		private TextureCoordinate[] _textureCoordinates;
		private Line[] _lines;
		private Normal[] _normals;
		

		public int FaceCount { get; private set; }
		public int VertexCount { get; private set; }
		public int TextureCoordinateCount { get; private set; }
		public int LineCount { get; private set; }
		public int NormalCount { get; private set; }

		public void AllocateFaces(int count)
		{
			_faces = new Face[count];
			FaceCount = count;
		}

		public void SetFace(int index, Face face)
		{
			_faces[index] = face;
		}

		public Face[] GetFaces()
		{
			return _faces;
		}

		public void InvalidateFace(int index)
		{
			
		}

		public void AllocateVertices(int count)
		{
			_vertices = new Vertex[count];
			VertexCount = count;
		}

		public void SetVertex(int index, Vertex vertex)
		{
			_vertices[index] = vertex;
		}

		public Vertex[] GetVertices()
		{
			return _vertices;
		}

		public void InvalidateVertex(int index)
		{
			
		}

		public void AllocateNormals(int count)
		{
			_normals = new Normal[count];
			NormalCount = count;
		}

		public void SetNormal(int index, Normal normal)
		{
			_normals[index] = normal;
		}

		public Normal[] GetNormals()
		{
			return _normals;
		}

		public void InvalidateNormal(int index)
		{
			
		}

		public void AllocateLines(int count)
		{
			_lines = new Line[count];
			LineCount = count;
		}

		public void SetLine(int index, Line line)
		{
			
		}

		public Line[] GetLines()
		{
			return _lines;
		}

		public Face GetFace(int index)
		{
			return _faces[index];
		}

		public Vector GetFaceNormal(int index)
		{
			return Vector.Zero;
		}

		public void AllocateTextureCoordinates(int count)
		{
			_textureCoordinates = new TextureCoordinate[count];
			TextureCoordinateCount = count;
		}

		public void SetTextureCoordinate(int index, TextureCoordinate textureCoordinate)
		{
			_textureCoordinates[index] = textureCoordinate;
		}

		public void SetFaceTextureCoordinateIndex(int index, int a, int b, int c)
		{
			_faces[index].DiffuseA = a;
			_faces[index].DiffuseB = b;
			_faces[index].DiffuseC = c;
			
		}

		public TextureCoordinate[] GetTextureCoordinates()
		{
			return _textureCoordinates;
		}

		public void SetMaterial(int index, Material material)
		{
			
		}

		public void SetMaterialForAllFaces(Material material)
		{
			
		}

		public void CalculateVertices(Viewport viewport, INode node)
		{
			
		}

		public void Render(Viewport viewport, INode node)
		{
			
		}
	}
}