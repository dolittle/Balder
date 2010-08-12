using System;

namespace Balder.Diagnostics
{
	public interface IStopwatch
	{
		TimeSpan Elapsed { get; }
		long ElapsedTicks { get; }
		long ElapsedMilliseconds { get; }
		bool IsRunning { get; }
		void Start();
		void Stop();
		void Reset();

	}
}