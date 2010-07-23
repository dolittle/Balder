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
using Balder.Debug;
using Balder.Display;
using Balder.Execution;
using Balder.Rendering;
#if(SILVERLIGHT)
using System.ComponentModel;
using Balder.Silverlight.TypeConverters;
#endif
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif

namespace Balder
{
	public abstract class RenderableNode : HierarchicalNode, ICanBeVisible, ICanRender, IHaveColor
	{
#if(DEFAULT_CONSTRUCTOR)
		protected RenderableNode()
			: this(Runtime.Instance.Kernel.Get<IIdentityManager>())
		{
		}
#endif

		protected RenderableNode(IIdentityManager identityManager)
			: base(identityManager)
		{
			IsVisible = true;
			InitializeColor();
		}

		private void InitializeColor()
		{
			Color = Color.Random();
		}

		public static readonly Property<RenderableNode, Color> ColorProp = Property<RenderableNode, Color>.Register(n => n.Color);
#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Color
		{
			get { return ColorProp.GetValue(this); }
			set
			{
				ColorProp.SetValue(this, value);
				SetColorForChildren();
			}
		}

		protected void SetColorForChildren()
		{
			foreach (var node in Children)
			{
				if (node is RenderableNode)
				{
					((RenderableNode)node).Color = Color;
				}
			}
		}

		public static readonly Property<RenderableNode, bool> IsVisibleProp = Property<RenderableNode, bool>.Register(n => n.IsVisible, true);
		public bool IsVisible
		{
			get { return IsVisibleProp.GetValue(this); }
			set { IsVisibleProp.SetValue(this, value); }
		}

		public virtual void Render(Viewport viewport, DetailLevel detailLevel) { }
		public virtual void RenderDebugInfo(Viewport viewport, DetailLevel detailLevel)
		{
			if (viewport.DebugInfo.BoundingSpheres)
			{
				DebugRenderer.Instance.RenderBoundingSphere(BoundingSphere, viewport, detailLevel, RenderingWorld);
			}
		}
	}
}
