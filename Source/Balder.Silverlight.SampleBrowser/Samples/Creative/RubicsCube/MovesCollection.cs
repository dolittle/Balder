using System.Collections.ObjectModel;

namespace Balder.Silverlight.SampleBrowser.Samples.Creative.RubicsCube
{
	public class MovesCollection : ObservableCollection<Move>
	{
		public bool IsEmpty
		{
			get { return Count == 0; }
		}

		public bool LastMoveIsGroup(CubeBoxGroup group)
		{
			if( IsEmpty )
			{
				return false;
			}
			return LastMove.Group.Equals(group);
		}

		public bool IsLastMoveClockWize()
		{
			if( IsEmpty )
			{
				return false;
			}

			return LastMove.ClockWize;
		}

		public bool IsLastMoveSameDirectionAs(bool clockWize)
		{
			if( IsEmpty )
			{
				return false;
			}

			return LastMove.ClockWize == clockWize;
		}

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