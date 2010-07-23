using System;

namespace Balder.Execution
{
	/// <summary>
	/// Represents an action for a message
	/// </summary>
	/// <typeparam name="T">Message it represents</typeparam>
	public class MessageAction<T>
	{
		public MessageAction(object target, Action<T> action)
		{
			Target = new WeakReference(target);
			Action = action;
		}

		public WeakReference Target { get; private set; }
		public Action<T> Action { get; private set; }
	}
}