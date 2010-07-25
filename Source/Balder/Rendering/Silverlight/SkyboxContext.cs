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

using System;
using Balder.Display;
using Balder.Lighting;
using Balder.Math;
using Balder.Objects;
using Balder.Objects.Geometries;

namespace Balder.Rendering.Silverlight
{
	public class SkyboxContext : ISkyboxContext
	{
		public class SkyboxNode : INode
		{
			public SkyboxNode()
			{
				Id = 0;
				ActualWorld = Matrix.Identity;
				RenderingWorld = Matrix.Identity;
			}
			public ushort Id { get; private set; }
			public INode Parent { get; private set; }
			public Matrix ActualWorld { get; private set; }
			public Matrix RenderingWorld { get; set; }
			public Scene Scene { get; set; }
			public void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
			{
				
			}
		}

		private const float XSize = 1;
		private const float YSize = 1;
		private const float ZSize = 1;



		private GeometryDetailLevel _skyboxGeometry;
		private SkyboxNode _node;

		public SkyboxContext(ILightCalculator lightCalculator, IMetaDataPixelBuffer metaDataPixelBuffer)
		{
			_skyboxGeometry = new GeometryDetailLevel(lightCalculator, metaDataPixelBuffer);
			PrepareVertices();
			PrepareFaces();

			_node = new SkyboxNode();
		}

		private void PrepareVertices()
		{
			_skyboxGeometry.AllocateVertices(8);

			_skyboxGeometry.SetVertex(0, new Vertex(-XSize, YSize, ZSize));
			_skyboxGeometry.SetVertex(1, new Vertex(XSize, YSize, ZSize));
			_skyboxGeometry.SetVertex(2, new Vertex(-XSize, -YSize, ZSize));
			_skyboxGeometry.SetVertex(3, new Vertex(XSize, -YSize, ZSize));
			_skyboxGeometry.SetVertex(4, new Vertex(-XSize, YSize, -ZSize));
			_skyboxGeometry.SetVertex(5, new Vertex(XSize, YSize, -ZSize));
			_skyboxGeometry.SetVertex(6, new Vertex(-XSize, -YSize, -ZSize));
			_skyboxGeometry.SetVertex(7, new Vertex(XSize, -YSize, -ZSize));
		}

		private void PrepareFaces()
		{
			_skyboxGeometry.AllocateFaces(2);

			_skyboxGeometry.SetFace(0, new Face(2, 1, 0));
			_skyboxGeometry.SetFace(1, new Face(1, 2, 3));
		}

		public void Render(Viewport viewport)
		{
			_node.Scene = viewport.Scene;
			var viewMatrix = viewport.View.ViewMatrix;
			_skyboxGeometry.Render(viewport, _node);

			
		}
	}
}