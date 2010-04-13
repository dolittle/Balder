#region License

//
// Author: Einar Ingebrigtsen <einar@dolittle.com>
// Copyright (c) 2007-2009, DoLittle Studios
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

using System.Collections.Generic;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public class MergedGeometry : Node, ICanBeVisible, ICanRender
	{
		public IGeometryContext GeometryContext { get; set; }

		public MergedGeometry()
		{
			// Todo : This should not be necessary.
			if (ObjectFactory.IsObjectFactoryInitialized)
			{
				MakeUnique();
			}
			ContentPrepared += ChildPrepared;
		}


		protected override void Initialize()
		{
			// Todo : This should not be necessary.
			if (null == GeometryContext)
			{
				MakeUnique();
			}

			base.Initialize();
		}

		public void MakeUnique()
		{
			GeometryContext = ObjectFactory.Instance.Get<IGeometryContext>();
		}


		public override void Prepare()
		{
			var nodes = new List<INode>();
			foreach (var item in Items)
			{
				if (item is INode)
				{
					nodes.Add(item as INode);
					if( item is Node )
					{
						((Node) item).Prepare();
					}
				}
			}

			
			var geometryContexts = new List<IGeometryContext>();
			GatherGeometries(nodes, geometryContexts);
			MergeGeometries(geometryContexts);

			base.Prepare();
		}

		private void GatherGeometries(IEnumerable<INode> nodes, IList<IGeometryContext> geometryContexts)
		{
			foreach (var node in nodes)
			{
				if (node is Geometry)
				{
					var context = ((Geometry) node).GeometryContext;
					if (null != context &&
						context.VertexCount > 0)
					{
						geometryContexts.Add(context);
					}
				}

				if (node is IHaveChildren)
				{
					GatherGeometries(((IHaveChildren)node).Children, geometryContexts);
				}
			}
		}

		private void ChildPrepared(INode sender, BubbledEventArgs eventArgs)
		{
			if( !eventArgs.OriginalSource.Equals(this))
			{
				InvalidatePrepare();	
			}
		}


		private void MergeGeometries(IEnumerable<IGeometryContext> geometryContexts)
		{
			var vertexCount = 0;
			var faceCount = 0;
			var textureCoordinateCount = 0;
			var lineCount = 0;
			foreach (var geometryContext in geometryContexts)
			{
				vertexCount += geometryContext.VertexCount;
				faceCount += geometryContext.FaceCount;
				textureCoordinateCount += geometryContext.TextureCoordinateCount;
				lineCount += geometryContext.LineCount;
			}

			GeometryContext.AllocateVertices(vertexCount);
			GeometryContext.AllocateFaces(faceCount);
			GeometryContext.AllocateTextureCoordinates(textureCoordinateCount);
			GeometryContext.AllocateLines(lineCount);

			var vertexOffset = 0;
			var textureCoordinateOffset = 0;
			var faceOffset = 0;
			var lineOffset = 0;

			foreach (var geometryContext in geometryContexts)
			{
				var vertices = geometryContext.GetVertices();
				SetVertices(vertexOffset, vertices);
				var textureCoordinates = geometryContext.GetTextureCoordinates();
				SetTextureCoordinates(textureCoordinateOffset, textureCoordinates);
				var faces = geometryContext.GetFaces();
				SetFaces(vertexOffset, textureCoordinateOffset, faceOffset, faces);
				var lines = geometryContext.GetLines();
				SetLines(vertexOffset, lineOffset, lines);


				vertexOffset += geometryContext.VertexCount;
				textureCoordinateOffset += geometryContext.TextureCoordinateCount;
				faceOffset += geometryContext.FaceCount;
				lineOffset += geometryContext.LineCount;
			}
		}



		private void SetVertices(int vertexOffset, Vertex[] vertices)
		{
			for (var index = 0; index < vertices.Length; index++)
			{
				GeometryContext.SetVertex(index + vertexOffset, vertices[index]);
			}
		}

		private void SetTextureCoordinates(int textureCoordinateOffset, TextureCoordinate[] textureCoordinates)
		{
			for (var index = 0; index < textureCoordinates.Length; index++)
			{
				GeometryContext.SetTextureCoordinate(index + textureCoordinateOffset, textureCoordinates[index]);
			}
		}

		private void SetFaces(int vertexOffset, int textureCoordinateOffset, int faceOffset, Face[] faces)
		{
			for (var index = 0; index < faces.Length; index++)
			{
				var face = faces[index];
				face.A += vertexOffset;
				face.B += vertexOffset;
				face.C += vertexOffset;
				face.DiffuseA += textureCoordinateOffset;
				face.DiffuseB += textureCoordinateOffset;
				face.DiffuseC += textureCoordinateOffset;
				GeometryContext.SetFace(index + faceOffset, face);
			}

		}

		private void SetLines(int vertexOffset, int lineOffset, Line[] lines)
		{
			for (var index = 0; index < lines.Length; index++)
			{
				var line = lines[index];
				line.A += vertexOffset;
				line.B += vertexOffset;
				GeometryContext.SetLine(index + lineOffset, line);
			}
		}

		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			base.BeforeRendering(viewport, view, projection, world);
		}


		public bool IsVisible { get; set; }
		public void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
		}

		public void RenderDebugInfo(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{

		}
	}
}
