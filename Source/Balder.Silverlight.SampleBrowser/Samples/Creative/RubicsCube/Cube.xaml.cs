using System.Collections.Generic;
using Balder.Core.Display;
using Balder.Core.Input;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public partial class Cube
	{
		public delegate void ManipulateGroupEventHandler(CubeBoxGroup group, ManipulationDeltaEventArgs args);

		public const int Depth = 3;
		public const int Width = 3;
		public const int Height = 3;

		private Dictionary<BoxSide, CubeBoxGroup> _groups;
		private CubeBoxGroup _manipulatingGroup;
		private ManipulateGroupEventHandler _manipulateGroupHandler;


		public Cube()
		{
			InitializeComponent();
			
			SetupEvents();
		}

		private void SetupEvents()
		{
			ManipulationStarted += Cube_ManipulationStarted;
			ManipulationDelta += Cube_ManipulationDelta;
			ManipulationStopped += Cube_ManipulationStopped;
		}


		void Cube_ManipulationStarted(object sender, ManipulationDeltaEventArgs args)
		{
			var cubeBox = args.OriginalSource as CubeBox;
			if (null == cubeBox)
			{
				return;
			}

			if (null != args.Face)
			{
				var rotation = Matrix.CreateRotation(
					(float)cubeBox.Rotation.X,
					(float)cubeBox.Rotation.Y,
					(float)cubeBox.Rotation.Z);

				var normal = Vector.TransformNormal(args.Face.Normal, rotation);
				normal.Normalize();

				var upLength = (Vector.Up - normal).Length;
				var frontLength = (Vector.Backward - normal).Length;
				var rightLength = (Vector.Right - normal).Length;

				_manipulatingGroup = null;
				if (upLength < 0.1)
				{
					if (args.Direction == ManipulationDirection.Up ||
						args.Direction == ManipulationDirection.Down)
					{
						var BoxSides = new[] { BoxSide.Front, BoxSide.Back };
						_manipulatingGroup = FindBoxInGroup(BoxSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.Rotate(a.DeltaY);
					}
				}
				else if (frontLength < 0.1)
				{
					if (args.Direction == ManipulationDirection.Left ||
						args.Direction == ManipulationDirection.Right)
					{
						var BoxSides = new[] { BoxSide.Top, BoxSide.Bottom };
						_manipulatingGroup = FindBoxInGroup(BoxSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.Rotate(-a.DeltaX);
					}
					else if (args.Direction == ManipulationDirection.Up ||
							  args.Direction == ManipulationDirection.Down)
					{
						var BoxSides = new[] { BoxSide.Left, BoxSide.Right };
						_manipulatingGroup = FindBoxInGroup(BoxSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.Rotate(-a.DeltaY);
					}
				}
				else if (rightLength < 0.1)
				{
					if (args.Direction == ManipulationDirection.Left ||
						args.Direction == ManipulationDirection.Right)
					{
						var BoxSides = new[] { BoxSide.Top, BoxSide.Bottom };
						_manipulatingGroup = FindBoxInGroup(BoxSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.Rotate(-a.DeltaX);
					}
					else if (args.Direction == ManipulationDirection.Up ||
							  args.Direction == ManipulationDirection.Down)
					{
						var BoxSides = new[] { BoxSide.Front, BoxSide.Back };
						_manipulatingGroup = FindBoxInGroup(BoxSides, cubeBox);
						_manipulateGroupHandler = (g, a) => g.Rotate(-a.DeltaY);
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

			if (null != _manipulatingGroup && null != _manipulateGroupHandler)
			{
				_manipulateGroupHandler(_manipulatingGroup, args);
			}
		}

		void Cube_ManipulationStopped(Core.INode sender, Core.Execution.BubbledEventArgs eventArgs)
		{
			if (null != _manipulatingGroup)
			{
				_manipulatingGroup.Snap();
			}

			_manipulatingGroup = null;
			_manipulateGroupHandler = null;
		}

		private CubeBoxGroup FindBoxInGroup(IEnumerable<BoxSide> BoxSides, CubeBox cubeBox)
		{
			foreach (var BoxSide in BoxSides)
			{
				var group = _groups[BoxSide];
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


		public override void Prepare(Viewport viewport)
		{
			PrepareCubeBoxGroups();
			GenerateCubeBoxes();
			base.Prepare(viewport);
		}


		private void PrepareCubeBoxGroups()
		{
			_groups = new Dictionary<BoxSide, CubeBoxGroup>();
			_groups[BoxSide.Front] = new CubeBoxGroup(BoxSide.Front);
			_groups[BoxSide.Back] = new CubeBoxGroup(BoxSide.Back);
			_groups[BoxSide.Left] = new CubeBoxGroup(BoxSide.Left);
			_groups[BoxSide.Right] = new CubeBoxGroup(BoxSide.Right);
			_groups[BoxSide.Top] = new CubeBoxGroup(BoxSide.Top);
			_groups[BoxSide.Bottom] = new CubeBoxGroup(BoxSide.Bottom);
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
						AddBoxToGroups(box, x, y, z);
					}
				}
			}
		}

		private void AddBoxToGroups(CubeBox box, int x, int y, int z)
		{
			if (x == 0)
			{
				_groups[BoxSide.Left].Add(box);
			}
			if (x == Width - 1)
			{
				_groups[BoxSide.Right].Add(box);
			}
			if (y == 0)
			{
				_groups[BoxSide.Top].Add(box);
			}
			if (y == Height - 1)
			{
				_groups[BoxSide.Bottom].Add(box);
			}
			if (z == 0)
			{
				_groups[BoxSide.Front].Add(box);
			}
			if (z == Depth - 1)
			{
				_groups[BoxSide.Back].Add(box);
			}
		}
	}
}
