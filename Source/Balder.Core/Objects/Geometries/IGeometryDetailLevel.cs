using Balder.Core.Display;
using Balder.Core.Materials;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public interface IGeometryDetailLevel
	{
		int FaceCount { get; }
		int VertexCount { get; }
		int TextureCoordinateCount { get; }
		int LineCount { get; }
		int NormalCount { get; }

		void AllocateFaces(int count);
		void SetFace(int index, Face face);
		Face[] GetFaces();
		void InvalidateFace(int index);

		void AllocateVertices(int count);
		void SetVertex(int index, Vertex vertex);
		Vertex[] GetVertices();
		void InvalidateVertex(int index);

		void AllocateNormals(int count);
		void SetNormal(int index, Vertex normal);
		Vertex[] GetNormals();
		void InvalidateNormal(int index);

		void AllocateLines(int count);
		void SetLine(int index, Line line);
		Line[] GetLines();

		Face GetFace(int index);
		Vector GetFaceNormal(int index);

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