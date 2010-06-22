using System;
using System.Collections.Generic;
using System.Windows.Media;
using Balder.Core.Assets;
using Balder.Core.Execution;
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using Color = Balder.Core.Color;
using Image = Balder.Core.Imaging.Image;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class CubeBox : ChamferBox
	{
		public const double BoxSize = 5f;
		public const double BoxSpace = 0.05f;
		public const double BoxAdd = BoxSize + BoxSpace;
		public const double BoxAligment = BoxAdd / 2;
		public const double BoxXAlignment = -(((BoxAdd * Cube.Width) / 2) - BoxAligment);
		public const double BoxYAlignment = (((BoxAdd * Cube.Height) / 2) - BoxAligment);
		public const double BoxZAlignment = -(((BoxAdd * Cube.Depth) / 2) - BoxAligment);

		public const double BoxXAdd = BoxAdd;
		public const double BoxYAdd = -BoxAdd;
		public const double BoxZAdd = BoxAdd;

		#region Static Content
		private static Material _black;
		private static readonly Dictionary<CubeColor, Material> Materials;

		static CubeBox()
		{
			Materials = new Dictionary<CubeColor, Material>();

			GenerateMaterials();
		}

		private static void GenerateMaterials()
		{
			_black = new Material { Ambient = Colors.Black, Diffuse = Colors.Black, Specular = Colors.Black, Shade=MaterialShade.Gouraud };
			Materials[CubeColor.White] = GetMaterial(Colors.Gray);  //LoadMaterial("White.png");
			Materials[CubeColor.Yellow] = GetMaterial(Colors.Yellow); //LoadMaterial("Yellow.png");
			Materials[CubeColor.Orange] = GetMaterial(Colors.Orange); //LoadMaterial("Orange.png");
			Materials[CubeColor.Red] = GetMaterial(Colors.Red); //LoadMaterial("Red.png");
			Materials[CubeColor.Blue] = GetMaterial(Colors.Blue); //LoadMaterial("Blue.png");
			Materials[CubeColor.Green] = GetMaterial(Colors.Green); //LoadMaterial("Green.png");
		}

		private static Material GetMaterial(Color color)
		{
			var material = new Material { Ambient = Colors.Black, Diffuse = color, Specular = Colors.Gray, Shade = MaterialShade.Gouraud };
			return material;
		}

		private static Material LoadMaterial(string file)
		{
			var material = new Material { DiffuseMap = LoadTexture(file) };
			return material;
		}

		private static Image LoadTexture(string file)
		{
			var uri = string.Format("/Balder.Silverlight.SampleBrowser;component/Samples/Creative/RubicsCube/Assets/{0}", file);

			// Todo: this is very hacky - refactoring of the asset system will make this not needed!
			var assetLoaderService = Runtime.Instance.Kernel.Get<IAssetLoaderService>();
			var loader = assetLoaderService.GetLoader<Image>(uri);
			var images = loader.Load(uri);

			if (images.Length == 1)
			{
				var image = images[0] as Image;
				image.AssetName = new Uri(uri, UriKind.Relative);
				return image;
			}
			return null;
		}
		#endregion

		public CubeBox(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;

			Fillet = 0.5f;

			InitializePivot();
			Dimension = new Coordinate(BoxSize, BoxSize, BoxSize);

			SetMaterialOnSide(BoxSide.None, _black);
			SetMaterialsByGridPosition(x, y, z);
			Reset();
		}

		

		private void InitializePivot()
		{
			var actualX = BoxXAlignment + (BoxXAdd * X);
			var actualY = BoxYAlignment + (BoxYAdd * Y);
			var actualZ = BoxZAlignment + (BoxZAdd * Z);
			PivotPoint = new Coordinate(-actualX, -actualY, -actualZ);
		}

		public int X { get; private set; }
		public int Y { get; private set; }
		public int Z { get; private set; }


		public CubeColor GetColorForSide(BoxSide side)
		{
			var material = GetMaterialOnSide(side);
			foreach (var key in Materials.Keys)
			{
				var materialByColor = Materials[key];
				if (materialByColor.Equals(material))
				{
					return key;
				}
			}
			return CubeColor.White;
		}

		public void SetColorForSide(BoxSide side, CubeColor color)
		{
			var material = Materials[color];
			SetMaterialOnSide(side, material);
		}


		private void SetMaterialsByGridPosition(int x, int y, int z)
		{
			var front = _black;
			var back = _black;
			var left = _black;
			var right = _black;
			var top = _black;
			var bottom = _black;

			if (x == 0)
			{
				left = Materials[CubeColor.Orange];
			}
			else if (x == Cube.Width - 1)
			{
				right = Materials[CubeColor.Red];
			}

			if (y == 0)
			{
				top = Materials[CubeColor.Blue];
			}
			else if (y == Cube.Height - 1)
			{
				bottom = Materials[CubeColor.Green];
			}

			if (z == 0)
			{
				front = Materials[CubeColor.White];
			}
			else if (z == Cube.Depth - 1)
			{
				back = Materials[CubeColor.Yellow];
			}

			SetMaterialOnSide(BoxSide.Front, front);
			SetMaterialOnSide(BoxSide.Back, back);
			SetMaterialOnSide(BoxSide.Left, left);
			SetMaterialOnSide(BoxSide.Right, right);
			SetMaterialOnSide(BoxSide.Top, top);
			SetMaterialOnSide(BoxSide.Bottom, bottom);
		}

		public void Reset()
		{
			SetMaterialsByGridPosition(X, Y, Z);
		}

		public void ResetRotation()
		{
			Rotation.Set(0, 0, 0);
		}


		public void Rotate(Coordinate rotation)
		{
			Rotation += rotation;
		}
	}
}
