using System;
using System.Collections.Generic;
using System.Linq;
using Balder.Core.Math;
using Balder.Core.Objects.Geometries;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class CubeBoxGroup
	{
		private readonly BoxSide _side;
		private double _angle;

		public CubeBoxGroup(BoxSide side)
		{
			_side = side;
			Boxes = new List<CubeBox>();
			_sideDefinitions = new List<BoxSide>();
			_sideBoxes = new List<CubeBox>();
		}

		public List<CubeBox> Boxes { get; private set; }

		private List<BoxSide> _sideDefinitions;
		private List<CubeBox> _sideBoxes;


		public void Clear()
		{
			Boxes.Clear();
		}

		public void Snap()
		{
			Action rotationAction;
			double positiveAngle;
			
			if( _angle < 0 )
			{
				rotationAction = RotateColors;
				positiveAngle = Math.Abs(_angle);
			} else
			{
				rotationAction = RotateColorsCounterClockWize;
				
				positiveAngle = _angle;
			}

			var rotationCount = (int)((positiveAngle / 90d) + 0.5d);

			foreach (var box in Boxes)
			{
				box.Reset();
			}

			for (var rotation = 0; rotation < rotationCount; rotation++ )
			{
				rotationAction();
			}
			_angle = 0;
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
			var c1 = _sideBoxes[10].GetColorForSide(_side);
			var c2 = _sideBoxes[11].GetColorForSide(_side);

			_sideBoxes[10].SetColorForSide(_side, _sideBoxes[7].GetColorForSide(_side));
			_sideBoxes[11].SetColorForSide(_side, _sideBoxes[8].GetColorForSide(_side));

			_sideBoxes[7].SetColorForSide(_side, _sideBoxes[4].GetColorForSide(_side));
			_sideBoxes[8].SetColorForSide(_side, _sideBoxes[5].GetColorForSide(_side));

			_sideBoxes[4].SetColorForSide(_side, _sideBoxes[1].GetColorForSide(_side));
			_sideBoxes[5].SetColorForSide(_side, _sideBoxes[2].GetColorForSide(_side));

			_sideBoxes[1].SetColorForSide(_side, c1);
			_sideBoxes[2].SetColorForSide(_side, c2);
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
			var c1 = _sideBoxes[1].GetColorForSide(_side);
			var c2 = _sideBoxes[2].GetColorForSide(_side);

			_sideBoxes[1].SetColorForSide(_side, _sideBoxes[4].GetColorForSide(_side));
			_sideBoxes[2].SetColorForSide(_side, _sideBoxes[5].GetColorForSide(_side));

			_sideBoxes[4].SetColorForSide(_side, _sideBoxes[7].GetColorForSide(_side));
			_sideBoxes[5].SetColorForSide(_side, _sideBoxes[8].GetColorForSide(_side));

			_sideBoxes[7].SetColorForSide(_side, _sideBoxes[10].GetColorForSide(_side));
			_sideBoxes[8].SetColorForSide(_side, _sideBoxes[11].GetColorForSide(_side));

			_sideBoxes[10].SetColorForSide(_side, c1);
			_sideBoxes[11].SetColorForSide(_side, c2);
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
			}
		}

		private Coordinate GetRotation(double angle)
		{
			var rotationVector = new Coordinate();
			switch (_side)
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


	}
}