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
			return this[Count - 1].Group.Equals(group);
		}

		public bool LastGroupClockWize()
		{
			if( IsEmpty )
			{
				return false;
			}

			return this[Count - 1].ClockWize;
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