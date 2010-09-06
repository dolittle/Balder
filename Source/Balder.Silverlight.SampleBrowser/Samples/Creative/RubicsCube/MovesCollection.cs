using System.Collections.ObjectModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	/// <summary>
	/// Represents collection of moves
	/// </summary>
	public class MovesCollection : ObservableCollection<Move>
	{
		/// <summary>
		/// Get wether or not the collection is empty
		/// </summary>
		public bool IsEmpty { get { return Count == 0; } }

		/// <summary>
		/// Check if the last move in the collection is for a given group
		/// </summary>
		/// <param name="group">Group to check for </param>
		/// <returns>True if the last move is the group, false if not</returns>
		public bool LastMoveIsGroup(CubeBoxGroup group)
		{
			if( IsEmpty )
			{
				return false;
			}
			return LastMove.Group.Equals(group);
		}

		/// <summary>
		/// Check if last move is a clockwize move
		/// </summary>
		/// <returns>True if last move is clockwize, false if not</returns>
		public bool IsLastMoveClockWize()
		{
			if( IsEmpty )
			{
				return false;
			}

			return LastMove.ClockWize;
		}

		/// <summary>
		/// Check if last move is a specific direction
		/// </summary>
		/// <param name="clockWize">True if direction to check is clockwize, false if not</param>
		/// <returns>True if specified direction is same as for last move</returns>
		public bool IsLastMoveSameDirectionAs(bool clockWize)
		{
			if( IsEmpty )
			{
				return false;
			}

			return LastMove.ClockWize == clockWize;
		}

		/// <summary>
		/// Get last move, null if there is no move in the collection
		/// </summary>
		public Move LastMove
		{
			get
			{
				if( IsEmpty )
				{
					return null;
				}
				return this[Count - 1];
			}
		}
	}
}