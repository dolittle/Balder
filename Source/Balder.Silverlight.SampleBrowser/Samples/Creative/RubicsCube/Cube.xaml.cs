﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Balder.Core.Display;
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

	public enum DragDirection
	{
		None = 1,
		Left,
		Right,
		Up,
		Down
	}

	public partial class Cube
	{
		public const int Depth = 3;
		public const int Width = 3;
		public const int Height = 3;

		private Dictionary<CubeSide, CubeBoxGroup> _groups;
		private bool _isDragging = false;
		private DragDirection _dragDirection;
		private Point _previousPosition;

		public Cube()
		{
			InitializeComponent();
			PrepareCubeBoxGroups();
			SetupEvents();
		}

		private void SetupEvents()
		{
			MouseLeftButtonDown += Cube_MouseLeftButtonDown;
			MouseLeftButtonUp += Cube_MouseLeftButtonUp;
			MouseLeave += Cube_MouseLeave;
			MouseMove += Cube_MouseMove;
		}

		void Cube_MouseLeave(object sender, MouseEventArgs e)
		{
			_isDragging = false;
		}

		void Cube_MouseMove(object sender, MouseEventArgs e)
		{
			if (_isDragging)
			{
				var position = e.GetPosition(this);
				var deltaX = position.X - _previousPosition.X;
				var deltaY = position.Y - _previousPosition.Y;

				InitializeDragDirection(deltaX, deltaY);

				var cubeBox = e.OriginalSource as CubeBox;

				if( _dragDirection == DragDirection.Left || 
					_dragDirection == DragDirection.Right )
				{
					_groups[CubeSide.Top].AddRotation(0, -deltaX, 0);

					/*
					var group = _groups[CubeSide.Left];
					foreach( var box in group.Boxes )
					{
						if( box.Equals(cubeBox))
						{
							group.AddRotation(0, -deltaX, 0);
							break;
						}
					}

					group = _groups[CubeSide.Right];
					foreach (var box in group.Boxes)
					{
						if (box.Equals(cubeBox))
						{
							group.AddRotation(0, -deltaX, 0);
							break;
						}
					}*/
					
				}


				if (_dragDirection == DragDirection.Up ||
					_dragDirection == DragDirection.Down)
				{
					_groups[CubeSide.Right].AddRotation(-deltaY, 0, 0);
				}

				_previousPosition = position;
			}
		}

		private void InitializeDragDirection(double deltaX, double deltaY)
		{
			if (_dragDirection == DragDirection.None)
			{
				var absoluteDeltaX = Math.Abs(deltaX);
				var absoluteDeltaY = Math.Abs(deltaY);

				if (absoluteDeltaX > absoluteDeltaY)
				{
					if (deltaX <= 0)
					{
						_dragDirection = DragDirection.Left;
					}
					else
					{
						_dragDirection = DragDirection.Right;
					}
				}
				else
				{
					if (deltaY <= 0)
					{
						_dragDirection = DragDirection.Up;
					}
					else
					{
						_dragDirection = DragDirection.Down;
					}
				}
			}
		}

		void Cube_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			_isDragging = false;
		}

		void Cube_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			_isDragging = true;
			_dragDirection = DragDirection.None;

			_previousPosition = e.GetPosition(this);
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
