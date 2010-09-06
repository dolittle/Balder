using System;
using System.ComponentModel;
using Balder.Extensions.Silverlight;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	/// <summary>
	/// Represents a move done in the Rubiks Cube
	/// </summary>
	public class Move : INotifyPropertyChanged
	{
		/// <summary>
		/// Event for when a property is changed
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged = (s, e) => { };

		private CubeBoxGroup _group;

		/// <summary>
		///  Get or set the group the move is done on
		/// </summary>
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

		/// <summary>
		/// Get or set wether or not the move is clockwize
		/// </summary>
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

		/// <summary>
		/// Get or set the number of rotations the move represents
		/// </summary>
		public int RotationCount
		{
			get { return _rotationCount; }
			set
			{
				_rotationCount = value;
				PropertyChanged.Notify(() => RotationCount);
			}
		}

		
		/// <summary>
		/// Get wether or not the move represents a rotation
		/// </summary>
		public bool HasRotation { get { return RotationCount > 0; } }


		/// <summary>
		/// Subtract rotation from the move
		/// </summary>
		/// <param name="rotationCount">Number of rotations</param>
		public void SubtractRotation(int rotationCount)
		{
			RotationCount -= rotationCount;
			if (RotationCount < 0)
			{
				RotationCount = System.Math.Abs(RotationCount);
				ClockWize ^= true;
			}
		}


		/// <summary>
		/// Add rotation to the move
		/// </summary>
		/// <param name="rotationCount">Number of rotations</param>
		public void AddRotation(int rotationCount)
		{
			RotationCount += rotationCount;
		}

		/// <summary>
		/// Optimize move
		/// </summary>
		public void Optimize()
		{
			// Since a group can only rotate 4 times to complete a full 360 degrees
			// we don't need to store counts more than 4, we can optimize it
			RotationCount = RotationCount % 4;

			// If the rotation count is 3 - its the same as rotating the group 
			// once in the oposite direction
			if( RotationCount == 3 )
			{
				ClockWize ^= true;
				RotationCount = 1;
			}
		}
	}
}