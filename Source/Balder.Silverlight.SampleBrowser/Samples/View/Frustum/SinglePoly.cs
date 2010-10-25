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
			Vertex3 = new Coordinate(-4, -4, 0);
			Vertex4 = new Coordinate(4, -4, 0);
			Vertex5 = new Coordinate(0, 0, 0);
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

		public static readonly Property<SinglePoly, Coordinate> Vertex4Property =
			Property<SinglePoly, Coordinate>.Register(s => s.Vertex4, VertexChanged);

		public Coordinate Vertex4
		{
			get { return Vertex4Property.GetValue(this); }
			set { Vertex4Property.SetValue(this, value); }
		}

		public static readonly Property<SinglePoly, Coordinate> Vertex5Property =
			Property<SinglePoly, Coordinate>.Register(s => s.Vertex5, VertexChanged);

		public Coordinate Vertex5
		{
			get { return Vertex5Property.GetValue(this); }
			set { Vertex5Property.SetValue(this, value); }
		}

		private static void VertexChanged(object owner, Coordinate oldCoordinate, Coordinate newCoordinate)
		{
			((SinglePoly)owner).InvalidatePrepare();
		}

		public override void Prepare(Display.Viewport viewport)
		{
			FullDetailLevel.AllocateVertices(5);
			FullDetailLevel.SetVertex(0, new Vertex((float)Vertex1.X, (float)Vertex1.Y, (float)Vertex1.Z) { Color = Colors.Red });
			FullDetailLevel.SetVertex(1, new Vertex((float)Vertex2.X, (float)Vertex2.Y, (float)Vertex2.Z) { Color = Colors.Blue });
			FullDetailLevel.SetVertex(2, new Vertex((float)Vertex3.X, (float)Vertex3.Y, (float)Vertex3.Z) { Color = Colors.Green });
			FullDetailLevel.SetVertex(3, new Vertex((float)Vertex4.X, (float)Vertex4.Y, (float)Vertex4.Z) { Color = Colors.Orange });
			FullDetailLevel.SetVertex(4, new Vertex((float)Vertex5.X, (float)Vertex5.Y, (float)Vertex5.Z) { Color = Colors.White });

			FullDetailLevel.AllocateTextureCoordinates(5);
			FullDetailLevel.SetTextureCoordinate(0, new TextureCoordinate(0, 0));
			FullDetailLevel.SetTextureCoordinate(1, new TextureCoordinate(1, 0));
			FullDetailLevel.SetTextureCoordinate(2, new TextureCoordinate(0, 1));
			FullDetailLevel.SetTextureCoordinate(3, new TextureCoordinate(1, 1));
			FullDetailLevel.SetTextureCoordinate(4, new TextureCoordinate(0.5f, 0.5f));

			FullDetailLevel.AllocateFaces(4);
			FullDetailLevel.SetFace(0, new Face(0, 1, 4) { ColorA = Colors.Red, ColorB = Colors.Green, ColorC = Colors.White });
			FullDetailLevel.SetFace(1, new Face(2, 3, 4) { ColorA = Colors.Blue, ColorB = Colors.Orange, ColorC = Colors.White });
			FullDetailLevel.SetFace(2, new Face(0, 2, 4) { ColorA = Colors.Red, ColorB = Colors.Blue, ColorC = Colors.White });
			FullDetailLevel.SetFace(3, new Face(1, 3, 4) { ColorA = Colors.Green, ColorB = Colors.Orange, ColorC = Colors.White });

			FullDetailLevel.SetFaceTextureCoordinateIndex(0, 0, 1, 4);
			FullDetailLevel.SetFaceTextureCoordinateIndex(1, 2, 3, 4);
			FullDetailLevel.SetFaceTextureCoordinateIndex(2, 0, 2, 4);
			FullDetailLevel.SetFaceTextureCoordinateIndex(3, 1, 3, 4);
			
			base.Prepare(viewport);
		}

	}
}
