using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.DI.Core;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Basic creation context for an actor.
	/// </summary>
	public sealed class ActorCreationContext
	{
		/// <summary>
		/// The factory responsible for creating the actor
		/// reference.
		/// (Using this instead of global reference factory creates child/parent tree relationships.)
		/// </summary>
		public IActorRefFactory ActorReferenceFactory { get; }

		public ActorCreationContext(IActorRefFactory actorReferenceFactory)
		{
			ActorReferenceFactory = actorReferenceFactory ?? throw new ArgumentNullException(nameof(actorReferenceFactory));
		}
	}

	/// <summary>
	/// Contract for types that can create actors of <typeparamref name="TActorType"/>.
	/// Returning <see cref="IActorRef"/> from creation context <see cref="ActorCreationContext"/>.
	/// </summary>
	/// <typeparam name="TActorType">The actor type.</typeparam>
	public interface IActorFactory<out TActorType> : IFactoryCreatable<IActorRef, ActorCreationContext>
		where TActorType : IInternalActor
	{

	}
}
