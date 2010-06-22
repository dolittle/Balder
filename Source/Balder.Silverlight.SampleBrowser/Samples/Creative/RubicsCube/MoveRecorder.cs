using System;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class MoveRecorder
	{
		public MovesCollection Moves { get; private set; }

		public MoveRecorder()
		{
			Moves = new MovesCollection();
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
			Move move = null;

			if (Moves.LastMoveIsGroup(group) &&
				Moves.IsLastMoveSameDirectionAs(clockWize))
			{
				move = Moves.LastMove;
				move.AddRotation(rotationCount);
			}
			else if (Moves.LastMoveIsGroup(group))
			{
				move = Moves.LastMove;
				move.SubtractRotation(rotationCount);
			} 
			else 
			{
				move = new Move {Group = group, ClockWize = clockWize, RotationCount = rotationCount};
				Moves.Add(move);
			}

			move.Optimize();
			if (!move.HasRotation)
			{
				Moves.Remove(move);
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
			if (_currentMove >= 0 && Moves.Count > 0)
			{
				Moves.RemoveAt(_currentMove);
			}

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
