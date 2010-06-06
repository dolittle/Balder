using System;
using System.Collections.Generic;
using Balder.Core.Math;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class CubeBoxGroup
	{
		private Coordinate _rotation;


		public CubeBoxGroup()
		{
			Boxes = new List<CubeBox>();
			_rotation = new Coordinate();
		}

		public List<CubeBox> Boxes { get; private set; }

		public void Clear()
		{
			Boxes.Clear();
		}

		public void Snap()
		{
			_rotation.X = SnapAxis(_rotation.X);
			_rotation.Y = SnapAxis(_rotation.Y);
			_rotation.Z = SnapAxis(_rotation.Z);
			Rotate(_rotation.X, _rotation.Y, _rotation.Z);
		}

		private static double SnapAxis(double axisValue)
		{
			axisValue = (axisValue + 360d)%360d;

			if (axisValue < 45)
			{
				axisValue = 0;
			}
			else if (axisValue < 135)
			{
				axisValue = 90;
			}
			else if (axisValue < 225)
			{
				axisValue = 180;
			}
			else if (axisValue < 315)
			{
				axisValue = 270;
			} else
			{
				axisValue = 0;
			}
			return axisValue;
		}



		public void Add(CubeBox box)
		{
			Boxes.Add(box);
		}

		public void Rotate(double x, double y, double z)
		{
			foreach (var box in Boxes)
			{
				//box.Rotate(x, y, z);
			}
			_rotation.Set(x, y, z);
		}

		public void AddRotation(double x, double y, double z)
		{
			foreach (var box in Boxes)
			{
				box.AddRotation(x, y, z);
			}
			_rotation.X += x;
			_rotation.Y += y;
			_rotation.Z += z;
		}
	}
}