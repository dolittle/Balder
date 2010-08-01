using System.Windows.Controls;
using Balder;
using Balder.Assets;
using Balder.Execution;
using Balder.Materials;
using Balder.Objects.Geometries;
using Ninject;

namespace Balder.Silverlight.SampleBrowser.Samples.Materials.MaterialPicker
{
	public partial class Content
	{
		private RenderableNode _selectedNode;
		private Material _selectedMaterial;

		private Material _flatMaterial;
		private Material _gouraudMaterial;
		private Material _texturedMaterial;
		private Material _reflectionMaterial;

		private ImageMap _selectedTexture;
		private ImageMap _reflectionMapTexture;
		private ImageMap _visualStudioTexture;
		private ImageMap _balderLogoTexture;


		public Content()
		{
			InitializeComponent();
			Loaded += Content_Loaded;

			_flatMaterial = Resources["FlatMaterial"] as Material;
			_gouraudMaterial = Resources["GouraudMaterial"] as Material;
			_texturedMaterial = Resources["TexturedMaterial"] as Material;
			_reflectionMaterial = Resources["ReflectionMaterial"] as Material;
		}

		void Content_Loaded(object sender, System.Windows.RoutedEventArgs e)
		{
			
			_reflectionMapTexture = LoadTexture("/Balder.Silverlight.SampleBrowser;component/Assets/ReflectionMap.jpg");
			_visualStudioTexture = LoadTexture("/Balder.Silverlight.SampleBrowser;component/Assets/VisualStudio.png");
			_balderLogoTexture = LoadTexture("/Balder.Silverlight.SampleBrowser;component/Assets/BalderLogo.png");

			HandleSelection();
		}

		private ImageMap LoadTexture(string uri)
		{
			// Todo: this is very hacky - load the material using Xaml instead and switch
			// between them in Xaml
			var image = Runtime.Instance.ContentManager.Load<Imaging.Image>(uri);
			if( null != image )
			{
				return new ImageMap(image);
			}
			return null;
		}

		


		private void ObjecTypeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if (null != ObjectComboBox)
			{
				HandleSelection();
			}

		}


		private void MaterialTypeChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
		{
			if( null != MaterialComboBox )
			{
				HandleMaterialSelection();
			}
		}

		private void TextureChanged(object sender, SelectionChangedEventArgs e)
		{
			if( null != TextureComboBox )
			{
				HandleTextureSelection();
			}
		}


		private void HandleSelection()
		{
			HandleObjectSelection();
			HandleMaterialSelection();
			HandleTextureSelection();			
		}

		private void HandleObjectSelection()
		{
			switch (ObjectComboBox.SelectedIndex)
			{
				case 0:
					{
						_selectedNode = Teapot;

						Teapot.IsVisible = true;
						Box.IsVisible = false;
					}
					break;
				case 1:
					{
						_selectedNode = Box;

						Teapot.IsVisible = false;
						Box.IsVisible = true;
					}
					break;
			}
		}

		private void HandleMaterialSelection()
		{
			
			switch( MaterialComboBox.SelectedIndex )
			{
				case 0:
					{
						_selectedMaterial = _flatMaterial;
					}
					break;
				case 1:
					{
						_selectedMaterial = _gouraudMaterial;
					}
					break;
				case 2:
					{
						_selectedMaterial = _texturedMaterial;
					}
					break;
				case 3:
					{
						_selectedMaterial = _reflectionMaterial;
					}
					break;
			}

			if (_selectedNode is Mesh)
			{
				((Mesh)_selectedNode).Material = _selectedMaterial;
			} else if( _selectedNode is Geometry )
			{
				((Geometry)_selectedNode).Material = _selectedMaterial;
			}

			HandleTextureSelection();
		}

		private void HandleTextureSelection()
		{
			switch( TextureComboBox.SelectedIndex )
			{
				case 0:
					{
						_selectedTexture = _reflectionMapTexture;
					}
					break;
				case 1:
					{
						_selectedTexture = _visualStudioTexture;
					}
					break;
				case 2:
					{
						_selectedTexture = _balderLogoTexture;
					}
					break;
			}

			if( null != _selectedMaterial.DiffuseMap )
			{
				_selectedMaterial.DiffuseMap = _selectedTexture;
			} else if( null != _selectedMaterial.ReflectionMap )
			{
				_selectedMaterial.ReflectionMap = _selectedTexture;
			}
		}

	}
}
