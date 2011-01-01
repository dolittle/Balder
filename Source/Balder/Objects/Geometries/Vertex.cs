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

using Balder.Math;

namespace Balder.Objects.Geometries
{
	public class Vertex
	{
		public static readonly Vertex Zero = new Vertex(0, 0, 0);

		public Vertex()
			: this(0, 0, 0)
		{

		}

		public Vertex(float x, float y, float z)
			: this(x, y, z, 0, 0, 0)
		{

		}

		public Vertex(float x, float y, float z, float normalX, float normalY, float normalZ)
		{
			X = x;
			Y = y;
			Z = z;
			NormalX = normalX;
			NormalY = normalY;
			NormalZ = normalZ;
		}


		public float X;
		public float Y;
		public float Z;

		public float NormalX;
		public float NormalY;
		public float NormalZ;

		public Color Color;

		public Vector ToVector()
		{
			return new Vector(X, Y, Z);
		}

		public Vector NormalToVector()
		{
			return new Vector(NormalX, NormalY, NormalZ);
		}
	}
}