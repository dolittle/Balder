using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using Balder.Core.Assets;
using Balder.Core.Execution;
using Balder.Core.Materials;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using Balder.Core.Silverlight.Extensions;
using Image = Balder.Core.Imaging.Image;
using Matrix = Balder.Core.Math.Matrix;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class CubeBox : Box, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };
		public const double BoxSize = 5f;
		public const double BoxSpace = 0.3f;
		public const double BoxAdd = BoxSize + BoxSpace;
		public const double BoxAligment = BoxAdd / 2;
		public const double BoxXAlignment = -(((BoxAdd * Cube.Width) / 2) - BoxAligment);
		public const double BoxYAlignment = (((BoxAdd * Cube.Height) / 2) - BoxAligment);
		public const double BoxZAlignment = (((BoxAdd * Cube.Depth) / 2) - BoxAligment);

		#region Static Content
		private static Material _black;
		private static readonly Dictionary<CubeSide, Material> Materials;

		static CubeBox()
		{
			Materials = new Dictionary<CubeSide, Material>();

			GenerateMaterials();
		}

		private static void GenerateMaterials()
		{
			_black = new Material { Ambient = Colors.Black, Diffuse = Colors.Black, Specular = Colors.White };
			Materials[CubeSide.Front] = LoadMaterial("White.png");
			Materials[CubeSide.Back] = LoadMaterial("Yellow.png");
			Materials[CubeSide.Left] = LoadMaterial("Orange.png");
			Materials[CubeSide.Right] = LoadMaterial("Red.png");
			Materials[CubeSide.Top] = LoadMaterial("Blue.png");
			Materials[CubeSide.Bottom] = LoadMaterial("Green.png");
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
				return images[0] as Image;
			}
			return null;
		}
		#endregion

		public CubeBox(int x, int y, int z)
		{
			X = x;
			Y = y;
			Z = z;
			var actualX = BoxXAlignment + (BoxAdd * x);
			var actualY = BoxXAlignment + (BoxAdd * y);
			var actualZ = BoxXAlignment + (BoxAdd * z);

			PivotPoint = new Coordinate(actualX, actualY, actualZ);
			Dimension = new Coordinate(BoxSize, BoxSize, BoxSize);
			SetMaterial(x, y, z);

			CreateNormals();
			UpdateNormals(Matrix.Identity);

			MouseMove += CubeBox_MouseMove;

			var toolTip = new ToolTip();
			var cubeToolTip = new CubeToolTip();
			cubeToolTip.DataContext = this;
			toolTip.Content = cubeToolTip;
			ToolTip = toolTip;
		}

		private void CubeBox_MouseMove(object sender, Core.Input.MouseEventArgs args)
		{
			if (null != Content.Display)
			{
				var material = Content.Display.GetMaterialAtPosition((int)args.Position.X, (int)args.Position.Y);
				if (null != material && null != material.DiffuseMap)
				{
					//MaterialName = material.DiffuseMap.AssetName.AbsolutePath;
				}
			}
			
			
		}

		public int X { get; private set; }
		public int Y { get; private set; }
		public int Z { get; private set; }

		public Vector FrontNormal { get; private set; }
		public Vector SideNormal { get; private set; }
		public Vector UpNormal { get; private set; }

		private string _materialName;
		public string MaterialName
		{
			get { return _materialName; }
			set
			{
				_materialName = value;
				PropertyChanged.Notify(() => MaterialName);
			}
		}

		private Vector _calculatedFrontNormal;
		public Vector CalculatedFrontNormal
		{
			get { return _calculatedFrontNormal; }
			private set
			{
				_calculatedFrontNormal = value;
				PropertyChanged.Notify(() => CalculatedFrontNormal);
			}
		}

		private Vector _calculatedSideNormal;
		public Vector CalculatedSideNormal
		{
			get { return _calculatedSideNormal; }
			private set
			{
				_calculatedSideNormal = value;
				PropertyChanged.Notify(() => CalculatedSideNormal);
			}
		}

		private Vector _calculatedUpNormal;
		public Vector CalculatedUpNormal
		{
			get { return _calculatedUpNormal; }
			private set
			{
				_calculatedUpNormal = value;
				PropertyChanged.Notify(() => CalculatedUpNormal);
			}
		}

		private void CreateNormals()
		{
			FrontNormal = Vector.Zero;
			SideNormal = Vector.Zero;
			UpNormal = Vector.Zero;

			if (X == 0)
			{
				SideNormal = Vector.Left;
			}
			else if (X == Cube.Width - 1)
			{
				SideNormal = Vector.Right;
			}

			if (Y == 0)
			{
				UpNormal = Vector.Up;
			}
			else if (Y == Cube.Height - 1)
			{
				UpNormal = Vector.Down;
			}

			if (Z == 0)
			{
				FrontNormal = Vector.Forward;
			}
			else if (Z == Cube.Depth - 1)
			{
				FrontNormal = Vector.Backward;
			}
		}

		public void UpdateNormals(Matrix world)
		{
			CalculatedFrontNormal = Vector.TransformNormal(FrontNormal, world);
			CalculatedFrontNormal.Normalize();
			PropertyChanged.Notify(() => CalculatedFrontNormal);

			CalculatedSideNormal = Vector.TransformNormal(SideNormal, world);
			CalculatedSideNormal.Normalize();
			PropertyChanged.Notify(() => CalculatedSideNormal);

			CalculatedUpNormal = Vector.TransformNormal(UpNormal, world);
			CalculatedUpNormal.Normalize();
			PropertyChanged.Notify(() => CalculatedUpNormal);
		}

		private void SetMaterial(int x, int y, int z)
		{
			var front = _black;
			var back = _black;
			var left = _black;
			var right = _black;
			var top = _black;
			var bottom = _black;

			if (x == 0)
			{
				left = Materials[CubeSide.Left];
			}
			else if (x == Cube.Width - 1)
			{
				right = Materials[CubeSide.Right];
			}

			if (y == 0)
			{
				top = Materials[CubeSide.Top];
			}
			else if (y == Cube.Height - 1)
			{
				bottom = Materials[CubeSide.Bottom];
			}

			if (z == 0)
			{
				back = Materials[CubeSide.Back];
			}
			else if (z == Cube.Depth - 1)
			{
				front = Materials[CubeSide.Front];
			}

			SetMaterialOnSide(BoxSide.Front, front);
			SetMaterialOnSide(BoxSide.Back, back);
			SetMaterialOnSide(BoxSide.Left, left);
			SetMaterialOnSide(BoxSide.Right, right);
			SetMaterialOnSide(BoxSide.Top, top);
			SetMaterialOnSide(BoxSide.Bottom, bottom);
		}

		private void Rotate()
		{
			var rotation = Matrix.CreateRotation(
				(float)Rotation.X,
				(float)Rotation.Y,
				(float)Rotation.Z);
			UpdateNormals(rotation);
		}

		public void Rotate(double x, double y, double z)
		{
			Rotation.Set(x, y, z);
			Rotate();
		}

		public void AddRotation(double x, double y, double z)
		{
			Rotation.X += x;
			Rotation.Y += y;
			Rotation.Z += z;
			Rotate();
		}


	}
}
