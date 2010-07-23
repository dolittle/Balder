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
using Balder.Objects.Geometries;
using Balder.Rendering;

namespace Balder.Debug
{
	public class DebugShape : RenderableNode
	{
		private IGeometryContext GeometryContext { get; set; }
		protected IGeometryDetailLevel GeometryDetailLevel { get; private set; }
		
		public DebugShape(IGeometryContext geometryContext, IIdentityManager identityManager)
			: base(identityManager)
		{
			GeometryContext = geometryContext;
			GeometryDetailLevel = GeometryContext.GetDetailLevel(DetailLevel.Full);
		}

		public override void Render(Viewport viewport, DetailLevel detailLevel)
		{
			GeometryDetailLevel.Render(viewport, this);
		}
	}
}
