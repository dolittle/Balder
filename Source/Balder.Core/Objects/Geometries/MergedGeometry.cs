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


		protected override void Prepare()
		{
			var nodes = new List<INode>();
			foreach( var item in Items )
			{
				if( item is INode )
				{
					nodes.Add(item as INode);
				}
			}

			var geometryContexts = new List<IGeometryContext>();
			GatherGeometries(nodes,geometryContexts);
			MergeGeometries(geometryContexts);

			base.Prepare();
		}

		private void GatherGeometries(IEnumerable<INode> nodes, IList<IGeometryContext> geometryContexts)
		{
			foreach( var node in nodes )
			{
				if( node is Geometry )
				{
					geometryContexts.Add(((Geometry)node).GeometryContext);
				}

				if( node is IHaveChildren )
				{
					GatherGeometries(((IHaveChildren)node).Children,geometryContexts);
				}
			}

			
		}

		private void MergeGeometries(IEnumerable<IGeometryContext> geometryContexts)
		{
			
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
