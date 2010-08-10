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
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Balder.Rendering.Silverlight;
using Balder.Rendering.Silverlight.Drawing;
using Balder.Silverlight.TypeConverters;
#endif
using Balder.Execution;


namespace Balder.Materials
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
		public static Material Default;

		static Material()
		{
			Default = new Material();
			Default.Ambient = Colors.Black;
			Default.Diffuse = Colors.Blue;
			Default.Specular = Colors.White;
			Default.Shade = MaterialShade.Gouraud;

			Default.LinkAmbientAndDiffuse = true;
		}

		public static Material FromColor(Color color)
		{
			var material = new Material();
			material.Diffuse = color;
			material.Specular = Colors.White;
			material.Shade = MaterialShade.Gouraud;
			return material;
		}

		/// <summary>
		/// Creates an instance of <see cref="Material"/>
		/// </summary>
		public Material()
		{
			Shade = MaterialShade.None;
			Diffuse = Color.Random();
			DiffuseMapOpacity = 1f;
			ReflectionMapOpacity = 1f;
			MagnificationFiltering = MaterialFiltering.None;
		}


		public bool LinkAmbientAndDiffuse { get; set; }

		public static readonly Property<Material, Color> AmbientProperty =
			Property<Material, Color>.Register(m => m.Ambient);

		/// <summary>
		/// Gets or sets the ambient <see cref="Color"/> of the material
		/// </summary>
#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Ambient
		{
			get { return AmbientProperty.GetValue(this); }
			set
			{
				if (LinkAmbientAndDiffuse)
				{
					Diffuse = value;
				}
				AmbientProperty.SetValue(this, value);
			}
		}


		public static readonly Property<Material, Color> DiffuseProperty =
			Property<Material, Color>.Register(m => m.Diffuse);

		/// <summary>
		/// Gets or sets the diffuse <see cref="Color"/> of the material
		/// </summary>
#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Diffuse
		{
			get { return DiffuseProperty.GetValue(this); }
			set
			{
				if (LinkAmbientAndDiffuse)
				{
					Ambient = value;
				}
				DiffuseProperty.SetValue(this, value);
			}
		}


		public static readonly Property<Material, Color> SpecularProperty =
			Property<Material, Color>.Register(m => m.Specular);


		/// <summary>
		/// Gets or sets the specular <see cref="Color"/> of the material
		/// </summary>
#if(SILVERLIGHT)
		[TypeConverter(typeof(ColorConverter))]
#endif
		public Color Specular
		{
			get { return SpecularProperty.GetValue(this); }
			set { SpecularProperty.SetValue(this, value); }
		}



		public static readonly Property<Material, float> ShineProperty =
			Property<Material, float>.Register(m => m.Shine);

		/// <summary>
		/// Gets or sets the shininess of the material
		/// </summary>
		public float Shine
		{
			get { return ShineProperty.GetValue(this); }
			set { ShineProperty.SetValue(this, value); }
		}


		public static readonly Property<Material, float> ShineStrengthProperty =
			Property<Material, float>.Register(m => m.ShineStrength);

		/// <summary>
		/// Gets or sets the shininess strength of the material
		/// </summary>
		public float ShineStrength
		{
			get { return ShineStrengthProperty.GetValue(this); }
			set { ShineStrengthProperty.SetValue(this, value); }
		}

		public static readonly Property<Material, MaterialShade> ShadeProperty =
			Property<Material, MaterialShade>.Register(m => m.Shade);

		/// <summary>
		/// Gets or sets the shade model for the material
		/// </summary>
		public MaterialShade Shade
		{
			get { return ShadeProperty.GetValue(this); }
			set
			{
				ShadeProperty.SetValue(this, value);
#if(SILVERLIGHT)
				MaterialPropertiesChanged();
#endif
			}
		}


		public static readonly Property<Material, MaterialFiltering> MagnificationFilteringProperty =
			Property<Material, MaterialFiltering>.Register(m => m.MagnificationFiltering);

		/// <summary>
		/// Gets or sets the filtering used when rendering the material
		/// </summary>
		public MaterialFiltering MagnificationFiltering
		{
			get { return MagnificationFilteringProperty.GetValue(this); }
			set { MagnificationFilteringProperty.SetValue(this, value); }
		}


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
			set
			{
				DiffuseMapProperty.SetValue(this, value);
#if(SILVERLIGHT)
				MaterialPropertiesChanged();
#endif

			}
		}

		public static readonly Property<Material, float> DiffuseMapOpacityProperty =
			Property<Material, float>.Register(m => m.DiffuseMapOpacity);
		public float DiffuseMapOpacity
		{
			get { return DiffuseMapOpacityProperty.GetValue(this); }
			set
			{
				DiffuseMapOpacityProperty.SetValue(this, value);
#if(SILVERLIGHT)
				MaterialPropertiesChanged();
#endif
			}
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
			set
			{
				ReflectionMapProperty.SetValue(this, value);
#if(SILVERLIGHT)
				MaterialPropertiesChanged();
#endif
			}
		}


		public static readonly Property<Material, float> ReflectionMapOpacityProperty =
			Property<Material, float>.Register(m => m.ReflectionMapOpacity);
		public float ReflectionMapOpacity
		{
			get { return ReflectionMapOpacityProperty.GetValue(this); }
			set
			{
				ReflectionMapOpacityProperty.SetValue(this, value);
#if(SILVERLIGHT)
				MaterialPropertiesChanged();
#endif
			}
		}


