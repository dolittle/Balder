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

using System.Collections.Generic;
using Balder.Collections;
using Balder.Display;
using Balder.Execution;
using Balder.Math;
using Balder.Rendering;
using Ninject;

namespace Balder.Objects.Geometries
{
	public class MergedGeometry : Node, ICanBeVisible, ICanRender
	{
		private readonly INodeRenderingService _renderingService;

		private IGeometryContext _geometryContext { get; set; }
		private IGeometryDetailLevel _geometryDetailLevel { get; set; }

		private readonly Dictionary<IGeometryDetailLevel, INode> _nodes;

#if(DEFAULT_CONSTRUCTOR)
		public MergedGeometry()
			: this(Runtime.Instance.Kernel.Get<INodeRenderingService>())
		{
			
		}
#endif

		public MergedGeometry(INodeRenderingService renderingService)
		{
			_renderingService = renderingService;
			_nodes = new Dictionary<IGeometryDetailLevel, INode>();

			MakeUnique();
			ContentPrepared += ChildPrepared;
			IsVisible = true;
		}



		public void MakeUnique()
		{
			_geometryContext = Runtime.Instance.Kernel.Get<IGeometryContext>();
			_geometryDetailLevel = _geometryContext.GetDetailLevel(DetailLevel.Full);
		}


		public override void Prepare(Viewport viewport)
		{
			var nodes = new NodeCollection(this);
#if(XAML)
			foreach (var item in Items)
			{
				if (item is INode)
				{
					nodes.Add(item as INode);
				}
			}
#endif

			_renderingService.Prepare(viewport, nodes);
			_renderingService.PrepareForRendering(viewport, nodes);
			
			var geometryDetailLevels = new List<IGeometryDetailLevel>();
			GatherGeometries(nodes, geometryDetailLevels);
			MergeGeometries(viewport, geometryDetailLevels);

			base.Prepare(viewport);
		}

		private void GatherGeometries(IEnumerable<INode> nodes, ICollection<IGeometryDetailLevel> geometryDetailLevels)
		{
			foreach (var node in nodes)
			{
				if (node is Geometry)
				{
					var context = ((Geometry) node).GeometryContext;
					if( null != context )
					{
						var detailLevel = context.GetDetailLevel(DetailLevel.Full);
						if( detailLevel.VertexCount > 0 )
						{

							geometryDetailLevels.Add(detailLevel);
							_nodes[detailLevel] = node;
						}
					}
				}

				if (node is IHaveChildren)
				{
					GatherGeometries(((IHaveChildren)node).Children, geometryDetailLevels);
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


		private void MergeGeometries(Viewport viewport, IEnumerable<IGeometryDetailLevel> geometryDetailLevels)
		{
			var vertexCount = 0;
			var faceCount = 0;
			var textureCoordinateCount = 0;
			var lineCount = 0;
			foreach (var geometryDetailLevel in geometryDetailLevels)
			{
				vertexCount += geometryDetailLevel.VertexCount;
				faceCount += geometryDetailLevel.FaceCount;
				textureCoordinateCount += geometryDetailLevel.TextureCoordinateCount;
				lineCount += geometryDetailLevel.LineCount;
			}

			_geometryDetailLevel.AllocateVertices(vertexCount);
			_geometryDetailLevel.AllocateFaces(faceCount);
			_geometryDetailLevel.AllocateTextureCoordinates(textureCoordinateCount);
			_geometryDetailLevel.AllocateLines(lineCount);

			var vertexOffset = 0;
			var textureCoordinateOffset = 0;
			var faceOffset = 0;
			var lineOffset = 0;

			var view = Matrix.Identity;
			var projection = Matrix.Identity;

			foreach (var geometryDetailLevel in geometryDetailLevels)
			{
				var node = _nodes[geometryDetailLevel];
				geometryDetailLevel.CalculateVertices(viewport, node);
				
				var vertices = geometryDetailLevel.GetVertices();
				if( null != vertices )
				{
					SetVertices(vertexOffset, vertices);	
				}

				var textureCoordinates = geometryDetailLevel.GetTextureCoordinates();
				if (null != textureCoordinates)
				{
					SetTextureCoordinates(textureCoordinateOffset, textureCoordinates);
				}

				var faces = geometryDetailLevel.GetFaces();
				if( null != faces )
				{
					SetFaces(vertexOffset, textureCoordinateOffset, faceOffset, faces);	
				}
				
				var lines = geometryDetailLevel.GetLines();
				if( null != lines )
				{
					SetLines(vertexOffset, lineOffset, lines);	
				}

				vertexOffset += geometryDetailLevel.VertexCount;
				textureCoordinateOffset += geometryDetailLevel.TextureCoordinateCount;
				faceOffset += geometryDetailLevel.FaceCount;
				lineOffset += geometryDetailLevel.LineCount;
			}
		}



		private void SetVertices(int vertexOffset, Vertex[] vertices)
		{
			for (var index = 0; index < vertices.Length; index++)
			{
				//var vector = vertices[index].TransformedVector;
				//var vertex = new Vertex(vector.X, vector.Y, vector.Z);
				//_geometryDetailLevel.SetVertex(index + vertexOffset, vertex); //vertices[index]);
			}
		}

		private void SetTextureCoordinates(int textureCoordinateOffset, TextureCoordinate[] textureCoordinates)
		{
			for (var index = 0; index < textureCoordinates.Length; index++)
			{
				_geometryDetailLevel.SetTextureCoordinate(index + textureCoordinateOffset, textureCoordinates[index]);
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
				_geometryDetailLevel.SetFace(index + faceOffset, face);
			}

		}

		private void SetLines(int vertexOffset, int lineOffset, Line[] lines)
		{
			for (var index = 0; index < lines.Length; index++)
			{
				var line = lines[index];
				line.A += vertexOffset;
				line.B += vertexOffset;
				_geometryDetailLevel.SetLine(index + lineOffset, line);
			}
		}


		public bool IsVisible { get; set; }
		public void Render(Viewport viewport, DetailLevel detailLevel)
		{
			_geometryContext.Render(viewport, this, detailLevel);
		}

		public void RenderDebugInfo(Viewport viewport, DetailLevel detailLevel)
		{

		}
	}
}
