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
#if(SILVERLIGHT)
using Balder.Display;
using Balder.Lighting;
using Balder.Materials;
using Balder.Math;
using Balder.Objects;
using Balder.Objects.Geometries;

namespace Balder.Rendering.Silverlight
{
	public class SkyboxContext : ISkyboxContext
	{
		private Material _front;
		private Material _back;
		private Material _top;
		private Material _bottom;
		private Material _left;
		private Material _right;


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

			public BoundingSphere BoundingSphere { get; set; }

			public void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
			{
				
			}
		}

		private const float XSize = 1F;
		private const float YSize = 1F;
		private const float ZSize = 1F;



		private GeometryDetailLevel _skyboxGeometry;
		private SkyboxNode _node;

		public SkyboxContext(ILightCalculator lightCalculator)
		{
			_front = new Material();
			_back = new Material();
			_top = new Material();
			_bottom = new Material();
			_left = new Material();
			_right = new Material();
			_skyboxGeometry = new GeometryDetailLevel(lightCalculator);
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
			_skyboxGeometry.AllocateTextureCoordinates(4);
			_skyboxGeometry.SetTextureCoordinate(0, new TextureCoordinate(0, 0));
			_skyboxGeometry.SetTextureCoordinate(1, new TextureCoordinate(0.99f, 0));
			_skyboxGeometry.SetTextureCoordinate(2, new TextureCoordinate(0, 0.99f));
			_skyboxGeometry.SetTextureCoordinate(3, new TextureCoordinate(0.99f, 0.99f));


			_skyboxGeometry.AllocateFaces(12);

			
			_skyboxGeometry.SetFace(0, new Face(2, 1, 0) { DiffuseA = 2, DiffuseB = 1, DiffuseC = 0, Material = _front });
			_skyboxGeometry.SetFace(1, new Face(1, 2, 3) { DiffuseA = 1, DiffuseB = 2, DiffuseC = 3, Material = _front });

			_skyboxGeometry.SetFace(2, new Face(4, 5, 6) { DiffuseA = 1, DiffuseB = 0, DiffuseC = 3, Material = _back });
			_skyboxGeometry.SetFace(3, new Face(7, 6, 5) { DiffuseA = 2, DiffuseB = 3, DiffuseC = 0, Material = _back });

			_skyboxGeometry.SetFace(4, new Face(6, 0, 4) { DiffuseA = 2, DiffuseB = 1, DiffuseC = 0, Material = _left });
			_skyboxGeometry.SetFace(5, new Face(0, 6, 2) { DiffuseA = 1, DiffuseB = 2, DiffuseC = 3, Material = _left });

			_skyboxGeometry.SetFace(6, new Face(3, 5, 1) { DiffuseA = 2, DiffuseB = 1, DiffuseC = 0, Material = _right });
			_skyboxGeometry.SetFace(7, new Face(5, 3, 7) { DiffuseA = 1, DiffuseB = 2, DiffuseC = 3, Material = _right });

			_skyboxGeometry.SetFace(8, new Face(0, 5, 4) { DiffuseA = 2, DiffuseB = 1, DiffuseC = 0, Material = _top });
			_skyboxGeometry.SetFace(9, new Face(5, 0, 1) { DiffuseA = 1, DiffuseB = 2, DiffuseC = 3, Material = _top });

			_skyboxGeometry.SetFace(10, new Face(6, 3, 2) { DiffuseA = 2, DiffuseB = 1, DiffuseC = 0, Material = _bottom });
			_skyboxGeometry.SetFace(11, new Face(3, 6, 7) { DiffuseA = 1, DiffuseB = 2, DiffuseC = 3, Material = _bottom });
			
		}

		public void Render(Viewport viewport, Skybox skybox)
		{
			_front.DiffuseMap = skybox.Front;
			_back.DiffuseMap = skybox.Back;
			_top.DiffuseMap = skybox.Top;
			_bottom.DiffuseMap = skybox.Bottom;
			_left.DiffuseMap = skybox.Left;
			_right.DiffuseMap = skybox.Right;

			_node.Scene = viewport.Scene;
			
			var viewMatrix = viewport.View.ViewMatrix.Clone();
			viewMatrix.SetTranslation(0,0,1);
			_skyboxGeometry.Render(viewport, _node, viewMatrix, viewport.View.ProjectionMatrix, Matrix.Identity, false);
		}
	}
}
#endif