using Balder.Execution;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.View.Frustum
{
	public class SinglePoly : Geometry
	{

		public SinglePoly()
		{
			Vertex1 = new Coordinate(-4, 4, 0);
			Vertex2 = new Coordinate(4, 4, 0);
			Vertex3 = new Coordinate(0, 0, 0);
		}

		public static readonly Property<SinglePoly, Coordinate> Vertex1Property =
			Property<SinglePoly, Coordinate>.Register(s => s.Vertex1, VertexChanged);

		public Coordinate Vertex1
		{
			get { return Vertex1Property.GetValue(this); }
			set { Vertex1Property.SetValue(this, value); }
		}

		public static readonly Property<SinglePoly, Coordinate> Vertex2Property =
			Property<SinglePoly, Coordinate>.Register(s => s.Vertex2, VertexChanged);

		public Coordinate Vertex2
		{
			get { return Vertex2Property.GetValue(this); }
			set { Vertex2Property.SetValue(this, value); }
		}


		public static readonly Property<SinglePoly, Coordinate> Vertex3Property =
			Property<SinglePoly, Coordinate>.Register(s => s.Vertex3, VertexChanged);

		public Coordinate Vertex3
		{
			get { return Vertex3Property.GetValue(this); }
			set { Vertex3Property.SetValue(this, value); }
		}

		private static void VertexChanged(object owner, Coordinate oldCoordinate, Coordinate newCoordinate)
		{
			((SinglePoly)owner).InvalidatePrepare();
		}

		public override void Prepare(Display.Viewport viewport)
		{
			FullDetailLevel.AllocateVertices(3);
			FullDetailLevel.SetVertex(0, new Vertex((float)Vertex1.X, (float)Vertex1.Y, (float)Vertex1.Z) { Color = Colors.Red });
			FullDetailLevel.SetVertex(1, new Vertex((float)Vertex2.X, (float)Vertex2.Y, (float)Vertex2.Z) { Color = Colors.Blue });
			FullDetailLevel.SetVertex(2, new Vertex((float)Vertex3.X, (float)Vertex3.Y, (float)Vertex3.Z) { Color = Colors.Green });

			

			FullDetailLevel.AllocateTextureCoordinates(3);
			FullDetailLevel.SetTextureCoordinate(0, new TextureCoordinate(0, 0));
			FullDetailLevel.SetTextureCoordinate(1, new TextureCoordinate(1, 0));
			FullDetailLevel.SetTextureCoordinate(2, new TextureCoordinate(0, 1));

			FullDetailLevel.AllocateFaces(1);
			FullDetailLevel.SetFace(0, new Face(0, 1, 2) { ColorA = Colors.Red, ColorB = Colors.Green, ColorC = Colors.Blue });

			FullDetailLevel.SetFaceTextureCoordinateIndex(0, 0, 1, 2);

			base.Prepare(viewport);
		}

	}
}
