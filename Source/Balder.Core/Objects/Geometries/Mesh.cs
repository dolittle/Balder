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

using System.Linq;
using Balder.Core.Assets;
using Balder.Core.Content;
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Materials;
using Balder.Core.Math;

namespace Balder.Core.Objects.Geometries
{
	public partial class Mesh : RenderableNode, IAsset, ICanHandleCloning
	{
		private readonly IAssetLoaderService _assetLoaderService;
		private readonly IContentManager _contentManager;
		private bool _materialSet = false;

		public Mesh()
			: this(Runtime.Instance.Kernel.Get<IAssetLoaderService>(),
					Runtime.Instance.Kernel.Get<IContentManager>())
		{

		}

		public Mesh(IAssetLoaderService assetLoaderService, IContentManager contentManager)
		{
			_assetLoaderService = assetLoaderService;
			_contentManager = contentManager;
		}


		public void Load(string assetName)
		{

			var loader = _assetLoaderService.GetLoader<Geometry>(assetName);
			var geometries = loader.Load(assetName);

			var boundingSphere = new BoundingSphere(Vector.Zero, 0);
			foreach (var geometry in geometries)
			{
				//geometry.InitializeBoundingSphere();
				//boundingSphere = BoundingSphere.CreateMerged(boundingSphere, geometry.BoundingSphere);
				Children.Add(geometry);
			}
			BoundingSphere = boundingSphere;

			// Todo: This has to be done since Loading of the node is done after Xaml has been bound - but we will get color from the File loaded
			SetColorForChildren();
		}

		public IAssetPart[] GetAssetParts()
		{
			var query = from c in Children
						where c is IAssetPart
			            select c as IAssetPart;
			return query.ToArray();
		}

		public void SetAssetParts(IAssetPart[] assetParts)
		{
			Children.Clear();
			var nodes = (from a in assetParts
			            where a is INode
			            select a as INode).ToArray();
			Children.AddRange(nodes);
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