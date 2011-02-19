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

#if(XAML)
#else
#if(!IOS)
using Colors = System.Drawing.Color;
#endif
#endif
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif
using Balder.Execution;
using Balder.Math;

namespace Balder.Objects.Geometries
{
	public class Heightmap : ArbitraryHeightmap
	{


#if(DEFAULT_CONSTRUCTOR)
		public Heightmap()
			: this(Runtime.Instance.Kernel.Get<IGeometryContext>())
		{

		}
#endif


		public Heightmap(IGeometryContext geometryContext)
			: base(geometryContext)
		{
		}



		public static Property<Heightmap, Dimension> DimensionProperty = Property<Heightmap, Dimension>.Register(p => p.Dimension);
		public Dimension Dimension
		{
			get { return DimensionProperty.GetValue(this); }
			set
			{
				DimensionProperty.SetValue(this, value);

				var halfWidth = (double)(value.Width / 2);
				var halfHeight = (double)(value.Height / 2);

				Point1 = new Coordinate(-halfWidth, 0, halfHeight);
				Point2 = new Coordinate(-halfWidth, 0, -halfHeight);
				Point3 = new Coordinate(halfWidth, 0, halfHeight);
				Point4 = new Coordinate(halfWidth, 0, -halfHeight);
				InvalidatePrepare();
			}
		}
	}
}
