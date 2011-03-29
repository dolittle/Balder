using System.Windows.Threading;
using System.Windows;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;
using Balder.Tools;
using System;

namespace Balder.Silverlight.SampleBrowser.Samples.Custom.VertexCustom
{
	public class VertexCustom : Geometry
	{
        public VertexCustom()
		{
		}

		public override void Prepare(Display.Viewport viewport)
		{
			GenerateVertices();
			GenerateLines();
			//GenerateFaces();

			//GeometryHelper.CalculateNormals(FullDetailLevel);

			base.Prepare(viewport);

            TempData.Changed = true;
            TempData.Y = 99;
            TempData.Changed = false;

            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
		}

        void timer_Tick(object sender, EventArgs e)
        {
            Rotation.Y += 0.5f;   
        }

		private void GenerateVertices()
		{
			var dimensionAsVector = new Vector(5f, 5f, 5f);
			var halfDimension = dimensionAsVector / 2f;

			var xAxisStart = new Vertex(-10, 0, 0);
            var xAxisEnd = new Vertex(10, 0, 0);
            var yAxisStart = new Vertex(0, -10, 0);
            var yAxisEnd = new Vertex(0, 10, 0);
            var zAxisStart = new Vertex(0, 0, -10);
            var zAxisEnd = new Vertex(0, 0, 10);
			//var frontLowerLeft = new Vertex(-halfDimension.X, -halfDimension.Y, -halfDimension.Z);
			

			FullDetailLevel.AllocateVertices(6);
            FullDetailLevel.SetVertex(0, xAxisStart);
            FullDetailLevel.SetVertex(1, xAxisEnd);
            FullDetailLevel.SetVertex(2, yAxisStart);
            FullDetailLevel.SetVertex(3, yAxisEnd);
            FullDetailLevel.SetVertex(4, zAxisStart);
            FullDetailLevel.SetVertex(5, zAxisEnd);
			//FullDetailLevel.SetVertex(2, frontLowerLeft);
			
		}

		private void GenerateLines()
		{
			FullDetailLevel.AllocateLines(3);
			FullDetailLevel.SetLine(0, new Line(0, 1));
			FullDetailLevel.SetLine(1, new Line(2, 3));
            FullDetailLevel.SetLine(2, new Line(4, 5));
			//FullDetailLevel.SetLine(2, new Line(0, 2));
			//FullDetailLevel.SetLine(3, new Line(1, 3));

			
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
