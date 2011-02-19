using System;

namespace Balder.Diagnostics
{
	/// <summary>
	/// Defines a stopwatch for measuring time
	/// </summary>
	public interface IStopwatch
	{
		/// <summary>
		/// Gets elapsed time since its start
		/// </summary>
		TimeSpan Elapsed { get; }

		/// <summary>
		/// Gets elapsed ticks since its start
		/// </summary>
		long ElapsedTicks { get; }

		/// <summary>
		/// Gets elapsed milliseconds since its start
		/// </summary>
		long ElapsedMilliseconds { get; }

		/// <summary>
		/// Gets state for wether or not it is running
		/// </summary>
		bool IsRunning { get; }

		/// <summary>
		/// Start the stopwatch
		/// </summary>
		void Start();

		/// <summary>
		/// Stop the stopwatch
		/// </summary>
		void Stop();

		/// <summary>
		/// Reset the stopwatch timer
		/// </summary>
		void Reset();

	}
}