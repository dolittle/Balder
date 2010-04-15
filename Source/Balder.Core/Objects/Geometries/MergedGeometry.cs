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
using Balder.Core.Collections;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;
using Balder.Core.Rendering;

namespace Balder.Core.Objects.Geometries
{
	public class MergedGeometry : Node, ICanBeVisible, ICanRender
	{
		private readonly INodeRenderingService _renderingService;

		public IGeometryContext GeometryContext { get; set; }

		private readonly Dictionary<IGeometryContext, INode> _nodes;

		public MergedGeometry()
			: this(KernelContainer.Kernel.Get<INodeRenderingService>())
		{
			
		}

		public MergedGeometry(INodeRenderingService renderingService)
		{
			_renderingService = renderingService;
			_nodes = new Dictionary<IGeometryContext, INode>();

			// Todo : This should not be necessary.
			if (ObjectFactory.IsObjectFactoryInitialized)
			{
				MakeUnique();
			}
			ContentPrepared += ChildPrepared;
			IsVisible = true;
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


		public override void Prepare(Viewport viewport)
		{
			var nodes = new NodeCollection(this);
			foreach (var item in Items)
			{
				if (item is INode)
				{
					nodes.Add(item as INode);
				}
			}

			_renderingService.Prepare(viewport, nodes);
			_renderingService.PrepareForRendering(viewport, nodes);
			
			var geometryContexts = new List<IGeometryContext>();
			GatherGeometries(nodes, geometryContexts);
			MergeGeometries(viewport, geometryContexts);

			base.Prepare(viewport);
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
						_nodes[context] = node;
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


		private void MergeGeometries(Viewport viewport, IEnumerable<IGeometryContext> geometryContexts)
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

			var view = Matrix.Identity;
			var projection = Matrix.Identity;

			foreach (var geometryContext in geometryContexts)
			{
				var node = _nodes[geometryContext];
				geometryContext.CalculateVertices(viewport, node, view, projection, node.ActualWorld);
				
				var vertices = geometryContext.GetVertices();
				if( null != vertices )
				{
					SetVertices(vertexOffset, vertices);	
				}

				var textureCoordinates = geometryContext.GetTextureCoordinates();
				if (null != textureCoordinates)
				{
					SetTextureCoordinates(textureCoordinateOffset, textureCoordinates);
				}

				var faces = geometryContext.GetFaces();
				if( null != faces )
				{
					SetFaces(vertexOffset, textureCoordinateOffset, faceOffset, faces);	
				}
				
				var lines = geometryContext.GetLines();
				if( null != lines )
				{
					SetLines(vertexOffset, lineOffset, lines);	
				}

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
				var vector = vertices[index].TransformedVector;
				var vertex = new Vertex(vector.X, vector.Y, vector.Z);
				GeometryContext.SetVertex(index + vertexOffset, vertex); //vertices[index]);
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


		public bool IsVisible { get; set; }
		public void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			GeometryContext.Render(viewport, this, view, projection, world);
		}

		public void RenderDebugInfo(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{

		}
	}
}
