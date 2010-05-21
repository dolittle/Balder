using System.Collections.Generic;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class CubeBoxGroup
	{
		public CubeBoxGroup()
		{
			Boxes = new List<CubeBox>();
		}

		public List<CubeBox> Boxes { get; private set; }

		public void Clear()
		{
			Boxes.Clear();
		}

		public void Add(CubeBox box)
		{
			Boxes.Add(box);
		}

		public void Rotate(double x, double y, double z)
		{
			foreach (var box in Boxes)
			{
				box.Rotation.Set(x, y, z);
			}
		}

		public void AddRotation(double x, double y, double z)
		{
			foreach (var box in Boxes)
			{
				box.Rotation.X += x;
				box.Rotation.Y += y;
				box.Rotation.Z += z;
			}
		}
	}
}