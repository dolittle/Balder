using System;

namespace Balder.Diagnostics
{
	/// <summary>
	/// Represents a null implementation of a <see cref="IStopwatch"/>
	/// 
	/// It does in fact nothing
	/// </summary>
	public class NullStopwatch : IStopwatch
	{
#pragma warning disable 1591 // Xml Comments
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
#pragma warning restore 1591 // Xml Comments
}