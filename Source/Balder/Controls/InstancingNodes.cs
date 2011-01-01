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
#if(SILVERLIGHT)
using System;
using System.Collections;
using System.Windows;
using Balder.Display;
using Balder.Execution;
using Balder.Input;
using Balder.Math;
using Balder.Rendering;
using Ninject;

namespace Balder.Controls
{
	public class InstancingNodes : RenderableNode, ICanGetNodeAtPosition
	{
		private class DataItemInfo
		{
			public Vector Position;
			public Vector Scale;
			public Vector Rotation;
			public Matrix Matrix;
			public object DataItem;
			public BoundingSphere BoundingSphere;
		}

		private readonly INodeRenderingService _nodeRenderingService;
		private DataItemInfo[] _dataItemInfos;
		private bool _boundingSpheresPrepared;


		public InstancingNodes()
			: this(Runtime.Instance.Kernel.Get<INodeRenderingService>())
		{

		}

		public InstancingNodes(INodeRenderingService nodeRenderingService)
		{
			_nodeRenderingService = nodeRenderingService;
			_boundingSpheresPrepared = false;
		}


		public static readonly Property<InstancingNodes, IEnumerable> DataProperty =
			Property<InstancingNodes, IEnumerable>.Register(i => i.Data);

		public IEnumerable Data
		{
			get { return DataProperty.GetValue(this); }
			set
			{
				DataProperty.SetValue(this, value);
				PrepareDataItemInfos(value);
				_boundingSpheresPrepared = false;
			}
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
				_boundingSpheresPrepared = false;

			}
		}

		private int GetDataLength(IEnumerable enumerable)
		{
			var length = 0;
			if (enumerable is Array)
			{
				length = ((Array)enumerable).Length;
			}
			else if (enumerable is ICollection)
			{
				length = ((ICollection)enumerable).Count;
			}

			return length;
		}


		private void PrepareDataItemInfos(IEnumerable enumerable)
		{
			var length = GetDataLength(enumerable);
			_dataItemInfos = new DataItemInfo[length];
			var index = 0;
			foreach (var item in enumerable)
			{
				var info = new DataItemInfo { DataItem = item };
				_dataItemInfos[index] = info;
				index++;
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
			if (null != Data && null != _actualNodeTemplate && null != _dataItemInfos)
			{
				var length = GetDataLength(Data);
				if( length != _dataItemInfos.Length )
				{
					PrepareDataItemInfos(Data);
				}

				for (var index = 0; index < _dataItemInfos.Length; index++)
				{
					_actualNodeTemplate.DataItem = _dataItemInfos[index].DataItem;
					_actualNodeTemplate.ActualWorld = GetRenderingWorld(_dataItemInfos[index], _actualNodeTemplate);
					_nodeRenderingService.PrepareNodeForRendering(_actualNodeTemplate, viewport);
					_nodeRenderingService.RenderNode(_actualNodeTemplate, viewport, detailLevel);
				}

				if( !_boundingSpheresPrepared )
				{
					PrepareMergedBoundingSphere();
				}
			}

			base.Render(viewport, detailLevel);
		}

		private Matrix GetRenderingWorld(DataItemInfo info, RenderableNode template)
		{
			Matrix matrix;
			if ((!info.Position.Equals(template.Position)) ||
				(!info.Rotation.Equals(template.Rotation)) ||
				(!info.Scale.Equals(template.Scale)))
			{
				_actualNodeTemplate.PrepareActualWorld();
				matrix = _actualNodeTemplate.ActualWorld;
				SetDataItemInfo(info, template);
			}
			else
			{
				matrix = info.Matrix;
			}

			return matrix;
		}

		private void SetDataItemInfo(DataItemInfo info, RenderableNode template)
		{
			info.Position = template.Position;
			info.Rotation = template.Rotation;
			info.Scale = template.Scale;
			info.Matrix = template.ActualWorld;
		}


		private void PrepareMergedBoundingSphere()
		{
			if( null == _actualNodeTemplate || null == _dataItemInfos)
			{
				return;
			}
			_actualNodeTemplate.PrepareBoundingSphere();
			for (var index = 0; index < _dataItemInfos.Length; index++)
			{
				var boundingSphere = _actualNodeTemplate.BoundingSphere.Transform(_dataItemInfos[index].Matrix);
				BoundingSphere = BoundingSphere.CreateMerged(BoundingSphere, boundingSphere);
			}
			_boundingSpheresPrepared = true;

			ActualBoundingSphere = BoundingSphere.Transform(ActualWorld);
		}


		public override float? Intersects(Viewport viewport, Ray pickRay)
		{
			RenderableNode node = null;
			float? distance = null;
			GetNodeAtPosition(viewport,pickRay, ref node, ref distance);
			return distance;
		}


		public void GetNodeAtPosition(Viewport viewport, Ray pickRay, ref RenderableNode closestNode, ref float? closestDistance)
		{
			closestDistance = null;
			closestNode = null;
			var distance = pickRay.Intersects(ActualBoundingSphere);
			if (null != distance)
			{
				var closestIndex = 0;
				for (var index = 0; index < _dataItemInfos.Length; index++)
				{
					_actualNodeTemplate.ActualWorld = _dataItemInfos[index].Matrix;
					if (_actualNodeTemplate.IsIntersectionTestEnabled)
					{
						_nodeRenderingService.PrepareNodeForRendering(_actualNodeTemplate, viewport);
						distance = _actualNodeTemplate.Intersects(viewport, pickRay);
						if (null != distance && (distance < closestDistance || closestDistance == null))
						{
							closestDistance = distance;
							closestIndex = index;

						}
					}
				}

				if (null != closestDistance)
				{
					_actualNodeTemplate.ActualWorld = _dataItemInfos[closestIndex].Matrix;
					_actualNodeTemplate.Parent = this;
					_nodeRenderingService.PrepareNodeForRendering(_actualNodeTemplate, viewport);
					
					closestNode = _actualNodeTemplate;
					closestNode.DataItem = _dataItemInfos[closestIndex].DataItem;
				}
			}
		}
	}
}
#endif