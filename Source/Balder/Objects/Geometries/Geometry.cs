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
		internal static readonly BubbledEvent<Geometry, FaceInputHandler> FaceMouseMoveEvent =
			BubbledEvent<Geometry, FaceInputHandler>.Register(g => g.FaceMouseMove);

		internal static readonly BubbledEvent<Geometry, FaceInputHandler> FaceMouseEnterEvent =
			BubbledEvent<Geometry, FaceInputHandler>.Register(g => g.FaceMouseEnter);

		internal static readonly BubbledEvent<Geometry, FaceInputHandler> FaceMouseLeaveEvent =
			BubbledEvent<Geometry, FaceInputHandler>.Register(g => g.FaceMouseLeave);

		public IGeometryContext GeometryContext { get; set; }
		public IGeometryDetailLevel FullDetailLevel { get; set; }

		private bool _materialSet = false;
		private bool _boundingSphereGenerated = false;

		public event FaceInputHandler FaceMouseMove = (a) => { };
		public event FaceInputHandler FaceMouseEnter = (a) => { };
		public event FaceInputHandler FaceMouseLeave = (a) => { }; 


#if(DEFAULT_CONSTRUCTOR)
		public Geometry()
			: this(Runtime.Instance.Kernel.Get<IGeometryContext>())
		{
		}
#endif

		public Geometry(IGeometryContext geometryContext)
		{
			GeometryContext = geometryContext;
			InitializeProperties();
		}


		public void MakeUnique()
		{
			GeometryContext = Runtime.Instance.Kernel.Get<IGeometryContext>();
			InitializeProperties();
		}

		protected override NodeStatistics GetStatisticsObject()
		{
			return new GeometryStatistics();
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
			if( null == vertices )
			{
				return;
			}
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

		public override float? Intersects(Ray pickRay)
		{
			Face face;
			int faceIndex;
			float faceU;
			float faceV;

			var distance = Intersects(pickRay, out face, out faceIndex, out faceU, out faceV);
			return distance;
		}


		public float? Intersects(Ray pickRay, out Face face, out int faceIndex, out float faceU, out float faceV)
		{

			face = null;
			faceIndex = -1;
			faceU = 0f;
			faceV = 0f;

			if( null == RenderingWorld )
			{
				return null;
			}

			var inverseWorldMatrix = Matrix.Invert(RenderingWorld);
			var transformedPosition = Vector.Transform(pickRay.Position, inverseWorldMatrix);
			var transformedDirection = Vector.TransformNormal(pickRay.Direction, inverseWorldMatrix);
			pickRay = new Ray(transformedPosition, transformedDirection);

			var distance = pickRay.Intersects(BoundingSphere);
			if (null != distance)
			{
				var vertices = FullDetailLevel.GetVertices();
				var faces = FullDetailLevel.GetFaces();

				if( null == vertices || null == faces )
				{
					return null;
				}

				Face closestFace = null;
				var closestFaceIndex = -1;
				var closestDistance = float.MaxValue;
				var closestFaceU = 0f;
				var closestFaceV = 0f;

				for (var i = 0; i< faces.Length; i++)
				{
					var currentFace = faces[i];
					var vertex1 = vertices[currentFace.A].ToVector();
					var vertex2 = vertices[currentFace.B].ToVector();
					var vertex3 = vertices[currentFace.C].ToVector();

					var rayDistance = pickRay.IntersectsTriangle(vertex1, vertex2, vertex3, out faceU, out faceV);
					if (null == rayDistance )
					{
						continue;
					}

					if (rayDistance < closestDistance)
					{
						closestFace = currentFace;
						closestFaceIndex = i;
						closestDistance = rayDistance.Value;
						closestFaceU = faceU;
						closestFaceV = faceV;
					}
				}

				if( null != closestFace )
				{
					face = closestFace;
					faceIndex = closestFaceIndex;
					faceU = closestFaceU;
					faceV = closestFaceV;
					return closestDistance;
				}
			}
			return null;

		}
	}
}