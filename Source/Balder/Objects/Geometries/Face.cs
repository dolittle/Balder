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
using Balder.Materials;
using Balder.Math;

namespace Balder.Objects.Geometries
{
	public class Face
	{
		//public Material Material;
		public Color ColorA;
		public Color ColorB;
		public Color ColorC;

		public int A;
		public int B;
		public int C;

		public int DiffuseA;
		public int DiffuseB;
		public int DiffuseC;

		public int SmoothingGroup;

		public Vector Normal;

		public int NormalA;
		public int NormalB;
		public int NormalC;

		public int Opacity;

		public int MaterialId;

		public Face(int a, int b, int c)
		{
			A = a;
			B = b;
			C = c;

			ColorA = Colors.White;
			ColorB = Colors.White;
			ColorC = Colors.White;
		}
	}
}