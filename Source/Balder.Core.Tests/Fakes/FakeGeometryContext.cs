using System;
using Balder.Core.Display;
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;

namespace Balder.Core.Tests.Fakes
{
	public class FakeGeometryContext : IGeometryContext
	{
		public int FaceCount { get; private set; }
		public int VertexCount { get; private set; }
		public int TextureCoordinateCount { get; private set; }
		public int LineCount { get; private set; }

		public Face[] Faces { get; private set; }
		public Vertex[] Vertices { get; private set; }
		public Line[] Lines { get; private set; }
		public TextureCoordinate[] TextureCoordinates { get; private set; }
		

		public void AllocateFaces(int count)
		{
			Faces = new Face[count];
		}

		public void SetFace(int index, Face face)
		{
			Faces[index] = face;
		}

		public Face[] GetFaces()
		{
			return Faces;
		}

		public void AllocateVertices(int count)
		{
			Vertices = new Vertex[count];
		}

		public void SetVertex(int index, Vertex vertex)
		{
			Vertices[index] = vertex;
		}

		public Vertex[] GetVertices()
		{
			return Vertices;
		}

		public void AllocateLines(int count)
		{
			Lines = new Line[count];
		}

		public void SetLine(int index, Line line)
		{
			Lines[index] = line;
		}

		public Line[] GetLines()
		{
			return Lines;
		}

		public void AllocateTextureCoordinates(int count)
		{
			TextureCoordinates = new TextureCoordinate[count];
		}

		public void SetTextureCoordinate(int index, TextureCoordinate textureCoordinate)
		{
			TextureCoordinates[index] = textureCoordinate;
		}

		public void SetFaceTextureCoordinateIndex(int index, int a, int b, int c)
		{
			throw new NotImplementedException();
		}

		public void SetMaterial(int index, Material material)
		{
			throw new NotImplementedException();
		}

		public void SetMaterialForAllFaces(Material material)
		{
			throw new NotImplementedException();
		}

		public void Render(Viewport viewport, RenderableNode geometry, Matrix view, Matrix projection, Matrix world)
		{
			throw new NotImplementedException();
		}
	}
}
