using System.Collections.ObjectModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class MoveRecorder
	{
		public class Move
		{
			public CubeBoxGroup Group { get; set; }
			public bool ClockWize { get; set; }
			public int RotationCount { get; set; }

			public override string ToString()
			{
				var description = string.Format("{0} rotated {1} {2}",
												Group.Side,
												RotationCount,
												ClockWize ? "clockwize" : "counter clockwize");
				return description;
			}
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
			var move = new Move { Group = group, ClockWize = clockWize, RotationCount = rotationCount };
			Moves.Add(move);
		}

		public void Solve()
		{
			for (var moveIndex = Moves.Count - 1; moveIndex >= 0; moveIndex--)
			{
				var move = Moves[moveIndex];

				if (move.ClockWize)
				{
					move.Group.RotateCounterClockWize(move.RotationCount, true);
				}
				else
				{
					move.Group.Rotate(move.RotationCount, true);
				}
			}

			Moves.Clear();
		}
	}
}
