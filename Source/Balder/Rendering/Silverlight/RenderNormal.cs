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
using Balder.Objects.Geometries;

namespace Balder.Rendering.Silverlight
{
	public class RenderNormal : Normal
	{
		public RenderNormal(Normal normal)
		{
			X = normal.X;
			Y = normal.Y;
			Z = normal.Z;
			Vector = new Vector(X,Y,Z);
			Transformed = new Vector(X,Y,Z);
			CalculatedColor = new Color(0, 0, 0, 0xff);
			IsColorCalculated = false;
		}

		public Vector Vector;
		public Vector Transformed;
		public Color CalculatedColor;
		public int CalculatedColorAsInt;
		public Color DiffuseColor;
		public int DiffuseColorAsInt;
		public Color SpecularColor;
		public int SpecularColorAsInt;
		public bool IsColorCalculated;
	}
}