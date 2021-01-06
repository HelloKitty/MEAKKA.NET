using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Akka.Actor;
using Glader.Essentials;
using MEAKKA;

namespace MEAKKA
{
	/// <summary>
	/// Marks a <see cref="IMessageHandler{TMessageType,TMessageContext}"/> as usable
	/// by a specific Entity type.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public sealed class ActorMessageHandlerAttribute : Attribute
	{
		/// <summary>
		/// The actor type this handler can be used by.
		/// </summary>
		public Type ActorType { get; }

		public ActorMessageHandlerAttribute(Type actorType)
		{
			if (!typeof(IInternalActor).IsAssignableFrom(actorType))
				throw new InvalidOperationException($"{actorType.Name} is not an actor type. Cannot be used in: {typeof(ActorMessageHandlerAttribute)}");

			ActorType = actorType;
		}
	}
}
