using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Balder.Core.Silverlight.Extensions;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class MoveRecorder
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
					PropertyChanged.Notify(()=>ClockWize);
				}
			}

			private int	_rotationCount;
			public int RotationCount
			{
				get { return _rotationCount; }
				set
				{
					_rotationCount = value;
					PropertyChanged.Notify(()=>RotationCount);
				}
			}

			public event PropertyChangedEventHandler PropertyChanged = (s, e) => { }; 
		}

		public ObservableCollection<Move> Moves { get; private set; }

		public MoveRecorder()
		{
			Moves = new ObservableCollection<Move>();
		}

		public void Reset()
		{
			Moves.Clear();
		}

		public void AddGroup(CubeBoxGroup group)
		{
			group.Snapped += GroupSnapped;

		}

		private void GroupSnapped(CubeBoxGroup group, bool clockWize, int rotationCount)
		{
			if (rotationCount == 0)
			{
				return;
			}

			

			if (Moves.Count > 0 &&
				Moves[Moves.Count - 1].Group.Equals(group) &&
				Moves[Moves.Count - 1].ClockWize == clockWize)
			{
				var move = Moves[Moves.Count - 1];
				move.RotationCount += rotationCount;
			}
			else
			{
				var move = new Move {Group = group, ClockWize = clockWize, RotationCount = rotationCount};
				Moves.Add(move);
			}
		}


		private int _currentMove;

		private void NextMove(object sender, EventArgs e)
		{
			if (_currentMove < 0)
			{
				Moves.Clear();
				return;
			}
			var move = Moves[_currentMove];

			if (move.ClockWize)
			{
				move.Group.RotateCounterClockWize(move.RotationCount, true, NextMove);
			}
			else
			{
				move.Group.Rotate(move.RotationCount, true, NextMove);
			}

			_currentMove--;
		}

		public void Solve()
		{
			_currentMove = Moves.Count - 1;
			if (_currentMove >= 0)
			{
				NextMove(this, EventArgs.Empty);
			}
		}
	}
}
