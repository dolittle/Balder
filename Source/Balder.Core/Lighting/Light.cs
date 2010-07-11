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
using System.ComponentModel;
#endif
using Balder.Core.Display;
using Balder.Core.Execution;
using Balder.Core.Math;

namespace Balder.Core.Lighting
{
#pragma warning disable 1591 // Xml Comments
	public abstract partial class Light : EnvironmentalNode, ILight
	{
		public abstract ColorAsFloats Calculate(Viewport viewport, Vector point, Vector normal);

		public static readonly Property<Light, Color> DiffuseProp = Property<Light, Color>.Register(l => l.Diffuse);

#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Diffuse
		{
			get { return DiffuseProp.GetValue(this); }
			set { DiffuseProp.SetValue(this, value); }
		}

		public static readonly Property<Light, Color> SpecularProp = Property<Light, Color>.Register(l => l.Specular);

#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Specular
		{
			get { return SpecularProp.GetValue(this); }
			set { SpecularProp.SetValue(this, value); }
		}

		public static readonly Property<Light, Color> AmbientProp = Property<Light, Color>.Register(l => l.Ambient);

#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Ambient
		{
			get { return AmbientProp.GetValue(this); }
			set { AmbientProp.SetValue(this, value); }
		}

	}
#pragma warning restore 1591
}
