using System.Collections.Generic;
using Balder.Materials;
using Balder.Math;
using Balder.Objects.Geometries;

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
			Materials[CubeColor.White] = GetMaterial(Colors.Gray);
			Materials[CubeColor.Yellow] = GetMaterial(Colors.Yellow);
			Materials[CubeColor.Orange] = GetMaterial(Colors.Orange);
			Materials[CubeColor.Red] = GetMaterial(Colors.Red);
			Materials[CubeColor.Blue] = GetMaterial(Colors.Blue);
			Materials[CubeColor.Green] = GetMaterial(Colors.Green);
		}

		private static Material GetMaterial(Color color)
		{
			var material = new Material { Ambient = Colors.Black, Diffuse = color, Specular = Colors.Gray, Shade = MaterialShade.Gouraud };
			return material;
		}
		#endregion

		/// <summary>
		/// Constructs a cubebox
		/// </summary>
		/// <param name="x">X position inside the cube grid</param>
		/// <param name="y">X position inside the cube grid</param>
		/// <param name="z">Z position inside the cube grid</param>
		public CubeBox(int x, int y, int z)
		{
			// Initialize the CubeBox position in the Cube grid
			X = x;
			Y = y;
			Z = z;

			// Set the fillet of the Chamfer, smooth corner
			Fillet = 0.5f;

			// Initialize the PivotPoint for the box
			InitializePivot();

			// Set the dimension of the Box
			Dimension = new Coordinate(BoxSize, BoxSize, BoxSize);

			// Initialize Materials
			SetMaterialOnSide(BoxSide.None, _black);
			SetMaterialsByGridPosition();
		}

		private void InitializePivot()
		{
			// Calculate the point in which the Box will rotate around
			var actualX = BoxXAlignment + (BoxXAdd * X);
			var actualY = BoxYAlignment + (BoxYAdd * Y);
			var actualZ = BoxZAlignment + (BoxZAdd * Z);
			PivotPoint = new Coordinate(-actualX, -actualY, -actualZ);
		}

		/// <summary>
		/// Gets the X position inside the grid
		/// </summary>
		public int X { get; private set; }
		public int Y { get; private set; }
		public int Z { get; private set; }

		private void SetMaterialsByGridPosition()
		{
			var front = _black;
			var back = _black;
			var left = _black;
			var right = _black;
			var top = _black;
			var bottom = _black;

			if (X == 0)
			{
				left = Materials[CubeColor.Orange];
			}
			else if (X == Cube.Width - 1)
			{
				right = Materials[CubeColor.Red];
			}

			if (Y == 0)
			{
				top = Materials[CubeColor.Blue];
			}
			else if (Y == Cube.Height - 1)
			{
				bottom = Materials[CubeColor.Green];
			}

			if (Z == 0)
			{
				front = Materials[CubeColor.White];
			}
			else if (Z == Cube.Depth - 1)
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

		/// <summary>
		/// Reset boxes colors to default positions
		/// </summary>
		public void Reset()
		{
			SetMaterialsByGridPosition();
		}

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
