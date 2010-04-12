using System.Collections.Generic;
using System.Windows.Media;
using Balder.Core.Assets;
using Balder.Core.Execution;
using Balder.Core.Imaging;
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public enum CubeColor
	{
		White = 0,
		Blue,
		Red,
		Green,
		Yellow,
		Orange
	}

	public enum CubeSide
	{
		Front = 0,
		Back,
		Left,
		Right,
		Top,
		Bottom
	}

	public partial class Cube
	{
		public Cube()
		{
			InitializeComponent();
		}

		private const int Depth = 3;
		private const int Width = 3;
		private const int Height = 3;
		private const float BoxSize = 5f;
		private const float BoxSpace = 0.3f;
		private const float BoxAdd = BoxSize + BoxSpace;

		private Material _black;
		private Dictionary<CubeSide, Material> _materials;


		protected override void Prepare()
		{
			_materials = new Dictionary<CubeSide, Material>();
			GenerateMaterials();
			GenerateCubeBoxes();
			base.Prepare();
		}


		private void GenerateMaterials()
		{
			_black = new Material { Ambient = Colors.Black, Diffuse = Colors.Black, Specular = Colors.White };
			_materials[CubeSide.Front] = LoadMaterial("White.png");
			_materials[CubeSide.Back] = LoadMaterial("Yellow.png");
			_materials[CubeSide.Left] = LoadMaterial("Orange.png");
			_materials[CubeSide.Right] = LoadMaterial("Red.png");
			_materials[CubeSide.Top] = LoadMaterial("Blue.png");
			_materials[CubeSide.Bottom] = LoadMaterial("Green.png");
		}

		private void GenerateCubeBoxes()
		{
			var currentDepth = (-((BoxAdd * Depth) / 2)) + (BoxAdd / 2);

			for (var depthIndex = 0; depthIndex < Depth; depthIndex++)
			{
				var currentHeight = (-((BoxAdd * Height) / 2)) + (BoxAdd / 2);
				for (var heightIndex = 0; heightIndex < Height; heightIndex++)
				{
					var currentWidth = ((BoxAdd * Width) / 2) - (BoxAdd / 2);
					for (var widthIndex = 0; widthIndex < Width; widthIndex++)
					{
						var box = CreateBox();
						box.PivotPoint = new Coordinate(currentWidth, currentHeight, currentDepth);
						SetMaterial(box, widthIndex, heightIndex, depthIndex);


						if (heightIndex == 0)
						{
							box.Rotation.Y = 25;
						}

						Children.Add(box);
						currentWidth -= BoxAdd;
					}
					currentHeight += BoxAdd;
				}
				currentDepth += BoxAdd;
			}
		}

		private void SetMaterial(Box box, int x, int y, int z)
		{
			var front = _black;
			var back = _black;
			var left = _black;
			var right = _black;
			var top = _black;
			var bottom = _black;

			if (x == 0)
			{
				right = _materials[CubeSide.Right];
			} else if( x == Width -1)
			{
				left = _materials[CubeSide.Left];
			}
			
			if (y == 0)
			{
				top = _materials[CubeSide.Top];
			} else if( y == Height -1 )
			{
				bottom = _materials[CubeSide.Bottom];
			}
			
			if( z == 0 )
			{
				back = _materials[CubeSide.Back];
			} else if ( z == Depth -1 )
			{
				front = _materials[CubeSide.Front];				
			}

			box.SetMaterialOnSide(BoxSide.Front, front);
			box.SetMaterialOnSide(BoxSide.Back, back);
			box.SetMaterialOnSide(BoxSide.Left, left);
			box.SetMaterialOnSide(BoxSide.Right, right);
			box.SetMaterialOnSide(BoxSide.Top, top);
			box.SetMaterialOnSide(BoxSide.Bottom, bottom);

		}


		private Box CreateBox()
		{
			var box = new Box();
			box.Dimension = new Coordinate(BoxSize, BoxSize, BoxSize);
			return box;
		}

		private Material LoadMaterial(string file)
		{
			var material = new Material();
			material.DiffuseMap = LoadTexture(file);
			return material;
		}

		private Image LoadTexture(string file)
		{
			var uri = string.Format("/Balder.Silverlight.SampleBrowser;component/Samples/Creative/RubicsCube/Assets/{0}", file);

			// Todo: this is very hacky - refactoring of the asset system will make this not needed!
			var assetLoaderService = KernelContainer.Kernel.Get<IAssetLoaderService>();
			var loader = assetLoaderService.GetLoader<Image>(uri);
			var images = loader.Load(uri);
			if (images.Length == 1)
			{
				return images[0];
			}
			return null;
		}
	}
}
