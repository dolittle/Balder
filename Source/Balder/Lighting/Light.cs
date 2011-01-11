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

#if(SILVERLIGHT)
using System.ComponentModel;
using Balder.Silverlight.TypeConverters;
#endif

using Balder.Display;
using Balder.Execution;
using Balder.Math;
using Balder.Materials;

namespace Balder.Lighting
{
#pragma warning disable 1591 // Xml Comments
	public abstract class Light : EnvironmentalNode, ILight
	{
		protected int AmbientAsInt;
		protected int DiffuseAsInt;
		protected int SpecularAsInt;
		protected float StrengthAsFloat;

		public abstract int Calculate(Viewport viewport, Material material, Vector point, Vector normal, out int diffuseResult, out int specularResult);

		public static readonly Property<Light, Color> DiffuseProperty = Property<Light, Color>.Register(l => l.Diffuse);

		protected Light()
		{
			Strength = 1;
			Diffuse = Colors.White;
			Specular = Colors.White;
		}

#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Diffuse
		{
			get { return DiffuseProperty.GetValue(this); }
			set
			{
				DiffuseProperty.SetValue(this, value);
				DiffuseAsInt = value.ToInt();
			}
		}

		public static readonly Property<Light, Color> SpecularProperty = Property<Light, Color>.Register(l => l.Specular);

#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Specular
		{
			get { return SpecularProperty.GetValue(this); }
			set
			{
				SpecularProperty.SetValue(this, value);
				SpecularAsInt = value.ToInt();
			}
		}

		public static readonly Property<Light, Color> AmbientProperty = Property<Light, Color>.Register(l => l.Ambient);

#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Ambient
		{
			get { return AmbientProperty.GetValue(this); }
			set
			{
				AmbientProperty.SetValue(this, value);
				AmbientAsInt = value.ToInt();
			}
		}


		public static readonly Property<Light, double> StrengthProperty =
			Property<Light, double>.Register(l => l.Strength);
		/// <summary>
		/// Gets or sets the strength of the light
		/// </summary>
		public double Strength
		{
			get { return StrengthProperty.GetValue(this); }
			set
			{
				StrengthProperty.SetValue(this, value);
				StrengthAsFloat = (float)value;
			}
		}

		public static readonly Property<Light, bool> IsEnabledProperty =
			Property<Light, bool>.Register(l => l.IsEnabled, true);
		public bool IsEnabled
		{
			get { return IsEnabledProperty.GetValue(this); }
			set { IsEnabledProperty.SetValue(this, value); }
		}

	}
#pragma warning restore 1591 // Xml Comments
}
