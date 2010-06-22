using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Threading;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public delegate void CubeBoxGroupSnappedEventHandler(CubeBoxGroup group, bool clockWize, int rotationCount);

	public class CubeBoxGroup
	{
		public event CubeBoxGroupSnappedEventHandler Snapped = (g, a, c) => { };

		private static readonly Dictionary<BoxSide, Vector> SideNormals = new Dictionary<BoxSide, Vector>
		                                                                  	{
		                                                                  		{BoxSide.Top, Vector.Up},
																				{BoxSide.Bottom, Vector.Down},
																				{BoxSide.Left, Vector.Left},
																				{BoxSide.Right, Vector.Right},
																				{BoxSide.Front, Vector.Backward},
																				{BoxSide.Back, Vector.Forward},
																				{BoxSide.None, Vector.Zero}
		                                                                  	};

		private double _angle;
		private readonly List<BoxSide> _sideDefinitions;
		private readonly List<CubeBox> _sideBoxes;
		private readonly Dictionary<BoxSide, Vector> _normals;

		public CubeBoxGroup(BoxSide side)
		{
			Side = side;
			Boxes = new List<CubeBox>();
			_sideDefinitions = new List<BoxSide>();
			_sideBoxes = new List<CubeBox>();
			_normals = new Dictionary<BoxSide, Vector>();
		}

		public List<CubeBox> Boxes { get; private set; }
		public BoxSide Side { get; private set; }


		public Vector[] GetManipulationNormals()
		{
			var keys = (from k in _normals.Keys
						  where !k.Equals(Side)
			              select k).ToArray();

			var normals = new List<Vector>();

			foreach( var key in keys )
			{
				normals.Add(_normals[key]);
			}

			return normals.ToArray();
		}

		public bool IsBoxManipulatedInGroup(CubeBox cubeBox, Vector direction, Matrix viewMatrix)
		{
			for (var boxIndex = 0; boxIndex < _sideBoxes.Count; boxIndex++ )
			{
				var box = _sideBoxes[boxIndex];
				var side = _sideDefinitions[boxIndex];

				if( box.Equals(cubeBox))
				{
					var normal = _normals[side];
					var rotatedNormal = Vector.TransformNormal(normal, viewMatrix);

					var distance = Vector.Distance(direction, rotatedNormal);
					if( distance < 1f )
					{
						return true;
					}
				}
			}
			return false;
			
		}


		public void Clear()
		{
			Boxes.Clear();
		}

		public void Snap()
		{
			Snap(true);
		}

		public void Reset()
		{
			foreach( var box in Boxes )
			{
				box.Reset();
			}
		}

		public bool ContainsBox(CubeBox box)
		{
			var query = from b in Boxes
						where b.Equals(box)
			            select b;
			return null != query.SingleOrDefault();
		}

		private void Snap(bool callSnapped)
		{
			Action<int> rotationAction;
			double positiveAngle;
			bool clockWize;

			if (_angle < 0)
			{
				rotationAction = RotateColors;
				positiveAngle = Math.Abs(_angle);
				clockWize = true;
			}
			else
			{
				rotationAction = RotateColorsCounterClockWize;
				positiveAngle = _angle;
				clockWize = false;
			}

			var rotationCount = (int)((positiveAngle / 90d) + 0.5d);

			foreach (var box in Boxes)
			{
				box.ResetRotation();
			}

			rotationAction(rotationCount);

			if( callSnapped )
			{
				Snapped(this, clockWize, rotationCount);	
			}
			_angle = 0;
		}

		private void RotateColors(int times)
		{
			for (var rotation = 0; rotation < times; rotation++)
			{
				RotateColors();
			}
		}

		private void RotateColorsCounterClockWize(int times)
		{
			for (var rotation = 0; rotation < times; rotation++)
			{
				RotateColorsCounterClockWize();
			}
		}

		private void RotateColors()
		{
			RotateFront();
			RotateSides();
		}

		private void RotateColorsCounterClockWize()
		{
			RotateFrontCounterClockWize();
			RotateSidesCounterClockWize();
		}

		private void RotateFront()
		{
			var c1 = _sideBoxes[10].GetColorForSide(Side);
			var c2 = _sideBoxes[11].GetColorForSide(Side);

			_sideBoxes[10].SetColorForSide(Side, _sideBoxes[7].GetColorForSide(Side));
			_sideBoxes[11].SetColorForSide(Side, _sideBoxes[8].GetColorForSide(Side));

			_sideBoxes[7].SetColorForSide(Side, _sideBoxes[4].GetColorForSide(Side));
			_sideBoxes[8].SetColorForSide(Side, _sideBoxes[5].GetColorForSide(Side));

			_sideBoxes[4].SetColorForSide(Side, _sideBoxes[1].GetColorForSide(Side));
			_sideBoxes[5].SetColorForSide(Side, _sideBoxes[2].GetColorForSide(Side));

			_sideBoxes[1].SetColorForSide(Side, c1);
			_sideBoxes[2].SetColorForSide(Side, c2);
		}

		private void RotateSides()
		{
			var c1 = _sideBoxes[10].GetColorForSide(_sideDefinitions[10]);
			var c2 = _sideBoxes[11].GetColorForSide(_sideDefinitions[11]);
			var c3 = _sideBoxes[0].GetColorForSide(_sideDefinitions[0]);

			_sideBoxes[10].SetColorForSide(_sideDefinitions[10], _sideBoxes[7].GetColorForSide(_sideDefinitions[7]));
			_sideBoxes[11].SetColorForSide(_sideDefinitions[11], _sideBoxes[8].GetColorForSide(_sideDefinitions[8]));
			_sideBoxes[0].SetColorForSide(_sideDefinitions[0], _sideBoxes[9].GetColorForSide(_sideDefinitions[9]));

			_sideBoxes[7].SetColorForSide(_sideDefinitions[7], _sideBoxes[4].GetColorForSide(_sideDefinitions[4]));
			_sideBoxes[8].SetColorForSide(_sideDefinitions[8], _sideBoxes[5].GetColorForSide(_sideDefinitions[5]));
			_sideBoxes[9].SetColorForSide(_sideDefinitions[9], _sideBoxes[6].GetColorForSide(_sideDefinitions[6]));

			_sideBoxes[4].SetColorForSide(_sideDefinitions[4], _sideBoxes[1].GetColorForSide(_sideDefinitions[1]));
			_sideBoxes[5].SetColorForSide(_sideDefinitions[5], _sideBoxes[2].GetColorForSide(_sideDefinitions[2]));
			_sideBoxes[6].SetColorForSide(_sideDefinitions[6], _sideBoxes[3].GetColorForSide(_sideDefinitions[3]));

			_sideBoxes[1].SetColorForSide(_sideDefinitions[1], c1);
			_sideBoxes[2].SetColorForSide(_sideDefinitions[2], c2);
			_sideBoxes[3].SetColorForSide(_sideDefinitions[3], c3);
		}

		private void RotateFrontCounterClockWize()
		{
			var c1 = _sideBoxes[1].GetColorForSide(Side);
			var c2 = _sideBoxes[2].GetColorForSide(Side);

			_sideBoxes[1].SetColorForSide(Side, _sideBoxes[4].GetColorForSide(Side));
			_sideBoxes[2].SetColorForSide(Side, _sideBoxes[5].GetColorForSide(Side));

			_sideBoxes[4].SetColorForSide(Side, _sideBoxes[7].GetColorForSide(Side));
			_sideBoxes[5].SetColorForSide(Side, _sideBoxes[8].GetColorForSide(Side));

			_sideBoxes[7].SetColorForSide(Side, _sideBoxes[10].GetColorForSide(Side));
			_sideBoxes[8].SetColorForSide(Side, _sideBoxes[11].GetColorForSide(Side));

			_sideBoxes[10].SetColorForSide(Side, c1);
			_sideBoxes[11].SetColorForSide(Side, c2);
		}


		private void RotateSidesCounterClockWize()
		{
			var c1 = _sideBoxes[1].GetColorForSide(_sideDefinitions[1]);
			var c2 = _sideBoxes[2].GetColorForSide(_sideDefinitions[2]);
			var c3 = _sideBoxes[3].GetColorForSide(_sideDefinitions[3]);

			_sideBoxes[1].SetColorForSide(_sideDefinitions[1], _sideBoxes[4].GetColorForSide(_sideDefinitions[4]));
			_sideBoxes[2].SetColorForSide(_sideDefinitions[2], _sideBoxes[5].GetColorForSide(_sideDefinitions[5]));
			_sideBoxes[3].SetColorForSide(_sideDefinitions[3], _sideBoxes[6].GetColorForSide(_sideDefinitions[6]));

			_sideBoxes[4].SetColorForSide(_sideDefinitions[4], _sideBoxes[7].GetColorForSide(_sideDefinitions[7]));
			_sideBoxes[5].SetColorForSide(_sideDefinitions[5], _sideBoxes[8].GetColorForSide(_sideDefinitions[8]));
			_sideBoxes[6].SetColorForSide(_sideDefinitions[6], _sideBoxes[9].GetColorForSide(_sideDefinitions[9]));

			_sideBoxes[7].SetColorForSide(_sideDefinitions[7], _sideBoxes[10].GetColorForSide(_sideDefinitions[10]));
			_sideBoxes[8].SetColorForSide(_sideDefinitions[8], _sideBoxes[11].GetColorForSide(_sideDefinitions[11]));
			_sideBoxes[9].SetColorForSide(_sideDefinitions[9], _sideBoxes[0].GetColorForSide(_sideDefinitions[0]));

			_sideBoxes[10].SetColorForSide(_sideDefinitions[10], c1);
			_sideBoxes[11].SetColorForSide(_sideDefinitions[11], c2);
			_sideBoxes[0].SetColorForSide(_sideDefinitions[0], c3);
		}


		public void Add(CubeBox box, params BoxSide[] sides)
		{
			Boxes.Add(box);

			foreach (var side in sides)
			{
				_sideDefinitions.Add(side);
				_sideBoxes.Add(box);

				_normals[side] = SideNormals[side];
			}
		}

		private Coordinate GetRotation(double angle)
		{
			var rotationVector = new Coordinate();
			switch (Side)
			{
				case BoxSide.Front:
				case BoxSide.Back:
					{
						rotationVector = new Coordinate(0, 0, angle);
					}
					break;
				case BoxSide.Left:
				case BoxSide.Right:
					{
						rotationVector = new Coordinate(angle, 0, 0);
					}
					break;
				case BoxSide.Top:
				case BoxSide.Bottom:
					{
						rotationVector = new Coordinate(0, angle, 0);
					}
					break;


			}
			return rotationVector;

		}

		public void Rotate(double angle)
		{
			_angle += angle;
			var rotation = GetRotation(angle);
			foreach (var box in Boxes)
			{
				box.Rotate(rotation);
			}
		}


		private const int RotationFrames = 10;
		private int _framesToRate;
		private double _anglesPerFrame;
		private EventHandler _rotationDone;

		private void RotationTimer(object sender, EventArgs e)
		{
			Rotate(_anglesPerFrame);
			if (--_framesToRate <= 0)
			{
				Snap(false);

				if( null != _rotationDone )
				{
					_rotationDone(this, EventArgs.Empty);
				}

				if (sender is DispatcherTimer)
				{
					var timer = sender as DispatcherTimer;
					timer.Stop();
				}
			}
		}

		private void StartRotationAnimation(int times, bool clockWize, EventHandler rotationDone)
		{
			_framesToRate = RotationFrames;
			if (clockWize)
			{
				_anglesPerFrame = -(((double)(90 * times)) / (double)RotationFrames);
			}
			else
			{
				_anglesPerFrame = ((double)(90 * times)) / (double)RotationFrames;
			}
			_rotationDone = rotationDone;

			var timer = new DispatcherTimer();
			timer.Interval = TimeSpan.FromMilliseconds(20);
			timer.Tick += RotationTimer;
			timer.Start();
		}

		public void Rotate(int times, bool animate, EventHandler rotationDone)
		{
			if (animate)
			{
				StartRotationAnimation(times, true, rotationDone);
			}
			else
			{
				RotateColors(times);
			}
		}

		public void RotateCounterClockWize(int times, bool animate, EventHandler rotationDone)
		{
			if (animate)
			{
				StartRotationAnimation(times, false, rotationDone);
			}
			else
			{
				RotateColorsCounterClockWize(times);
			}
		}
	}
}