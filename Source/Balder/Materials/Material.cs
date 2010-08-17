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
using Ninject;
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
		internal Color ActualAmbient;
		internal Color ActualDiffuse;
		internal int AmbientAsInt;
		internal int DiffuseAsInt;
		internal int SpecularAsInt;
		internal float ShineAsFloat;
		internal float ShineStrengthAsFloat;
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
			Shine = 1;
			ShineStrength = 1;
#if(SILVERLIGHT)
			Renderer = GouraudTriangleRenderer;
#endif
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
				UpdateColors();
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
				UpdateColors();
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



		public static readonly Property<Material, double> ShineProperty =
			Property<Material, double>.Register(m => m.Shine, 1);

		/// <summary>
		/// Gets or sets the shininess of the material
		/// </summary>
		public double Shine
		{
			get { return ShineProperty.GetValue(this); }
			set { ShineProperty.SetValue(this, value); }
		}


		public static readonly Property<Material, double> ShineStrengthProperty =
			Property<Material, double>.Register(m => m.ShineStrength, 1);

		/// <summary>
		/// Gets or sets the shininess strength of the material
		/// </summary>
		public double ShineStrength
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
			set
			{
				MagnificationFilteringProperty.SetValue(this, value);
#if(SILVERLIGHT)
				MaterialPropertiesChanged();
#endif
			}
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
				UpdateColors();
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
				UpdateColors();
			}
		}


		private void UpdateColors()
		{
			ActualAmbient = GetActualColor(Ambient);
			ActualDiffuse = GetActualColor(Diffuse);

			AmbientAsInt = ActualAmbient.ToInt();
			DiffuseAsInt = ActualDiffuse.ToInt();
			SpecularAsInt = Specular.ToInt();

			ShineAsFloat = (float) Shine;
			ShineStrengthAsFloat = (float) ShineStrength;
		}

		private Color GetActualColor(Color color)
		{
			Color actualColor;
			if (null != DiffuseMap)
			{
				if (null != ReflectionMap)
				{
					actualColor = color * (1f - DiffuseMapOpacity) * (1f - ReflectionMapOpacity);
				}
				else
				{
					actualColor = color * (1f - DiffuseMapOpacity);
				}
			}
			else
			{
				if (null != ReflectionMap)
				{
					actualColor = color * (1f - ReflectionMapOpacity);
				}
				else
				{
					actualColor = color;
				}
			}

			return actualColor;

		}

#if(SILVERLIGHT)
		private static readonly FlatTriangle FlatTriangleRenderer = new FlatTriangle();
		private static readonly FlatTriangleNoDepth FlatTriangleNoDepthRenderer = new FlatTriangleNoDepth();
		private static readonly GouraudTriangle GouraudTriangleRenderer = new GouraudTriangle();
		private static readonly GouraudTriangleNoDepth GouraudTriangleNoDepthRenderer = new GouraudTriangleNoDepth();
		private static readonly FlatTextureTriangle FlatTextureTriangleRenderer = new FlatTextureTriangle();
		private static readonly FlatTextureTriangleBilinear FlatTextureTriangleBilinearRenderer = new FlatTextureTriangleBilinear();
		private static readonly TextureTriangle TextureTriangleRenderer = new TextureTriangle();
		private static readonly TextureTriangleNoDepth TextureTriangleNoDepthRenderer = new TextureTriangleNoDepth();
		private static readonly GouraudTextureTriangle GouraudTextureTriangleRenderer = new GouraudTextureTriangle();
		private static readonly TextureTriangleBilinear TextureTriangleBilinearRenderer = new TextureTriangleBilinear();
		private static readonly GouraudTextureTriangleBilinear GouraudTextureTriangleBilinearRenderer = new GouraudTextureTriangleBilinear();

		internal Texture DiffuseTexture;
		internal Texture ReflectionTexture;


		private void MaterialPropertiesChanged()
		{
			UpdateColors();
			SetRenderer();
			SetTextures();
		}

		private static ITextureManager _textureManager;
		private void SetTextures()
		{
			if( null == _textureManager )
			{
				// TODO: Look into this - would be nice to have a better way of doing 
				// the performance boost of caching the textures directly - very platform specific thing
				// bleeding into core - possibly introduce a MaterialContext thing
				_textureManager = Runtime.Instance.Kernel.Get<ITextureManager>();
			}

			if (null != DiffuseMap)
			{
				DiffuseTexture = _textureManager.GetTextureForMap(DiffuseMap);
			}
			if( null != ReflectionMap )
			{
				ReflectionTexture = _textureManager.GetTextureForMap(ReflectionMap);
			}
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
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
											? TextureTriangleBilinearRenderer
											: (Triangle)TextureTriangleRenderer;
							}
						}
						else
						{
							if (null != ReflectionMap && ReflectionMapOpacity != 0)
							{
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
								           	? TextureTriangleBilinearRenderer
								           	: (Triangle)TextureTriangleRenderer;
							}
							else
							{
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
											? TextureTriangleBilinearRenderer
											: (Triangle)TextureTriangleRenderer;
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
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
											? FlatTextureTriangleBilinearRenderer
											: (Triangle)FlatTextureTriangleRenderer;
							}
						} else
						{
							if( null != ReflectionMap && ReflectionMapOpacity != 0 )
							{
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
											? FlatTextureTriangleBilinearRenderer
											: (Triangle)FlatTextureTriangleRenderer;
							}
							else
							{
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
											? FlatTextureTriangleBilinearRenderer
											: (Triangle)FlatTextureTriangleRenderer;
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
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
											? GouraudTextureTriangleBilinearRenderer
											: (Triangle)GouraudTextureTriangleRenderer;

							}
						}
						else
						{
							if (null != ReflectionMap && ReflectionMapOpacity != 0)
							{
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
											? GouraudTextureTriangleBilinearRenderer
											: (Triangle)GouraudTextureTriangleRenderer;
							}
							else
							{
								Renderer = MagnificationFiltering == MaterialFiltering.Bilinear
											? GouraudTextureTriangleBilinearRenderer
											: (Triangle)GouraudTextureTriangleRenderer;
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
