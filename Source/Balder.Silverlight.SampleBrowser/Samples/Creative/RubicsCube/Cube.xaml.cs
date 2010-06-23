using System.Collections.Generic;
using System.Linq;
using Balder.Core.Display;
using Balder.Core.Input;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;
using Balder.Core.Silverlight.Helpers;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public partial class Cube
	{
		public delegate void RotateEventHandler(int deltaX, int deltaY);

		public const int Depth = 3;
		public const int Width = 3;
		public const int Height = 3;

		public event RotateEventHandler Rotate = (x, y) => { }; 

		private Dictionary<BoxSide, CubeBoxGroup> _groups;
		private CubeBoxGroup _manipulatingGroup;
		private CubeBox[, ,] _boxesGrid;

		public Cube()
		{
			InitializeComponent();
			SetupEvents();
		}

		private void SetupEvents()
		{
			ManipulationStarted += CubeManipulationStarted;
			ManipulationDelta += CubeManipulationDelta;
			ManipulationStopped += Cube_ManipulationStopped;
		}

		public Matrix ActualViewMatrix { get; set; }

		public DependencyProperty<Cube, MoveRecorder> MoveRecorderProperty =
			DependencyProperty<Cube, MoveRecorder>.Register(c => c.MoveRecorder);
		
		public MoveRecorder MoveRecorder
		{
			get { return MoveRecorderProperty.GetValue(this); }
			set 
			{ 
				MoveRecorderProperty.SetValue(this,value);
				InitializeMoveRecorder();
			}
		}

		public void Solve()
		{
			if( null != MoveRecorder )
			{
				MoveRecorder.Solve();
			}
		}

		public void Reset()
		{
			foreach( var group in _groups.Values )
			{
				group.Reset();
			}
			if (null != MoveRecorder)
			{
				MoveRecorder.Reset();
			}
		}


		private void InitializeMoveRecorder()
		{
			if( null != _groups && _groups.Count > 0 )
			{
				foreach( var group in _groups.Values )
				{
					MoveRecorder.AddGroup(group);
				}
			}
		}

		private void CubeManipulationStarted(object sender, ManipulationDeltaEventArgs args)
		{
			var cubeBox = args.OriginalSource as CubeBox;
			if (null == cubeBox)
			{
				return;
			}

			var manipulationNormal = new Vector(args.DeltaX, -args.DeltaY, 0);
			manipulationNormal.Normalize();

			if( null != args.Face )
			{
				var group = GetGroupForBoxFromManipulationDirection(cubeBox, manipulationNormal, args.Face.Normal);
				_manipulatingGroup = group;
				
			}
		}

		private CubeBoxGroup GetGroupForBoxFromManipulationDirection(CubeBox box, Vector direction, Vector faceNormal)
		{
			var groups = GetGroupsForBox(box);
			var rotatedDirection = Vector.TransformNormal(direction, ActualViewMatrix);

			var nearestDistance = float.MaxValue;
			CubeBoxGroup nearestGroup = null;

			foreach( var group in groups )
			{
				var distance = group.GetNearestDistanceForBoxManipulatedInGroup(box, rotatedDirection, faceNormal, ActualViewMatrix);
				if (distance < nearestDistance)
				{
					nearestDistance = distance;
					nearestGroup = group;
				}
			}

			return nearestGroup;
		}




		private CubeBoxGroup[]	GetGroupsForBox(CubeBox box)
		{
			var query = from g in _groups.Values
						where g.ContainsBox(box)
			            select g;
			return query.ToArray();
		}



		private void CubeManipulationDelta(object sender, ManipulationDeltaEventArgs args)
		{
			var cubeBox = args.OriginalSource as CubeBox;
			if (null == cubeBox)
			{
				return;
			}

			

			if ((cubeBox.X == 1 && cubeBox.Y == 1) ||
				(cubeBox.Y == 1 && cubeBox.Z == 1) ||
				(cubeBox.X == 1 && cubeBox.Z == 1))
			{
				Rotate(args.DeltaX, args.DeltaY);
			}
			else
			{
				if (null != _manipulatingGroup ) 
				{
					var positive = true;

					if( args.Direction == ManipulationDirection.Left || 
						args.Direction == ManipulationDirection.Right )
					{
						if( args.DeltaX < 0 )
						{
							positive = false;
						}
					} else
					{
						if( args.DeltaY < 0 )
						{
							positive = false;
						}
					}


					var vector = new Vector(args.DeltaX, args.DeltaY, 0);
					var length = vector.Length;
					if( ! positive )
					{
						length = -length;
					}
					

					_manipulatingGroup.Rotate(-length);
				}
			}
		}

		void Cube_ManipulationStopped(Core.INode sender, Core.Execution.BubbledEventArgs eventArgs)
		{
			if (null != _manipulatingGroup)
			{
				_manipulatingGroup.Snap();
			}
			_manipulatingGroup = null;
		}


		public override void Prepare(Viewport viewport)
		{
			PrepareCubeBoxGroups();
			GenerateCubeBoxes();
			OrganizeBoxes();
			base.Prepare(viewport);
		}


		private void PrepareCubeBoxGroups()
		{
			_groups = new Dictionary<BoxSide, CubeBoxGroup>();
			_groups[BoxSide.Front] = CreateCubeBoxGroup(BoxSide.Front);
			_groups[BoxSide.Back] = CreateCubeBoxGroup(BoxSide.Back);
			_groups[BoxSide.Left] = CreateCubeBoxGroup(BoxSide.Left);
			_groups[BoxSide.Right] = CreateCubeBoxGroup(BoxSide.Right);
			_groups[BoxSide.Top] = CreateCubeBoxGroup(BoxSide.Top);
			_groups[BoxSide.Bottom] = CreateCubeBoxGroup(BoxSide.Bottom);
		}

		private CubeBoxGroup CreateCubeBoxGroup(BoxSide side)
		{
			var group = new CubeBoxGroup(side);
			if (null != MoveRecorder)
			{
				MoveRecorder.AddGroup(group);
			}
			return group;
		}

		private void GenerateCubeBoxes()
		{
			_boxesGrid = new CubeBox[Width, Height, Depth];
			for (var z = 0; z < Depth; z++)
			{
				for (var y = 0; y < Height; y++)
				{
					for (var x = 0; x < Width; x++)
					{
						var box = new CubeBox(x, y, z);
						Children.Add(box);
						_boxesGrid[x, y, z] = box;
					}
				}
			}
		}

		private void OrganizeBoxes()
		{
			OrganizeFrontBoxes();
			OrganizeBackBoxes();
			OrganizeLeftBoxes();
			OrganizeRightBoxes();
			OrganizeTopBoxes();
			OrganizeBottomBoxes();
		}

		private void OrganizeFrontBoxes()
		{
			var group = _groups[BoxSide.Front];

			group.Add(_boxesGrid[0, 0, 0], BoxSide.Left, BoxSide.Top);
			group.Add(_boxesGrid[1, 0, 0], BoxSide.Top);
			group.Add(_boxesGrid[2, 0, 0], BoxSide.Top, BoxSide.Right);
			group.Add(_boxesGrid[2, 1, 0], BoxSide.Right);
			group.Add(_boxesGrid[2, 2, 0], BoxSide.Right, BoxSide.Bottom);
			group.Add(_boxesGrid[1, 2, 0], BoxSide.Bottom);
			group.Add(_boxesGrid[0, 2, 0], BoxSide.Bottom, BoxSide.Left);
			group.Add(_boxesGrid[0, 1, 0], BoxSide.Left);
			group.Add(_boxesGrid[1, 1, 0]);
		}

		private void OrganizeBackBoxes()
		{
			var group = _groups[BoxSide.Back];

			group.Add(_boxesGrid[0, 0, 2], BoxSide.Left, BoxSide.Top);
			group.Add(_boxesGrid[1, 0, 2], BoxSide.Top);
			group.Add(_boxesGrid[2, 0, 2], BoxSide.Top, BoxSide.Right);
			group.Add(_boxesGrid[2, 1, 2], BoxSide.Right);
			group.Add(_boxesGrid[2, 2, 2], BoxSide.Right, BoxSide.Bottom);
			group.Add(_boxesGrid[1, 2, 2], BoxSide.Bottom);
			group.Add(_boxesGrid[0, 2, 2], BoxSide.Bottom, BoxSide.Left);
			group.Add(_boxesGrid[0, 1, 2], BoxSide.Left);
			group.Add(_boxesGrid[1, 1, 2]);
		}

		private void OrganizeLeftBoxes()
		{
			var group = _groups[BoxSide.Left];

			group.Add(_boxesGrid[0, 0, 2], BoxSide.Back, BoxSide.Top);
			group.Add(_boxesGrid[0, 0, 1], BoxSide.Top);
			group.Add(_boxesGrid[0, 0, 0], BoxSide.Top, BoxSide.Front);
			group.Add(_boxesGrid[0, 1, 0], BoxSide.Front);
			group.Add(_boxesGrid[0, 2, 0], BoxSide.Front, BoxSide.Bottom);
			group.Add(_boxesGrid[0, 2, 1], BoxSide.Bottom);
			group.Add(_boxesGrid[0, 2, 2], BoxSide.Bottom, BoxSide.Back);
			group.Add(_boxesGrid[0, 1, 2], BoxSide.Back);
			group.Add(_boxesGrid[0, 1, 1]);
		}

		private void OrganizeRightBoxes()
		{
			var group = _groups[BoxSide.Right];

			group.Add(_boxesGrid[2, 0, 2], BoxSide.Back, BoxSide.Top);
			group.Add(_boxesGrid[2, 0, 1], BoxSide.Top);
			group.Add(_boxesGrid[2, 0, 0], BoxSide.Top, BoxSide.Front);
			group.Add(_boxesGrid[2, 1, 0], BoxSide.Front);
			group.Add(_boxesGrid[2, 2, 0], BoxSide.Front, BoxSide.Bottom);
			group.Add(_boxesGrid[2, 2, 1], BoxSide.Bottom);
			group.Add(_boxesGrid[2, 2, 2], BoxSide.Bottom, BoxSide.Back);
			group.Add(_boxesGrid[2, 1, 2], BoxSide.Back);
			group.Add(_boxesGrid[2, 1, 1]);
		}

		private void OrganizeTopBoxes()
		{
			var group = _groups[BoxSide.Top];

			group.Add(_boxesGrid[2, 0, 2], BoxSide.Right, BoxSide.Back);
			group.Add(_boxesGrid[1, 0, 2], BoxSide.Back);
			group.Add(_boxesGrid[0, 0, 2], BoxSide.Back, BoxSide.Left);
			group.Add(_boxesGrid[0, 0, 1], BoxSide.Left);
			group.Add(_boxesGrid[0, 0, 0], BoxSide.Left, BoxSide.Front);
			group.Add(_boxesGrid[1, 0, 0], BoxSide.Front);
			group.Add(_boxesGrid[2, 0, 0], BoxSide.Front, BoxSide.Right);
			group.Add(_boxesGrid[2, 0, 1], BoxSide.Right);
			group.Add(_boxesGrid[1, 0, 1]);
		}

		private void OrganizeBottomBoxes()
		{
			var group = _groups[BoxSide.Bottom];

			group.Add(_boxesGrid[2, 2, 2], BoxSide.Right, BoxSide.Back);
			group.Add(_boxesGrid[1, 2, 2], BoxSide.Back);
			group.Add(_boxesGrid[0, 2, 2], BoxSide.Back, BoxSide.Left);
			group.Add(_boxesGrid[0, 2, 1], BoxSide.Left);
			group.Add(_boxesGrid[0, 2, 0], BoxSide.Left, BoxSide.Front);
			group.Add(_boxesGrid[1, 2, 0], BoxSide.Front);
			group.Add(_boxesGrid[2, 2, 0], BoxSide.Front, BoxSide.Right);
			group.Add(_boxesGrid[2, 2, 1], BoxSide.Right);
			group.Add(_boxesGrid[1, 2, 1]);
		}
	}
}
