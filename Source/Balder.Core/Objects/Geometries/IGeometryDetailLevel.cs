using Balder.Core.Display;
using Balder.Core.Materials;

namespace Balder.Core.Objects.Geometries
{
	public interface IGeometryDetailLevel
	{
		int FaceCount { get; }
		int VertexCount { get; }
		int TextureCoordinateCount { get; }
		int LineCount { get; }

		void AllocateFaces(int count);
		void SetFace(int index, Face face);
		Face[] GetFaces();

		void AllocateVertices(int count);
		void SetVertex(int index, Vertex vertex);
		Vertex[] GetVertices();

		void AllocateLines(int count);
		void SetLine(int index, Line line);
		Line[] GetLines();

		void AllocateTextureCoordinates(int count);
		void SetTextureCoordinate(int index, TextureCoordinate textureCoordinate);
		void SetFaceTextureCoordinateIndex(int index, int a, int b, int c);
		TextureCoordinate[] GetTextureCoordinates();

		void SetMaterial(int index, Material material);
		void SetMaterialForAllFaces(Material material);

		void CalculateVertices(Viewport viewport, INode node);
		void Render(Viewport viewport, INode node);
	}
}