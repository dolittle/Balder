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

using System;
using System.Windows;
using System.Windows.Controls;
using Balder.Core;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;

namespace Balder.Silverlight.Controls
{
	public class NodesControl : ItemsControl, INode, ICanRender
	{
		public static readonly Property<NodesControl, DataTemplate> NodeTemplateProperty =
			Property<NodesControl, DataTemplate>.Register(n => n.NodeTemplate);
		public DataTemplate NodeTemplate
		{
			get { return ItemTemplate; }
			set { ItemTemplate = value; }
		}

		public Matrix ActualWorld { get; private set; }
		public Matrix RenderingWorld { get; set; }


		public void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			
		}


		public void Render(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			
		}

		public void RenderDebugInfo(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			
		}
	}
}
