using System;
using System.Collections.Generic;
using System.Text;

namespace MEAKKA
{
	/// <summary>
	/// Simple generic mutable <see cref="IActorState{T}"/> implementation.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public sealed class MutableGenericActorState<T> : IActorState<T>, IMutableActorState<T>
	{
		/// <inheritdoc />
		public T Data { get; set; }

		//To not confuse IoC we don't add a CTOR with this as a parameter.
		/// <summary>
		/// Empty uninitialized version of mutable state.
		/// </summary>
		public MutableGenericActorState()
		{
			
		}
	}
}
