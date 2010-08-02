using Balder.Execution;
using Balder.Materials;
using Balder.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Events.FacePicker
{
	public class PickedFaceVisualizer : Geometry
	{
		private Material _material;

		public PickedFaceVisualizer()
		{
			FaceIndex = -1;
			_material = new Material();
			_material.Diffuse = Colors.Green;
			_material.Shade = MaterialShade.None;
			Color = _material.Diffuse;
			Material = _material;
		}


		public static readonly Property<PickedFaceVisualizer, Geometry> GeometryToVisualizeProperty =
			Property<PickedFaceVisualizer, Geometry>.Register(p => p.GeometryToVisualize);
		public Geometry GeometryToVisualize
		{
			get { return GeometryToVisualizeProperty.GetValue(this); }
			set { GeometryToVisualizeProperty.SetValue(this, value); }
		}

		private int _faceIndex;
		public int FaceIndex
		{
			get { return _faceIndex; }
			set
			{
				_faceIndex = value;


				if (value == -1)
				{
					IsVisible = false;
				}
				else
				{
					IsVisible = true;
					InvalidatePrepare();
				}
			}
		}


		public override void Prepare(Display.Viewport viewport)
		{
			var faceIndex = FaceIndex;
			if (null != GeometryToVisualize && faceIndex != -1)
			{
				var vertices = GeometryToVisualize.FullDetailLevel.GetVertices();
				var faces = GeometryToVisualize.FullDetailLevel.GetFaces();

				if (null != vertices && null != faces)
				{
					FullDetailLevel.AllocateVertices(3);
					FullDetailLevel.AllocateLines(3);

					var face = faces[faceIndex];
					var vertex1 = vertices[face.A];
					var vertex2 = vertices[face.B];
					var vertex3 = vertices[face.C];

					FullDetailLevel.SetVertex(0, vertex1);
					FullDetailLevel.SetVertex(1, vertex2);
					FullDetailLevel.SetVertex(2, vertex3);
					
					FullDetailLevel.SetLine(0, CreateLine(0, 1));
					FullDetailLevel.SetLine(1, CreateLine(1, 2));
					FullDetailLevel.SetLine(2, CreateLine(2, 0));

				}
			}

			base.Prepare(viewport);
		}

		private Line CreateLine(int a, int b)
		{
			var line = new Line(a, b) { Color = _material.Diffuse };
			return line;
		}
	}
}
