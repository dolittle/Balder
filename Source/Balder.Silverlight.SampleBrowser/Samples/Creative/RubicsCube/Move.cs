using System;
using System.ComponentModel;
using Balder.Extensions.Silverlight;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class Move : INotifyPropertyChanged
	{
		private CubeBoxGroup _group;
		public CubeBoxGroup Group
		{
			get { return _group; }
			set
			{
				_group = value;
				PropertyChanged.Notify(() => Group);
			}
		}

		private bool _clockWize;
		public bool ClockWize
		{
			get { return _clockWize; }
			set
			{
				_clockWize = value;
				PropertyChanged.Notify(() => ClockWize);
			}
		}

		private int _rotationCount;
		public int RotationCount
		{
			get { return _rotationCount; }
			set
			{
				_rotationCount = value;
				PropertyChanged.Notify(() => RotationCount);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		public bool HasRotation { get { return RotationCount > 0; } }

		public void SubtractRotation(int rotationCount)
		{
			RotationCount -= rotationCount;
			if (RotationCount < 0)
			{
				RotationCount = System.Math.Abs(RotationCount);
				ClockWize ^= true;
			}
		}

		public void AddRotation(int rotationCount)
		{
			RotationCount += rotationCount;
		}

		public void Optimize()
		{
			RotationCount = RotationCount % 4;
			if( RotationCount == 3 )
			{
				ClockWize ^= true;
				RotationCount = 1;
			}
		}
	}
}