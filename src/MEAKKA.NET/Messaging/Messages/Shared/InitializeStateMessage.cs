using System;
using System.Collections.Generic;
using System.Text;

namespace MEAKKA
{
	/// <summary>
	/// Actor message ot initialize state.
	/// </summary>
	/// <typeparam name="T">The state type.</typeparam>
	public sealed class InitializeStateMessage<T> : EntityActorMessage
	{
		/// <summary>
		/// The state value to initialize.
		/// </summary>
		public T State { get; private set; }

		public InitializeStateMessage(T state)
		{
			State = state ?? throw new ArgumentNullException(nameof(state));
		}
	}
}
