using System;
using System.Collections.Generic;
using System.Text;

namespace GladMMO
{
	/// <summary>
	/// Contract for message type that initializes an actor.
	/// </summary>
	/// <typeparam name="TStateType">State type.</typeparam>
	public interface IEntityActorStateInitializeMessage<out TStateType>
	{
		/// <summary>
		/// State to initialize to.
		/// </summary>
		TStateType State { get; }
	}

	/// <summary>
	/// Default implementation of <see cref="IEntityActorStateInitializeMessage{TStateType}"/>
	/// </summary>
	/// <typeparam name="TStateType"></typeparam>
	public sealed class EntityActorStateInitializeMessage<TStateType> : EntityActorMessage, IEntityActorStateInitializeMessage<TStateType>
		where TStateType : class
	{
		public TStateType State { get; }

		public EntityActorStateInitializeMessage(TStateType state)
		{
			State = state ?? throw new ArgumentNullException(nameof(state));
		}
	}
}
