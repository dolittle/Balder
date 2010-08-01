using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Geometries.Custom
{
	public class MyBox : Geometry
	{
		public MyBox()
		{
		}

		public override void Prepare(Display.Viewport viewport)
		{
			GenerateVertices();
			//GenerateLines();
			GenerateFaces();

			GeometryHelper.CalculateFaceNormals(FullDetailLevel);
			GeometryHelper.CalculateVertexNormals(FullDetailLevel);

			base.Prepare(viewport);
		}

		private void GenerateVertices()
		{
			var dimensionAsVector = new Vector(5f, 5f, 5f);
			var halfDimension = dimensionAsVector / 2f;

			var frontUpperLeft = new Vertex(-halfDimension.X, halfDimension.Y, -halfDimension.Z);
			var frontUpperRight = new Vertex(halfDimension.X, halfDimension.Y, -halfDimension.Z);
			var frontLowerLeft = new Vertex(-halfDimension.X, -halfDimension.Y, -halfDimension.Z);
			var frontLowerRight = new Vertex(halfDimension.X, -halfDimension.Y, -halfDimension.Z);

			var backUpperLeft = new Vertex(-halfDimension.X, halfDimension.Y, halfDimension.Z);
			var backUpperRight = new Vertex(halfDimension.X, halfDimension.Y, halfDimension.Z);
			var backLowerLeft = new Vertex(-halfDimension.X, -halfDimension.Y, halfDimension.Z);
			var backLowerRight = new Vertex(halfDimension.X, -halfDimension.Y, halfDimension.Z);

			FullDetailLevel.AllocateVertices(8);
			FullDetailLevel.SetVertex(0, frontUpperLeft);
			FullDetailLevel.SetVertex(1, frontUpperRight);
			FullDetailLevel.SetVertex(2, frontLowerLeft);
			FullDetailLevel.SetVertex(3, frontLowerRight);

			FullDetailLevel.SetVertex(4, backUpperLeft);
			FullDetailLevel.SetVertex(5, backUpperRight);
			FullDetailLevel.SetVertex(6, backLowerLeft);
			FullDetailLevel.SetVertex(7, backLowerRight);
		}

		private void GenerateLines()
		{
			FullDetailLevel.AllocateLines(12);
			FullDetailLevel.SetLine(0, new Line(0, 1));
			FullDetailLevel.SetLine(1, new Line(2, 3));
			FullDetailLevel.SetLine(2, new Line(0, 2));
			FullDetailLevel.SetLine(3, new Line(1, 3));

			FullDetailLevel.SetLine(4, new Line(4, 5));
			FullDetailLevel.SetLine(5, new Line(6, 7));
			FullDetailLevel.SetLine(6, new Line(4, 6));
			FullDetailLevel.SetLine(7, new Line(5, 7));

			FullDetailLevel.SetLine(8, new Line(0, 4));
			FullDetailLevel.SetLine(9, new Line(1, 5));
			FullDetailLevel.SetLine(10, new Line(2, 6));
			FullDetailLevel.SetLine(11, new Line(3, 7));
		}

		private void GenerateFaces()
		{
			FullDetailLevel.AllocateFaces(12);

			FullDetailLevel.SetFace(0, new Face(2,1,0));
			FullDetailLevel.SetFace(1, new Face(1,2,3));

			FullDetailLevel.SetFace(2, new Face(4,5,6));
			FullDetailLevel.SetFace(3, new Face(7,6,5));

			FullDetailLevel.SetFace(4, new Face(0,4,2));
			FullDetailLevel.SetFace(5, new Face(6,2,4));

			FullDetailLevel.SetFace(6, new Face(3,5,1));
			FullDetailLevel.SetFace(7, new Face(5,3,7));

			FullDetailLevel.SetFace(8, new Face(0,1,4));
			FullDetailLevel.SetFace(9, new Face(5,4,1));

			FullDetailLevel.SetFace(10, new Face(6,3,2));
			FullDetailLevel.SetFace(11, new Face(3,6,7));
		}

		

	}
}
