using System;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	/// <summary>
	/// Manages the recording and playback of moves
	/// </summary>
	public class MoveRecorder
	{
		private int _currentMove;

		/// <summary>
		/// Gets the collection of moves
		/// </summary>
		public MovesCollection Moves { get; private set; }


		/// <summary>
		/// Constructs the MoveRecorder
		/// </summary>
		public MoveRecorder()
		{
			Moves = new MovesCollection();
		}

		/// <summary>
		/// Reset all moves
		/// </summary>
		public void Reset()
		{
			Moves.Clear();
		}

		/// <summary>
		/// Add group to the recorder
		/// </summary>
		/// <param name="group"></param>
		public void AddGroup(CubeBoxGroup group)
		{
			// Hook up the snapped event, so we can record every time a group has been snapped
			group.Snapped += GroupSnapped;
		}

		/// <summary>
		/// Solve the cube
		/// </summary>
		public void Solve()
		{
			_currentMove = Moves.Count - 1;
			if (_currentMove >= 0)
			{
				NextMove(this, EventArgs.Empty);
			}
		}

		/// <summary>
		/// Gets called when the group has snapped
		/// </summary>
		/// <param name="group">Group that has snapped</param>
		/// <param name="clockWize">ClockWize snap or not</param>
		/// <param name="rotationCount">How many times it was rotated</param>
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

			// If move doesn't have any rotation left - remove it
			if (!move.HasRotation)
			{
				Moves.Remove(move);
			}
		}


		/// <summary>
		/// Gets called when its time for the next move, or used directly internally for going to the next move
		/// </summary>
		/// <param name="sender">Sender</param>
		/// <param name="e">Event Args</param>
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
	}
}
