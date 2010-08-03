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

using Balder.Execution;

namespace Balder.Objects.Geometries
{
	public class GeneratedGeometry : Geometry
	{
		protected GeneratedGeometry(IGeometryContext geometryContext)
			: base(geometryContext)
		{
			
		}


		public static readonly Property<GeneratedGeometry, bool> FlipNormalsProperty =
			Property<GeneratedGeometry, bool>.Register(g => g.FlipNormals);
		public bool FlipNormals
		{
			get { return FlipNormalsProperty.GetValue(this); }
			set { FlipNormalsProperty.SetValue(this, value); }
		}



		protected Face CreateFace(int a, int b, int c)
		{
			var flipNormals = FlipNormals;
			var face = new Face(flipNormals ? c : a, b, flipNormals ? a : c);
			return face;
		}
	}
}