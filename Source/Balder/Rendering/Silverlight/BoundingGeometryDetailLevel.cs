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
#if(SILVERLIGHT)
using Balder.Lighting;
using Balder.Math;
using Balder.Objects.Geometries;

namespace Balder.Rendering.Silverlight
{
	public class BoundingGeometryDetailLevel : GeometryDetailLevel
	{
		public BoundingGeometryDetailLevel(Vector minimum, Vector maximum, ILightCalculator lightCalculator, ITextureManager textureManager)
			: base(lightCalculator, textureManager)
		{
			AllocateVertices(8);
			SetVertex(0,new Vertex(minimum.X, maximum.Y, minimum.Z));
			SetVertex(1,new Vertex(maximum.X, maximum.Y, minimum.Z));
			SetVertex(2,new Vertex(minimum.X, minimum.Y, minimum.Z));
			SetVertex(3,new Vertex(maximum.X, minimum.Y, minimum.Z));
			SetVertex(4,new Vertex(minimum.X, maximum.Y, maximum.Z));
			SetVertex(5,new Vertex(maximum.X, maximum.Y, maximum.Z));
			SetVertex(6,new Vertex(minimum.X, minimum.Y, maximum.Z));
			SetVertex(7,new Vertex(maximum.X, minimum.Y, maximum.Z));

			AllocateLines(12);
			SetLine(0, new Line(0, 1));
			SetLine(1, new Line(2, 3));
			SetLine(2, new Line(4, 5));
			SetLine(3, new Line(6, 7));

			SetLine(4, new Line(0, 4));
			SetLine(5, new Line(1, 5));
			SetLine(6, new Line(2, 6));
			SetLine(7, new Line(3, 7));

			SetLine(8, new Line(0, 2));
			SetLine(9, new Line(1, 3));
			SetLine(10, new Line(4, 6));
			SetLine(11, new Line(5, 7));

			/*
			Faces = new Face[12];
			Faces[0] = new Face(2, 1, 0);
			Faces[1] = new Face(1, 2, 3);
				 
			
			Faces[2] = new Face(4, 5, 6);
			Faces[3] = new Face(7, 6, 5);
			
			Faces[4] = new Face(0, 4, 2);
			Faces[5] = new Face(6, 2, 4);
									   
			Faces[6] = new Face(3, 5, 1);
			Faces[7] = new Face(5, 3, 7);
				 
				 
			Faces[8] = new Face(0, 1, 4);
			Faces[9] = new Face(5, 4, 1);
									   
			Faces[10] = new Face(6, 3, 2);
			Faces[11] = new Face(3, 6, 7);
			 * */
		}

	}
}
#endif