#if(SILVERLIGHT)
		private static readonly FlatTriangle FlatTriangleRenderer = new FlatTriangle();
		private static readonly FlatTriangleNoDepth FlatTriangleNoDepthRenderer = new FlatTriangleNoDepth();
		private static readonly GouraudTriangle GouraudTriangleRenderer = new GouraudTriangle();
		private static readonly GouraudTriangleNoDepth GouraudTriangleNoDepthRenderer = new GouraudTriangleNoDepth();
		private static readonly FlatTextureTriangle FlatTextureTriangleRenderer = new FlatTextureTriangle();
		private static readonly TextureTriangle TextureTriangleRenderer = new TextureTriangle();
		private static readonly TextureTriangleNoDepth TextureTriangleNoDepthRenderer = new TextureTriangleNoDepth();
		private static readonly GouraudTextureTriangle GouraudTextureTriangleRenderer = new GouraudTextureTriangle();
		private static readonly TextureTriangleBilinear TextureTriangleBilinearRenderer = new TextureTriangleBilinear();

		private void MaterialPropertiesChanged()
		{
			SetRenderer();

		}


		private void SetRenderer()
		{
			switch( Shade )
			{
				case MaterialShade.None:
					{
						if (null == DiffuseMap || DiffuseMapOpacity == 0)
						{
							if (null == ReflectionMap || ReflectionMapOpacity == 0)
							{
								Renderer = FlatTriangleRenderer;
							}
							else
							{
								Renderer = TextureTriangleRenderer;
							}
						}
						else
						{
							if (null != ReflectionMap && ReflectionMapOpacity != 0)
							{
								Renderer = TextureTriangleRenderer;
							}
							else
							{
								Renderer = TextureTriangleRenderer;
							}
						}
						
					}
					break;

				case MaterialShade.Flat:
					{
						if( null == DiffuseMap || DiffuseMapOpacity == 0)
						{
							if( null == ReflectionMap || ReflectionMapOpacity == 0 )
							{
								Renderer = FlatTriangleRenderer;
							} else
							{
								Renderer = FlatTextureTriangleRenderer;
							}
						} else
						{
							if( null != ReflectionMap && ReflectionMapOpacity != 0 )
							{
								Renderer = FlatTextureTriangleRenderer;
							} else
							{
								Renderer = FlatTextureTriangleRenderer;
							}
						}
					}
					break;

				case MaterialShade.Gouraud:
					{
						if (null == DiffuseMap || DiffuseMapOpacity == 0)
						{
							if (null == ReflectionMap || ReflectionMapOpacity == 0)
							{
								Renderer = GouraudTriangleRenderer;
							}
							else
							{
								Renderer = GouraudTextureTriangleRenderer;
							}
						}
						else
						{
							if (null != ReflectionMap && ReflectionMapOpacity != 0)
							{
								Renderer = GouraudTextureTriangleRenderer;
							}
							else
							{
								Renderer = GouraudTextureTriangleRenderer;
							}
						}
					}
					break;
			}
		}

		public IVertexModifier DefaultVertexModifier { get; private set; }
		public List<IVertexModifier> VertexModifiers { get; private set; }

		public Triangle Renderer { get; private set; }
#endif
	}
}
