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
			if( _rotation.X < 45 )
			{
				_rotation.X = 0;
			} else if( _rotation.X < 135)
			{
				_rotation.X = 90;
			} else if( _rotation.X < 225)
			{
				_rotation.X = 180;
			} else if( _rotation.X < 315 )
			{
				_rotation.X = 270;
			}

			if (_rotation.Y < 45)
			{
				_rotation.Y = 0;
			}
			else if (_rotation.Y < 135)
			{
				_rotation.Y = 90;
			}
			else if (_rotation.Y < 225)
			{
				_rotation.Y = 180;
			}
			else if (_rotation.Y < 315)
			{
				_rotation.Y = 270;
			}
	
			Rotate(_rotation.X,_rotation.Y,_rotation.Z);
		}

		

		public void Add(CubeBox box)
		{
			Boxes.Add(box);
		}

		public void Rotate(double x, double y, double z)
		{
			foreach (var box in Boxes)
			{
				box.Rotate(x,y,z);
			}
			_rotation.Set(x,y,z);
		}

		public void AddRotation(double x, double y, double z)
		{
			foreach (var box in Boxes)
			{
				box.AddRotation(x,y,z);
			}
			_rotation.X += x;
			_rotation.Y += y;
			_rotation.Z += z;
		}
	}
}