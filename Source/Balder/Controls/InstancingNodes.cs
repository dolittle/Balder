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
using System;
using System.Collections;
using System.Windows;
using Balder.Display;
using Balder.Execution;
using Balder.Rendering;
using Ninject;

namespace Balder.Controls
{
	public class InstancingNodes : RenderableNode
	{
		private readonly INodeRenderingService _nodeRenderingService;

		public InstancingNodes()
			: this(Runtime.Instance.Kernel.Get<INodeRenderingService>())
		{
			
		}

		public InstancingNodes(INodeRenderingService nodeRenderingService)
		{
			_nodeRenderingService = nodeRenderingService;
		}


		public static readonly Property<InstancingNodes, IEnumerable> DataProperty =
			Property<InstancingNodes, IEnumerable>.Register(i => i.Data);

		public IEnumerable Data
		{
			get { return DataProperty.GetValue(this); }
			set { DataProperty.SetValue(this, value); }
		}

		public static readonly Property<InstancingNodes, DataTemplate> NodeTemplateProperty =
			Property<InstancingNodes, DataTemplate>.Register(n => n.NodeTemplate);
		public DataTemplate NodeTemplate
		{
			get { return base.ItemTemplate; }
			set
			{
				base.ItemTemplate = value;
				LoadTemplate();
			}
		}

		private RenderableNode _actualNodeTemplate;
		private bool _templatePrepared;

		private void LoadTemplate()
		{
			var templateContent = NodeTemplate.LoadContent();
			if (!(templateContent is RenderableNode))
			{
				throw new ArgumentException("NodeTemplate must be of a RenderableNode type");
			}
			_actualNodeTemplate = templateContent as RenderableNode;
			_templatePrepared = false;
		}

		public override void Prepare(Viewport viewport)
		{
			if (!_templatePrepared && null != _actualNodeTemplate)
			{
				_nodeRenderingService.PrepareNode(viewport, _actualNodeTemplate);
			}
			base.Prepare(viewport);
		}

		public override void Render(Viewport viewport, DetailLevel detailLevel)
		{
			if (null != Data && null != _actualNodeTemplate)
			{
				foreach( var item in Data )
				{
					_actualNodeTemplate.DataItem = item;
					_actualNodeTemplate.PrepareActualWorld();
					_actualNodeTemplate.RenderingWorld = _actualNodeTemplate.ActualWorld;
					_nodeRenderingService.PrepareNodeForRendering(_actualNodeTemplate, viewport);
					_nodeRenderingService.RenderNode(_actualNodeTemplate, viewport, detailLevel);
				}
			}

			base.Render(viewport, detailLevel);
		}

	}
}
#endif