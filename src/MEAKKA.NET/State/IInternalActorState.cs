using System;
using System.Collections.Generic;
using System.Text;

namespace MEAKKA
{
	/// <summary>
	/// Simple mutable actor state interface.
	/// This state is not automatically mutable from external messages (internal mutable by default).
	/// </summary>
	/// <typeparam name="T">State type.</typeparam>
	public interface IInternalMutableActorState<T>
	{
		/// <summary>
		/// The actor state data.
		/// </summary>
		T Data { get; set; }
	}

	/// <summary>
	/// Simple immutable actor state interface.
	/// (Immutable through this interface but does not promise it won't mutate but does promise control over external mutation through messages).
	/// This state is not automatically mutable from external messages (internal mutable by default).
	/// </summary>
	/// <typeparam name="T">State type.</typeparam>
	public interface IInternalActorState<out T>
	{
		/// <summary>
		/// The actor state data.
		/// </summary>
		T Data { get; }
	}
}
