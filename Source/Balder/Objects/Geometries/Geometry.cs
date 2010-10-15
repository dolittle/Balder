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
		/*
		internal static readonly BubbledEvent<Geometry, FaceInputHandler> FaceMouseMoveEvent =
			BubbledEvent<Geometry, FaceInputHandler>.Register(g => g.FaceMouseMove);

		internal static readonly BubbledEvent<Geometry, FaceInputHandler> FaceMouseEnterEvent =
			BubbledEvent<Geometry, FaceInputHandler>.Register(g => g.FaceMouseEnter);

		internal static readonly BubbledEvent<Geometry, FaceInputHandler> FaceMouseLeaveEvent =
			BubbledEvent<Geometry, FaceInputHandler>.Register(g => g.FaceMouseLeave);
		*/
		public IGeometryContext GeometryContext { get; set; }
		public IGeometryDetailLevel FullDetailLevel { get; set; }

		private bool _materialSet = false;
		private bool _boundingSphereGenerated = false;
		private BoundingSphere _boundingSphere;

		/*
		public event FaceInputHandler FaceMouseMove = (a) => { };
		public event FaceInputHandler FaceMouseEnter = (a) => { };
		public event FaceInputHandler FaceMouseLeave = (a) => { }; 
		*/

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

		public override void PrepareBoundingSphere()
		{
			if( BoundingSphere.IsSet() )
			{
				return;
			}

			var vertices = FullDetailLevel.GetVertices();
			if( null == vertices )
			{
				return;
			}
            var minX = (from v in vertices
                          select v.X).Min();
            var minY = (from v in vertices
                          select v.Y).Min();
            var minZ = (from v in vertices
                          select v.Z).Min();
            var maxX = (from v in vertices
                          select v.X).Max();
            var maxY = (from v in vertices
                          select v.Y).Max();
            var maxZ = (from v in vertices
                          select v.Z).Max();
            var lowestVector = new Vector(minX, minY, minZ);
            var highestVector = new Vector(maxX,maxY,maxZ);
		    var center = Centroid(vertices);
            var length = highestVector - lowestVector;
			BoundingSphere = new BoundingSphere(center, length.Length / 2);
			base.PrepareBoundingSphere();
		}

        public Vector Centroid(Vertex[] vertices)
        {
            Vector centroid = Vector.Zero;

            centroid.X = (from v in vertices
                          select v.X).Average();
            centroid.Y = (from v in vertices
                          select v.Y).Average();
            centroid.Z = (from v in vertices
                          select v.Z).Average();
            return centroid;
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

			if( null != Material && !_materialSet )
			{
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
				BoundingSphere = geometry.BoundingSphere;
				FullDetailLevel = geometry.FullDetailLevel;
			}
		}

		public override float? Intersects(Viewport viewport, Ray pickRay)
		{
			Face face;
			int faceIndex;
			float faceU;
			float faceV;

			var distance = Intersects(viewport, pickRay, out face, out faceIndex, out faceU, out faceV);
			return distance;
		}


		public float? Intersects(Viewport viewport, Ray pickRay, out Face face, out int faceIndex, out float faceU, out float faceV)
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