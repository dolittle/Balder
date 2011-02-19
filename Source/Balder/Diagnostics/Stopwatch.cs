using System;

namespace Balder.Diagnostics
{
		/// <summary>
	/// Represents a <see cref="IStopwatch"/> implementation
	/// 
	/// Based upon Tiaan Gelenhuys Stopwatch : http://blog.tiaan.com/link/2009/02/03/stopwatch-silverlight
	/// </summary>
	public class Stopwatch : IStopwatch
	{
		/// <summary>
		/// Gets or sets wether or not to use high resolution.
		/// 
		/// Default is false
		/// </summary>
		public static bool IsHighResolution = false;

		/// <summary>
		/// Gets or sets the frequency for the stopwatch
		/// 
		/// Default is <see cref="TimeSpan.TicksPerSecond"/>
		/// </summary>
		public static long Frequency = TimeSpan.TicksPerSecond;

#pragma warning disable 1591 // Xml Comments
		public TimeSpan Elapsed
		{
			get
			{
				if (!StartUtc.HasValue)
				{
					return TimeSpan.Zero;
				}
				if (!EndUtc.HasValue)
				{
					return (DateTime.UtcNow - StartUtc.Value);
				}
				return (EndUtc.Value - StartUtc.Value);
			}
		}

		public long ElapsedMilliseconds
		{
			get
			{
				return ElapsedTicks / TimeSpan.TicksPerMillisecond;
			}
		}
		public long ElapsedTicks { get { return Elapsed.Ticks; } }
		public bool IsRunning { get; private set; }
		private DateTime? StartUtc { get; set; }
		private DateTime? EndUtc { get; set; }

		public static long GetTimestamp()
		{
			return DateTime.UtcNow.Ticks;
		}

		public void Reset()
		{
			Stop();
			EndUtc = null;
			StartUtc = null;
		}

		public void Start()
		{
			if (IsRunning)
			{
				return;
			}
			if ((StartUtc.HasValue) &&
				(EndUtc.HasValue))
			{
				// Resume the timer from its previous state
				StartUtc = StartUtc.Value +
					(DateTime.UtcNow - EndUtc.Value);
			}
			else
			{
				// Start a new time-interval from scratch
				StartUtc = DateTime.UtcNow;
			}
			IsRunning = true;
			EndUtc = null;
		}

		public void Stop()
		{
			if (IsRunning)
			{
				IsRunning = false;
				EndUtc = DateTime.UtcNow;
			}
		}

		public static Stopwatch StartNew()
		{
			var stopwatch = new Stopwatch();
			stopwatch.Start();
			return stopwatch;
		}

#pragma warning restore 1591 // Xml Comments
	}
}
