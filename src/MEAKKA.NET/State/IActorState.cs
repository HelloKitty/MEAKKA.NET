using System;
using System.Collections.Generic;
using System.Text;

namespace MEAKKA
{
	/// <summary>
	/// Simple mutable actor state interface.
	/// </summary>
	/// <typeparam name="T">State type.</typeparam>
	public interface IMutableActorState<T>
	{
		/// <summary>
		/// The actor state data.
		/// </summary>
		T Data { get; set; }
	}

	/// <summary>
	/// Simple immutable actor state interface.
	/// (Immutable through this interface but does not promise it won't mutate).
	/// </summary>
	/// <typeparam name="T">State type.</typeparam>
	public interface IActorState<out T>
	{
		/// <summary>
		/// The actor state data.
		/// </summary>
		T Data { get; }
	}
}
