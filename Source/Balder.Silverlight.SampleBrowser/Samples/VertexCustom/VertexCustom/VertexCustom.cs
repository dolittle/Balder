using System.Windows.Threading;
using System.Windows;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;
using Balder.Tools;
using System;
using System.Collections.Generic;
using Balder.View;

namespace Balder.Silverlight.SampleBrowser.Samples.VertexCustom.VertexCustom
{
    public class VertexCustom : Geometry
    {
        public VertexCustom()
        {
        }

        public override void Prepare(Display.Viewport viewport)
        {
            base.Prepare(viewport);
        }

        public void GenerateVertices(int nrVertex, int vtxCustom, Vertex vtP1, Vertex vtP2)
        {
            var xAxisStart = new Vertex(-10, 0, 0);
            var xAxisEnd = new Vertex(10, 0, 0);
            var yAxisStart = new Vertex(0, -10, 0);
            var yAxisEnd = new Vertex(0, 10, 0);
            var zAxisStart = new Vertex(0, 0, -10);
            var zAxisEnd = new Vertex(0, 0, 10);

            var vtxP1 = vtP1;
            var vtxP2 = vtP2;

            Dictionary<int, Vertex> vertexAxisXN = new Dictionary<int, Vertex>();
            Dictionary<int, Vertex> vertexAxisXP = new Dictionary<int, Vertex>();
            Dictionary<int, Vertex> vertexAxisYP = new Dictionary<int, Vertex>();
            Dictionary<int, Vertex> vertexAxisYN = new Dictionary<int, Vertex>();
            Dictionary<int, Vertex> vertexAxisZP = new Dictionary<int, Vertex>();
            Dictionary<int, Vertex> vertexAxisZN = new Dictionary<int, Vertex>();

            int m = 0, n = 0;

            for (int i = 1, j = 0; i <= 20; i++, j++)
            {
                if (i % 2 == 0)
                    m++;
                else
                    n++;

                vertexAxisXN.Add(j, new Vertex(i % 2 == 0 ? -m : -n, i % 2 == 0 ? 0.2f : -0.2f, 0));
                vertexAxisXP.Add(j, new Vertex(i % 2 == 0 ? m : n, i % 2 == 0 ? 0.2f : -0.2f, 0));

                vertexAxisYN.Add(j, new Vertex(i % 2 == 0 ? 0.2f : -0.2f, i % 2 == 0 ? -m : -n, 0));
                vertexAxisYP.Add(j, new Vertex(i % 2 == 0 ? 0.2f : -0.2f, i % 2 == 0 ? m : n, 0));

                vertexAxisZN.Add(j, new Vertex(i % 2 == 0 ? 0.2f : -0.2f, 0, i % 2 == 0 ? -m : -n));
                vertexAxisZP.Add(j, new Vertex(i % 2 == 0 ? 0.2f : -0.2f, 0, i % 2 == 0 ? m : n));
            }

            FullDetailLevel.AllocateVertices(nrVertex + vtxCustom);
            FullDetailLevel.SetVertex(0, xAxisStart);
            FullDetailLevel.SetVertex(1, xAxisEnd);
            FullDetailLevel.SetVertex(2, yAxisStart);
            FullDetailLevel.SetVertex(3, yAxisEnd);
            FullDetailLevel.SetVertex(4, zAxisStart);
            FullDetailLevel.SetVertex(5, zAxisEnd);

            Dictionary<int, Vertex>.Enumerator enumDicXN = vertexAxisXN.GetEnumerator();
            while (enumDicXN.MoveNext())
            {
                FullDetailLevel.SetVertex(((KeyValuePair<int, Vertex>)(enumDicXN.Current)).Key + 6, ((KeyValuePair<int, Vertex>)(enumDicXN.Current)).Value);
            }

            foreach (var pair in vertexAxisXP)
            {
                FullDetailLevel.SetVertex((pair.Key) + 26, pair.Value);
            }

            Dictionary<int, Vertex>.Enumerator enumDicYN = vertexAxisYN.GetEnumerator();
            while (enumDicYN.MoveNext())
            {
                FullDetailLevel.SetVertex(((KeyValuePair<int, Vertex>)(enumDicYN.Current)).Key + 46, ((KeyValuePair<int, Vertex>)(enumDicYN.Current)).Value);
            }

            foreach (var pair in vertexAxisYP)
            {
                FullDetailLevel.SetVertex((pair.Key) + 66, pair.Value);
            }

            Dictionary<int, Vertex>.Enumerator enumDicZN = vertexAxisZN.GetEnumerator();
            while (enumDicZN.MoveNext())
            {
                FullDetailLevel.SetVertex(((KeyValuePair<int, Vertex>)(enumDicZN.Current)).Key + 86, ((KeyValuePair<int, Vertex>)(enumDicZN.Current)).Value);
            }

            foreach (var pair in vertexAxisZP)
            {
                FullDetailLevel.SetVertex((pair.Key) + 106, pair.Value);
            }

            FullDetailLevel.SetVertex(126, vtxP1);
            FullDetailLevel.SetVertex(127, vtxP2);
        }

        public void GenerateLines(int nrLines, int nrCustom)
        {
            FullDetailLevel.AllocateLines(nrLines + nrCustom);

            int k = 0, j = 1;

            for (int i = 0; i < (nrLines + nrCustom); i++)
            {
                FullDetailLevel.SetLine(i, new Line(k, j));
                k += 2;
                j += 2;
            }
        }
    }
}
