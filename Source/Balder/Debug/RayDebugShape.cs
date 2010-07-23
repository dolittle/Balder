#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2010, DoLittle Studios
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

using Balder.Display;
using Balder.Execution;
using Balder.Math;
using Balder.Objects.Geometries;
using Balder.Rendering;

namespace Balder.Debug
{
	public class RayDebugShape : DebugShape
	{
		public RayDebugShape(IGeometryContext geometryContext, IIdentityManager identityManager)
			: base(geometryContext,identityManager)
		{
			
		}

		protected override void Initialize()
		{
			GeometryDetailLevel.AllocateVertices(2);
			GeometryDetailLevel.AllocateLines(1);

			var line = new Line(0, 1);
			GeometryDetailLevel.SetLine(0, line);
			
			base.Initialize();
		}

		public Vector Start { get; set; }
		public Vector Direction { get; set; }


		public override void Render(Viewport viewport, DetailLevel detailLevel)
		{
			var vertex = new Vertex(Start.X, Start.Y, Start.Z);
			GeometryDetailLevel.SetVertex(0, vertex);

			var normalizedDirection = Direction;
			normalizedDirection.Normalize();

			var frustumDepth = viewport.View.Far - viewport.View.Near;
			var end = Start + (normalizedDirection*frustumDepth);

			vertex = new Vertex(end.X,end.Y,end.Z);
			GeometryDetailLevel.SetVertex(1, vertex);

			base.Render(viewport, detailLevel);
		}
	}
}
