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

using System.Collections.Generic;
using Balder.Display;
using Balder.Execution;
using Balder.Materials;
using Balder.Math;
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif

namespace Balder.Objects.Geometries
{
	public class Plane : GeneratedGeometry
	{
		private readonly Dictionary<BoxSide, List<int>> _facesBySide;

#if(DEFAULT_CONSTRUCTOR)
		public Plane()
			: this(Runtime.Instance.Kernel.Get<IGeometryContext>())
		{
			
		}
#endif


		public Plane(IGeometryContext geometryContext)
			: base(geometryContext)
		{
			_facesBySide = new Dictionary<BoxSide, List<int>>();
            RepeatTextureCoordinateU = 1f;
            RepeatTextureCoordinateV = 1f;
		}


        public static readonly Property<Plane, Dimension> DimensionProperty = Property<Plane, Dimension>.Register(p => p.Dimension);
		public Dimension Dimension
		{
			get { return DimensionProperty.GetValue(this); }
			set
			{
				DimensionProperty.SetValue(this, value);
				InvalidatePrepare();
			}
		}

        public static readonly Property<Plane, double> RepeatTextureCoordinateUProperty = Property<Plane, double>.Register(p => p.RepeatTextureCoordinateU);
        public double RepeatTextureCoordinateU
        {
            get { return RepeatTextureCoordinateUProperty.GetValue(this); }
            set
            {
                RepeatTextureCoordinateUProperty.SetValue(this, value);
                InvalidatePrepare();
            }
        }


        public static readonly Property<Plane, double> RepeatTextureCoordinateVProperty = Property<Plane, double>.Register(p => p.RepeatTextureCoordinateV);
        public double RepeatTextureCoordinateV
        {
            get { return RepeatTextureCoordinateVProperty.GetValue(this); }
            set
            {
                RepeatTextureCoordinateVProperty.SetValue(this, value);
                InvalidatePrepare();
            }
        }


		public override void Prepare(Viewport viewport)
		{
			GenerateVertices();
			GenerateTextureCoordinates();
			GenerateFaces();

			GeometryHelper.CalculateNormals(FullDetailLevel);

			base.Prepare(viewport);
		}


        void GenerateVertices()
        {
            var halfDimensionX = Dimension.Width / 2f;
            var halfDimensionY = Dimension.Height / 2f;

            var frontUpperLeft = new Vertex(-halfDimensionX, halfDimensionY, 0);
            var frontUpperRight = new Vertex(halfDimensionX, halfDimensionY, 0);
            var frontLowerLeft = new Vertex(-halfDimensionX, -halfDimensionY, 0);
            var frontLowerRight = new Vertex(halfDimensionX, -halfDimensionY, 0);

            FullDetailLevel.AllocateVertices(4);
            FullDetailLevel.SetVertex(0, frontUpperLeft);
            FullDetailLevel.SetVertex(1, frontUpperRight);
            FullDetailLevel.SetVertex(2, frontLowerLeft);
            FullDetailLevel.SetVertex(3, frontLowerRight);
        }

		void GenerateTextureCoordinates()
		{
            var uRepeat = (float)RepeatTextureCoordinateU;
            var vRepeat = (float)RepeatTextureCoordinateV;
			FullDetailLevel.AllocateTextureCoordinates(4);
            FullDetailLevel.SetTextureCoordinate(0, new TextureCoordinate(0f, 0f));
            FullDetailLevel.SetTextureCoordinate(1, new TextureCoordinate(uRepeat, 0f));
            FullDetailLevel.SetTextureCoordinate(2, new TextureCoordinate(0f, vRepeat));
            FullDetailLevel.SetTextureCoordinate(3, new TextureCoordinate(uRepeat, vRepeat));
		}

		protected void SetFace(int faceIndex, int a, int b, int c, Vector normal, int diffuseA, int diffuseB, int diffuseC, int smoothingGroup)
		{
            var flipNormals = FlipNormals;
			var face = CreateFace(a, b, c);
			face.Normal = normal;
			face.DiffuseA = flipNormals ? diffuseC : diffuseA;
			face.DiffuseB = diffuseB;
            face.DiffuseC = flipNormals ? diffuseA : diffuseC;

			face.SmoothingGroup = smoothingGroup;
			FullDetailLevel.SetFace(faceIndex, face);
		}


		void GenerateFaces()
		{
			FullDetailLevel.AllocateFaces(2);

			SetFace(0, 2, 1, 0, Vector.Backward, 2, 1, 0, 0);
			SetFace(1, 1, 2, 3, Vector.Backward, 1, 2, 3, 0);
		}
	}
}
