using System.Collections.Generic;
using Balder.Core.Display;
using Balder.Core.Input;
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

		private Dictionary<CubeSide, CubeBoxGroup> _groups;

		public Cube()
		{
			InitializeComponent();
			PrepareCubeBoxGroups();
			SetupEvents();
		}

		private void SetupEvents()
		{
			ManipulationDelta += Cube_ManipulationDelta;
			ManipulationStopped += Cube_ManipulationStopped;
		}


		void Cube_ManipulationDelta(object sender, ManipulationDeltaEventArgs args)
		{
			var cubeBox = args.OriginalSource as CubeBox;
			if (null == cubeBox)
			{
				return;
			}
			if (args.Direction == ManipulationDirection.Left || 
				args.Direction == ManipulationDirection.Right)
			{
				var cubeSides = new[] { CubeSide.Top, CubeSide.Bottom };
				FindBoxInGroupAndRotate(args.DeltaX, 0, cubeSides, cubeBox);
			} else if (	args.Direction == ManipulationDirection.Up || 
						args.Direction == ManipulationDirection.Down)
			{
				var cubeSides = new[] { CubeSide.Left, CubeSide.Right };
				FindBoxInGroupAndRotate(0, args.DeltaY, cubeSides, cubeBox);
			}
		}

		void Cube_ManipulationStopped(Core.INode sender, Core.Execution.BubbledEventArgs eventArgs)
		{
			SnapGroups();
			OrganizeCubeBoxesInGroups();
		}

		private void FindBoxInGroupAndRotate(double deltaX, double deltaY, IEnumerable<CubeSide> cubeSides, CubeBox cubeBox)
		{
			foreach (var cubeSide in cubeSides)
			{
				var group = _groups[cubeSide];
				foreach (var box in group.Boxes)
				{
					if (box.Equals(cubeBox))
					{
						group.AddRotation(-deltaY, -deltaX, 0);
						break;
					}
				}
			}
		}



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

		public override void Prepare(Viewport viewport)
		{
			GenerateCubeBoxes();
			OrganizeCubeBoxesInGroups();
			base.Prepare(viewport);
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
			ClearGroups();
			foreach (CubeBox box in Children)
			{
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

		private void ClearGroups()
		{
			foreach( var group in _groups.Values )
			{
				group.Clear();
			}
		}

		private void SnapGroups()
		{
			foreach( var group in _groups.Values )
			{
				group.Snap();
			}
		}
	}

}
