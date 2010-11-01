using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Debug
{
	public class RectangleDebugShape : DebugShape
	{
		public RectangleDebugShape(IGeometryContext geometryContext)
			: base(geometryContext)
		{
		}


		public void SetRectangle(Vector upperLeft, Vector upperRight, Vector lowerLeft, Vector lowerRight)
		{
			GeometryDetailLevel.AllocateVertices(4);
			GeometryDetailLevel.SetVertex(0, new Vertex(upperLeft.X, upperLeft.Y, upperLeft.Z));
			GeometryDetailLevel.SetVertex(1, new Vertex(upperRight.X, upperRight.Y, upperRight.Z));
			GeometryDetailLevel.SetVertex(2, new Vertex(lowerLeft.X, lowerLeft.Y, lowerLeft.Z));
			GeometryDetailLevel.SetVertex(3, new Vertex(lowerRight.X, lowerRight.Y, lowerRight.Z));

			GeometryDetailLevel.AllocateLines(4);
			GeometryDetailLevel.SetLine(0, new Line(0, 1));
			GeometryDetailLevel.SetLine(1, new Line(2, 3));
			GeometryDetailLevel.SetLine(2, new Line(0, 2));
			GeometryDetailLevel.SetLine(3, new Line(1, 3));
		}

		protected override void Initialize()
		{



			base.Initialize();
		}
	}
}