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

using Balder.Assets;
using Balder.Display;
using Balder.Execution;
using Balder.Materials;
using Balder.Math;
using Balder.Rendering;
using Ninject;

namespace Balder.Objects.Geometries
{
	public class Geometry : RenderableNode, IAssetPart, ICanBeUnique
	{
		public IGeometryContext GeometryContext { get; set; }
		public IGeometryDetailLevel FullDetailLevel { get; set; }

		private bool _materialSet = false;
		private bool _boundingSphereGenerated = false;

#if(DEFAULT_CONSTRUCTOR)
		public Geometry()
			: this(Runtime.Instance.Kernel.Get<IGeometryContext>(),
			Runtime.Instance.Kernel.Get<IIdentityManager>())
		{
		}
#endif

		public Geometry(IGeometryContext geometryContext, IIdentityManager identityManager)
			: base(identityManager)
		{
			GeometryContext = geometryContext;
			InitializeProperties();
		}


		public void MakeUnique()
		{
			GeometryContext = Runtime.Instance.Kernel.Get<IGeometryContext>();
			InitializeProperties();
		}

		private void InitializeProperties()
		{
			FullDetailLevel = GeometryContext.GetDetailLevel(DetailLevel.Full);
		}

		// TODO : Add boundingsphere automatically somewhere else..
		
		public void InitializeBoundingSphere()
		{
			var lowestVector = Vector.Zero;
			var highestVector = Vector.Zero;
			var vertices = FullDetailLevel.GetVertices();
			for (var vertexIndex = 0; vertexIndex < vertices.Length; vertexIndex++)
			{
				var vertex = vertices[vertexIndex];
				if (vertex.X < lowestVector.X)
				{
					lowestVector.X = vertex.X;
				}
				if (vertex.Y < lowestVector.Y)
				{
					lowestVector.Y = vertex.Y;
				}
				if (vertex.Z < lowestVector.Z)
				{
					lowestVector.Z = vertex.Z;
				}
				if (vertex.X > highestVector.X)
				{
					highestVector.X = vertex.X;
				}
				if (vertex.Y > highestVector.Y)
				{
					highestVector.Y = vertex.Y;
				}
				if (vertex.Z > highestVector.Z)
				{
					highestVector.Z = vertex.Z;
				}
			}

			var length = highestVector - lowestVector;
			var center = lowestVector + (length / 2);

			BoundingSphere = new BoundingSphere(center, length.Length / 2);
		}
		


		public override void Prepare(Viewport viewport)
		{
			if( !GeometryContext.HasDetailLevel(DetailLevel.BoundingBox) )
			{
				GeometryContext.GenerateDetailLevel(DetailLevel.BoundingBox,DetailLevel.Full);
			}
			base.Prepare(viewport);
		}

		public override void Render(Viewport viewport, DetailLevel detailLevel)
		{
			// Todo : this really doesn't work in all scenarios - generated meshes and so forth will only hit this once.
			// Read above comment - BoundingSphere generation should be put somewhere else
			if( !_boundingSphereGenerated )
			{
				InitializeBoundingSphere();
				_boundingSphereGenerated = true;
			}

			if( null != Material && !_materialSet )
			{
				GeometryContext.SetMaterialForAllFaces(Material);

				// Todo : Due to the fact that setting a color on a RenderableNode will cause the
				// OnColorChanged() to be called and it sets the material, this will affect all children
				// Not sure we want that behavior
				foreach( var child in Children )
				{
					if( child is Geometry )
					{
						((Geometry) child).Material = Material;
					}
				}

				_materialSet = true;
			}
			GeometryContext.Render(viewport, this, detailLevel);
		}


		protected override void OnColorChanged()
		{
			if (null == Material)
			{
				Material = Material.FromColor(Color);
			}
			else
			{
				Material.Diffuse = Color;
			}
			base.OnColorChanged();
		}


		public static readonly Property<Geometry, Material> MaterialProperty = Property<Geometry, Material>.Register(g => g.Material);
		public Material Material
		{
			get { return MaterialProperty.GetValue(this); }
			set
			{
				MaterialProperty.SetValue(this, value);
				_materialSet = false;
			}
		}


#if(!SILVERLIGHT)
		public string Name { get; set; }
#endif
		public object GetContext()
		{
			return GeometryContext;
		}

		public void SetContext(object context)
		{
			GeometryContext = context as IGeometryContext;
		}

		public void InitializeFromAssetPart(IAssetPart assetPart)
		{
			if( assetPart is Geometry )
			{
				var geometry = assetPart as Geometry;
				World = geometry.World;
			}
		}
	}
}