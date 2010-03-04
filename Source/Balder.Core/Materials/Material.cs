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

using System.ComponentModel;
#if(SILVERLIGHT)
using System.Windows;
#endif
using Balder.Core.Execution;
using Balder.Core.Imaging;
using Balder.Core.Silverlight.TypeConverters;

namespace Balder.Core.Materials
{
	/// <summary>
	/// Represents a material
	/// </summary>
#if(SILVERLIGHT)
	public class Material : FrameworkElement
#else
	public class Material
#endif

	{
		/// <summary>
		/// Creates an instance of <see cref="Material"/>
		/// </summary>
		public Material()
		{
			Shade = MaterialShade.None;
			Diffuse = Color.Random();
		}

		/// <summary>
		/// Gets or sets the ambient <see cref="Color"/> of the material
		/// </summary>
#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Ambient { get; set; }


		/// <summary>
		/// Gets or sets the diffuse <see cref="Color"/> of the material
		/// </summary>
#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Diffuse { get; set; }


		/// <summary>
		/// Gets or sets the specular <see cref="Color"/> of the material
		/// </summary>
#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Specular { get; set; }

		/// <summary>
		/// Gets or sets the shininess of the material
		/// </summary>
		public float Shine { get; set; }

		/// <summary>
		/// Gets or sets the shininess strength of the material
		/// </summary>
		public float ShineStrength { get; set; }


		/// <summary>
		/// Gets or sets the shade model for the material
		/// </summary>
		public MaterialShade Shade { get; set; }


		/// <summary>
		/// Gets or sets wether or not the material is double sided or not
		/// </summary>
		public bool DoubleSided { get; set; }

		/// <summary>
		/// DiffuseMap Property
		/// </summary>
		public static readonly Property<Material, Image> DiffuseMapProperty =
			Property<Material, Image>.Register(m => m.DiffuseMap);

		/// <summary>
		/// Gets or sets the diffuse map <see cref="Image"/>
		/// </summary>
		public Image DiffuseMap
		{
			get { return DiffuseMapProperty.GetValue(this); }
			set { DiffuseMapProperty.SetValue(this, value); }
		}

		/// <summary>
		/// ReflectionMap Property
		/// </summary>
		public static readonly Property<Material, Image> ReflectionMapProperty =
			Property<Material, Image>.Register(m => m.ReflectionMap);

		/// <summary>
		/// Gets or sets the reflection map <see cref="Image"/>
		/// </summary>
		public Image ReflectionMap
		{
			get { return ReflectionMapProperty.GetValue(this); }
			set { ReflectionMapProperty.SetValue(this, value); }
		}
	}
}
