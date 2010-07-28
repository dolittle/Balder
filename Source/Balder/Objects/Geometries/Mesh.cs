﻿#region License
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

using System.Collections.Generic;
using System.Linq;
using Balder.Assets;
using Balder.Content;
using Balder.Display;
using Balder.Execution;
using Balder.Materials;
using Balder.Math;
using Ninject;

namespace Balder.Objects.Geometries
{
	public partial class Mesh : RenderableNode, IAsset, ICanHandleCloning
	{
		private readonly IContentManager _contentManager;
		private bool _materialSet = false;

#if(SILVERLIGHT)
		public Mesh()
			: this(
				Runtime.Instance.Kernel.Get<IContentManager>(),
				Runtime.Instance.Kernel.Get<IIdentityManager>())
		{

		}
#endif

		public Mesh(IContentManager contentManager, IIdentityManager identityManager)
			: base(identityManager)
		{
			_contentManager = contentManager;
		}


		public IAssetPart[] GetAssetParts()
		{
			var query = from c in Children
						where c is IAssetPart
			            select c as IAssetPart;
			return query.ToArray();
		}

		public void SetAssetParts(IEnumerable<IAssetPart> assetParts)
		{
			Children.Clear();
			var nodes = (from a in assetParts
			            where a is INode
			            select a as INode).ToArray();
			Children.AddRange(nodes);
			// Todo: This has to be done since Loading of the node is done after Xaml has been bound - but we will get color from the File loaded
			SetColorForChildren();
		}

		public void PreClone()
		{

		}

		public void PostClone(object source)
		{

		}

		public bool CopyChildren
		{
			get { return true; }
		}

		public override void BeforeRendering(Viewport viewport, Matrix view, Matrix projection, Matrix world)
		{
			if (null != Material && !_materialSet)
			{
				foreach (var child in Children)
				{
					if (child is Geometry)
					{
						((Geometry)child).Material = Material;
					}
				}
				_materialSet = false;
			}
			base.BeforeRendering(viewport, view, projection, world);
		}


		public Property<Mesh, Material> MaterialProperty = Property<Mesh, Material>.Register(g => g.Material);
		public Material Material
		{
			get { return MaterialProperty.GetValue(this); }
			set
			{
				MaterialProperty.SetValue(this, value);
				_materialSet = false;
			}
		}

	}
}