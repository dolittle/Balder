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

using System;
#if(SILVERLIGHT)
using System.ComponentModel;
using System.Windows;
using Balder.Silverlight.TypeConverters;
#endif
#if(DEFAULT_CONSTRUCTOR)
using Ninject;
#endif
using Balder.Execution;
using Balder.Imaging;


namespace Balder.Materials
{
	/// <summary>
	/// Represents a material
	/// </summary>
#if(SILVERLIGHT)
	public class Material : FrameworkElement, IHaveIdentity
#else
	public class Material : IHaveIdentity
#endif
	{
		public static Material Default;

		static Material()
		{
			Default = Runtime.Instance.ContentManager.Creator.CreateMaterial();
			Default.Ambient = Colors.Black;
			Default.Diffuse = Colors.Blue;
			Default.Specular = Colors.White;
		}

#if(DEFAULT_CONSTRUCTOR)
		/// <summary>
		/// Creates an instance of <see cref="Material"/>
		/// </summary>
		public Material()
			: this(
				Runtime.Instance.Kernel.Get<IIdentityManager>()
			)
		{
		}
#endif


		/// <summary>
		/// Creates an instance of <see cref="Material"/>
		/// </summary>
		public Material(IIdentityManager identityManager)
		{
			Shade = MaterialShade.None;
			Diffuse = Color.Random();
			Id = identityManager.AllocateIdentity<Material>();
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
		public static readonly Property<Material, IMap> DiffuseMapProperty =
			Property<Material, IMap>.Register(m => m.DiffuseMap);

		/// <summary>
		/// Gets or sets the diffuse map <see cref="Image"/>
		/// </summary>
#if(SILVERLIGHT)
		[TypeConverter(typeof(UriToImageMapTypeConverter))]
#endif
		public IMap DiffuseMap
		{
			get { return DiffuseMapProperty.GetValue(this); }
			set { DiffuseMapProperty.SetValue(this, value); }
		}

		/// <summary>
		/// ReflectionMap Property
		/// </summary>
		public static readonly Property<Material, IMap> ReflectionMapProperty =
			Property<Material, IMap>.Register(m => m.ReflectionMap);

		/// <summary>
		/// Gets or sets the reflection map <see cref="Image"/>
		/// </summary>
#if(SILVERLIGHT)
		[TypeConverter(typeof(UriToImageMapTypeConverter))]
#endif
		public IMap ReflectionMap
		{
			get { return ReflectionMapProperty.GetValue(this); }
			set { ReflectionMapProperty.SetValue(this, value); }
		}

		/// <summary>
		/// Gets the Id of the Material
		/// </summary>
		public UInt16 Id { get; private set; }
	}
}
