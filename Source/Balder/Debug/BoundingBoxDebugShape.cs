#region License
//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2011, DoLittle Studios
//
// Licensed under the Microsoft Permissive License (Ms-PL), Version 1.1 (the "License")
// you may not use this file except in compliance with the License.
// You may obtain a copy of the license at 
//
//   http://balder.codeplex.com/license
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
#endregion

using Balder.Objects.Geometries;

namespace Balder.Debug
{
	public class BoundingBoxDebugShape : DebugShape
	{
        public BoundingBoxDebugShape(IGeometryContext geometryContext)
			: base(geometryContext)
		{
			
		}

		protected override void Initialize()
		{
			InitializeVertices();
			InitializeLines();
		}

		private void InitializeVertices()
		{
			GeometryDetailLevel.AllocateVertices(8);

            GeometryDetailLevel.SetVertex(0,new Vertex(-1,1,-1));
            GeometryDetailLevel.SetVertex(1,new Vertex(1,1,-1));
            GeometryDetailLevel.SetVertex(2,new Vertex(-1,-1,-1));
            GeometryDetailLevel.SetVertex(3,new Vertex(1,-1,-1));
            GeometryDetailLevel.SetVertex(4,new Vertex(-1,1,1));
            GeometryDetailLevel.SetVertex(5,new Vertex(1,1,1));
            GeometryDetailLevel.SetVertex(6,new Vertex(-1,-1,1));
            GeometryDetailLevel.SetVertex(7,new Vertex(1,-1,1));
		}

		private void InitializeLines()
		{
			GeometryDetailLevel.AllocateLines(12);

            GeometryDetailLevel.SetLine(0,new Line(0,1));
            GeometryDetailLevel.SetLine(1,new Line(1,3));
            GeometryDetailLevel.SetLine(2,new Line(3,2));
            GeometryDetailLevel.SetLine(3,new Line(2,0));

            GeometryDetailLevel.SetLine(4,new Line(4,5));
            GeometryDetailLevel.SetLine(5,new Line(5,7));
            GeometryDetailLevel.SetLine(6,new Line(7,6));
            GeometryDetailLevel.SetLine(7,new Line(6,4));

            GeometryDetailLevel.SetLine(8,new Line(0,4));
            GeometryDetailLevel.SetLine(9,new Line(2,6));
            GeometryDetailLevel.SetLine(10,new Line(1,5));
            GeometryDetailLevel.SetLine(11,new Line(3,7));
		}
	}
}
