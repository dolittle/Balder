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
using Balder.Objects.Geometries;
using Balder.Rendering;
using Ninject;

namespace Balder.Controls
{
	/// <summary>
	/// Represents a control that can take one node and instantiate it during rendering based upon data given to it
	/// </summary>
	public class InstancingNodes : RenderableNode, ICanGetNodeAtPosition
	{
		readonly INodeRenderingService _nodeRenderingService;
		DataItemInfo[] _dataItemInfos;
        RenderableNode _actualNodeTemplate;
        bool _templatePrepared;

        public event PrepareDataItemInfo PrepareDataItemInfo;

		/// <summary>
		/// Initializes an instance of <see cref="InstancingNodes"/>
		/// </summary>
		public InstancingNodes()
			: this(Runtime.Instance.Kernel.Get<INodeRenderingService>())
		{

		}

		/// <summary>
		/// Initializes an instance of <see cref="InstancingNodes"/>
		/// </summary>
		public InstancingNodes(INodeRenderingService nodeRenderingService)
		{
			_nodeRenderingService = nodeRenderingService;
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
                PrepareMergedBoundingSphere();
			}
		}

        public DataItemInfo[] DataItemInfos { get { return _dataItemInfos; } }

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
            if (!HasData || !HasDataItemInfos ) return;

			for (var index = 0; index < _dataItemInfos.Length; index++)
			{
                var node = _dataItemInfos[index].Node ?? _actualNodeTemplate;
                if (node != null)
                {
                    node.ActualWorld = _dataItemInfos[index].Matrix;
                    if( node is IHaveMaterial )
                        ((IHaveMaterial)node).Material = _dataItemInfos[index].Material;
                    
                    _nodeRenderingService.PrepareNodeForRendering(node, viewport);
                    _nodeRenderingService.RenderNode(node, viewport, detailLevel);
                }
			}

			base.Render(viewport, detailLevel);
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
            var distance = ActualBoundingObject.Intersects(pickRay);
			if (null != distance)
			{
				var closestIndex = 0;
				for (var index = 0; index < _dataItemInfos.Length; index++)
				{
                    var node = _dataItemInfos[index].Node ?? _actualNodeTemplate;
                    if (node != null)
                    {
                        node.ActualWorld = _dataItemInfos[index].Matrix;
                        if (node.IsIntersectionTestEnabled)
                        {
                            _nodeRenderingService.PrepareNodeForRendering(node, viewport);
                            distance = node.Intersects(viewport, pickRay);
                            if (null != distance && (distance < closestDistance || closestDistance == null))
                            {
                                closestDistance = distance;
                                closestIndex = index;
                            }
                        }
                    }
				}

				if (null != closestDistance)
				{
                    var node = _dataItemInfos[closestIndex].Node ?? _actualNodeTemplate;
                    if (node != null)
                    {
                        node.ActualWorld = _dataItemInfos[closestIndex].Matrix;
                        node.Parent = this;
                        _nodeRenderingService.PrepareNodeForRendering(node, viewport);
                        node.DataContext = _dataItemInfos[closestIndex].DataItem;

                        closestNode = node;
                    }
				}
			}
		}

        int GetDataLength(IEnumerable enumerable)
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

        void PrepareDataItemInfos(IEnumerable enumerable)
        {
            var length = GetDataLength(enumerable);
            _dataItemInfos = new DataItemInfo[length];
            var index = 0;
            foreach (var item in enumerable)
            {
                var info = new DataItemInfo { DataItem = item };
                _dataItemInfos[index] = info;
                if (PrepareDataItemInfo != null)
                    PrepareDataItemInfo(item, info);

                _dataItemInfos[index].Prepare();

                index++;
            }
        }

        void LoadTemplate()
        {
            var templateContent = NodeTemplate.LoadContent();
            if (!(templateContent is RenderableNode))
            {
                throw new ArgumentException("NodeTemplate must be of a RenderableNode type");
            }
            _actualNodeTemplate = templateContent as RenderableNode;
            _templatePrepared = false;
            _actualNodeTemplate.PrepareBoundingObject();
        }

        void PrepareMergedBoundingSphere()
        {
            if (!HasDataItemInfos) return;

            for (var index = 0; index < _dataItemInfos.Length; index++)
            {
                var node = _dataItemInfos[index].Node ?? _actualNodeTemplate;
                if (node != null)
                {
                    node.PrepareBoundingObject();
                    var boundingSphere = node.BoundingObject.Transform(_dataItemInfos[index].Matrix);
                    BoundingObject.Include(boundingSphere);
                }
            }
            ActualBoundingObject = BoundingObject.Transform(ActualWorld);
        }

        bool HasDataItemInfos { get { return _dataItemInfos != null; } }
        bool HasNodeTemplate { get { return _actualNodeTemplate != null; } }
        bool HasData { get { return Data != null; } }
	}
}
#endif