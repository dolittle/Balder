using System.Collections.Generic;
using Balder.Core.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class CubeBoxGroup
	{
		private readonly CubeSide _side;
		private double _angle;

		public CubeBoxGroup(CubeSide side)
		{
			_side = side;
			Boxes = new List<CubeBox>();
		}

		public List<CubeBox> Boxes { get; private set; }

		public void Clear()
		{
			Boxes.Clear();
		}

		public void Snap()
		{
			foreach (var box in Boxes)
			{
				box.Reset();
			}
		}

		public void Add(CubeBox box)
		{
			Boxes.Add(box);
		}

		private Coordinate GetRotation(double angle)
		{
			var rotationVector = new Coordinate();
			switch( _side )
			{
				case CubeSide.Front:
				case CubeSide.Back:
					{
						rotationVector = new Coordinate(0,0,angle);
					}
					break;
				case CubeSide.Left:
				case CubeSide.Right:
					{
						rotationVector = new Coordinate(angle, 0, 0);
					}
					break;
				case CubeSide.Top:
				case CubeSide.Bottom:
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