using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;

namespace MEAKKA
{
	/// <summary>
	/// The context of the entity actor message sent.
	/// </summary>
	public sealed class EntityActorMessageContext
	{
		/// <summary>
		/// Actor that sent the corresponding <see cref="EntityActorMessage"/>.
		/// </summary>
		public IActorRef Sender => ActorContext.Sender;

		/// <summary>
		/// Actor that is receiving this message.
		/// </summary>
		public IActorRef Entity => ActorContext.Self;

		/// <summary>
		/// The complete actor message context.
		/// </summary>
		public IUntypedActorContext ActorContext { get; }

		public EntityActorMessageContext(IUntypedActorContext actorContext)
		{
			ActorContext = actorContext ?? throw new ArgumentNullException(nameof(actorContext));
		}
	}
}
