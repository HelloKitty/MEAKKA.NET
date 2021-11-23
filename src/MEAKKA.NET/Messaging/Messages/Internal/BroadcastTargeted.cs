using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;

namespace MEAKKA
{
	/// <summary>
	/// Internal message used to broadcast a message to a specific actor target.
	/// </summary>
	internal sealed class BroadcastTargeted : Broadcast
	{
		//TODO: This won't be serializable
		/// <summary>
		/// The target of the message.
		/// </summary>
		public IActorRef Target { get; }

		public BroadcastTargeted(EntityActorMessage message, IActorRef target) 
			: base(message)
		{
			Target = target ?? throw new ArgumentNullException(nameof(target));
		}
	}
}
