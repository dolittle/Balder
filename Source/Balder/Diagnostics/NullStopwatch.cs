using System;

namespace Balder.Diagnostics
{
	public class NullStopwatch : IStopwatch
	{
		public TimeSpan Elapsed { get; private set; }
		public long ElapsedTicks { get; private set; }
		public long ElapsedMilliseconds { get; private set; }
		public bool IsRunning { get; private set; }
		public void Start()
		{
			
		}

		public void Stop()
		{
			
		}

		public void Reset()
		{
			
		}
	}
}