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
			ManipulationStarted += Cube_ManipulationStarted;
			ManipulationDelta += Cube_ManipulationDelta;
			ManipulationStopped += Cube_ManipulationStopped;
		}

		private CubeBoxGroup _manipulatingGroup;

		public delegate void ManipulateGroupEventHandler(CubeBoxGroup group, ManipulationDeltaEventArgs args);

		private ManipulateGroupEventHandler _manipulateGroupHandler;


		void Cube_ManipulationStarted(object sender, ManipulationDeltaEventArgs args)
		{
			var cubeBox = args.OriginalSource as CubeBox;
			if (null == cubeBox)
			{
				return;
			}

			if (null != args.Face)
			{
				cubeBox.FaceNormal = args.Face.Normal;

				var rotation = Matrix.CreateRotation(
					(float)cubeBox.Rotation.X,
					(float)cubeBox.Rotation.Y,
					(float)cubeBox.Rotation.Z);

				var normal = Vector.TransformNormal(args.Face.Normal, rotation);
				normal.Normalize();
				cubeBox.CurrentFaceNormal = normal;

				var upLength = (Vector.Up - normal).Length;
				var frontLength = (Vector.Backward - normal).Length;
				var downLength = (Vector.Down - normal).Length;
				var leftLength = (Vector.Left - normal).Length;
				var rightLength = (Vector.Right - normal).Length;
				var backLength = (Vector.Forward - normal).Length;

				cubeBox.NormalLengths = string.Format("{0}, {1}, {2}, {3}, {4}, {5}", upLength, downLength, leftLength, rightLength, frontLength, backLength);

				if (upLength < 0.1)
				{
					if (args.Direction == ManipulationDirection.Up ||
						args.Direction == ManipulationDirection.Down)
					{
						var cubeSides = new[] { CubeSide.Front, CubeSide.Back };
						_manipulatingGroup = FindBoxInGroup(cubeSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.AddRotation(0, 0, a.DeltaY);
					}
				}
				else if (frontLength < 0.1)
				{
					if (args.Direction == ManipulationDirection.Left ||
						args.Direction == ManipulationDirection.Right)
					{
						var cubeSides = new[] { CubeSide.Top, CubeSide.Bottom };
						_manipulatingGroup = FindBoxInGroup(cubeSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.AddRotation(0, -a.DeltaX,  0);
					}
					else if (args.Direction == ManipulationDirection.Up ||
							  args.Direction == ManipulationDirection.Down)
					{
						var cubeSides = new[] { CubeSide.Left, CubeSide.Right };
						_manipulatingGroup = FindBoxInGroup(cubeSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.AddRotation(-a.DeltaY,0,0);
					}
				}
				else if (rightLength < 0.1)
				{
					if (args.Direction == ManipulationDirection.Left ||
						args.Direction == ManipulationDirection.Right)
					{
						var cubeSides = new[] { CubeSide.Top, CubeSide.Bottom };
						_manipulatingGroup = FindBoxInGroup(cubeSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.AddRotation(0, -a.DeltaX, 0);
					}
					else if (args.Direction == ManipulationDirection.Up ||
							  args.Direction == ManipulationDirection.Down)
					{
						var cubeSides = new[] { CubeSide.Front, CubeSide.Back };
						_manipulatingGroup = FindBoxInGroup(cubeSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.AddRotation(0, 0, -a.DeltaY);
					}
				}
			}
		}


		void Cube_ManipulationDelta(object sender, ManipulationDeltaEventArgs args)
		{
			var cubeBox = args.OriginalSource as CubeBox;
			if (null == cubeBox)
			{
				return;
			}

			if( null != _manipulatingGroup && null != _manipulateGroupHandler )
			{
				_manipulateGroupHandler(_manipulatingGroup, args);
			}
		}

		void Cube_ManipulationStopped(Core.INode sender, Core.Execution.BubbledEventArgs eventArgs)
		{
			SnapGroups();
			OrganizeCubeBoxesInGroups();
			
			_manipulatingGroup = null;
			_manipulateGroupHandler = null;
		}

		private CubeBoxGroup FindBoxInGroup(IEnumerable<CubeSide> cubeSides, CubeBox cubeBox)
		{
			foreach (var cubeSide in cubeSides)
			{
				var group = _groups[cubeSide];
				foreach (var box in group.Boxes)
				{
					if (box.Equals(cubeBox))
					{
						return group;
					}
				}
			}
			return null;
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
				if (box.IsFront)
				{
					_groups[CubeSide.Front].Add(box);
				}
				else if (box.IsBack)
				{
					_groups[CubeSide.Back].Add(box);
				}

				if (box.IsLeft)
				{
					_groups[CubeSide.Left].Add(box);
				}
				else if (box.IsRight)
				{
					_groups[CubeSide.Right].Add(box);
				}

				if (box.IsTop)
				{
					_groups[CubeSide.Top].Add(box);
				}
				else if (box.IsBottom)
				{
					_groups[CubeSide.Bottom].Add(box);
				}
			}
		}

		private void ClearGroups()
		{
			foreach (var group in _groups.Values)
			{
				group.Clear();
			}
		}

		private void SnapGroups()
		{
			foreach (var group in _groups.Values)
			{
				group.Snap();
			}
		}
	}

}
