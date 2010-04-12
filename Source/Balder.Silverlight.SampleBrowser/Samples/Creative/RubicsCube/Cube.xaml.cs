using System.Collections.Generic;
using Balder.Core.Math;

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
		public const int Depth = 3;
		public const int Width = 3;
		public const int Height = 3;

		public Cube()
		{
			InitializeComponent();
			PrepareCubeBoxGroups();
		}

		public class CubeBoxGroup
		{
			public CubeBoxGroup()
			{
				Boxes = new List<CubeBox>();
			}

			public List<CubeBox> Boxes { get; private set; }

			public void Clear()
			{
				Boxes.Clear();
			}

			public void Add(CubeBox box)
			{
				Boxes.Add(box);
			}

			public void Rotate(double x, double y, double z)
			{
				foreach (var box in Boxes)
				{
					box.Rotation.Set(x, y, z);
				}
			}
		}


		private Dictionary<CubeSide, CubeBoxGroup> _groups;

		private void PrepareCubeBoxGroups()
		{
			_groups = new Dictionary<CubeSide, CubeBoxGroup>();
			_groups[CubeSide.Front] = new CubeBoxGroup();
			_groups[CubeSide.Back] = new CubeBoxGroup();
			_groups[CubeSide.Left] = new CubeBoxGroup();
			_groups[CubeSide.Right] = new CubeBoxGroup();
			_groups[CubeSide.Top] = new CubeBoxGroup();
			_groups[CubeSide.Bottom] = new CubeBoxGroup();
		}

		protected override void Prepare()
		{
			GenerateCubeBoxes();
			OrganizeCubeBoxesInGroups();

			_groups[CubeSide.Top].Rotate(0, 12, 0);
			base.Prepare();
		}


		private void GenerateCubeBoxes()
		{
			for (var z = 0; z < Depth; z++)
			{
				for (var y = 0; y < Height; y++)
				{
					for (var x = 0; x < Width; x++)
					{
						var box = new CubeBox(x, y, z);
						Children.Add(box);
					}
				}
			}
		}

		private void OrganizeCubeBoxesInGroups()
		{
			foreach (CubeBox box in Children)
			{
				box.UpdateNormals();

				if (box.CalculatedFrontNormal.Equals(Vector.Backward))
				{
					_groups[CubeSide.Front].Add(box);
				}
				else if (box.CalculatedFrontNormal.Equals(Vector.Forward))
				{
					_groups[CubeSide.Back].Add(box);
				}

				if (box.CalculatedSideNormal.Equals(Vector.Left))
				{
					_groups[CubeSide.Left].Add(box);
				}
				else if (box.CalculatedSideNormal.Equals(Vector.Right))
				{
					_groups[CubeSide.Right].Add(box);
				}

				if (box.CalculatedUpNormal.Equals(Vector.Up))
				{
					_groups[CubeSide.Top].Add(box);
				}
				else if (box.CalculatedUpNormal.Equals(Vector.Down))
				{
					_groups[CubeSide.Bottom].Add(box);
				}
			}
		}
	}
}
