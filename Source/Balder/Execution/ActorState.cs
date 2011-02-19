namespace Balder.Execution
{
	/// <summary>
	/// State of an actor
	/// </summary>
	public enum ActorState
	{
		/// <summary>
		/// Actor is idle
		/// </summary>
		Idle=1,

		/// <summary>
		/// Actor is initializing
		/// </summary>
		Initialize,

		/// <summary>
		/// Actor is loading
		/// </summary>
		Load,

		/// <summary>
		/// Actor is in run state - execution
		/// </summary>
		Run
	}
}