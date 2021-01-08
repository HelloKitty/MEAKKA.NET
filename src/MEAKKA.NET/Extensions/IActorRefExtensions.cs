using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;

namespace MEAKKA
{
	public static class IActorRefExtensions
	{
		/// <summary>
		/// See Akka.NET's <see cref="IActorRef"/>.Tell.
		/// </summary>
		/// <param name="actorReference"></param>
		/// <param name="message"></param>
		public static void Tell(this IActorRef actorReference, EntityActorMessage message)
		{
			if (actorReference == null) throw new ArgumentNullException(nameof(actorReference));
			if (message == null) throw new ArgumentNullException(nameof(message));

			actorReference.Tell((object) message);
		}

		/// <summary>
		/// See Akka.NET's <see cref="IActorRef"/>.Tell.
		/// </summary>
		/// <param name="actorReference"></param>
		/// <param name="message"></param>
		/// <param name="sender"></param>
		public static void Tell(this IActorRef actorReference, EntityActorMessage message, IActorRef sender)
		{
			if(actorReference == null) throw new ArgumentNullException(nameof(actorReference));
			if(message == null) throw new ArgumentNullException(nameof(message));
			if (sender == null) throw new ArgumentNullException(nameof(sender));

			actorReference.Tell((object)message, sender);
		}

		/// <summary>
		/// See Akka.NET's <see cref="IActorRef"/>.Tell.
		/// Sends self with <see cref="actorReference"/> as the sender.
		/// </summary>
		/// <param name="actorReference"></param>
		/// <param name="message"></param>
		public static void TellSelf(this IActorRef actorReference, EntityActorMessage message)
		{
			if(actorReference == null) throw new ArgumentNullException(nameof(actorReference));
			if(message == null) throw new ArgumentNullException(nameof(message));

			actorReference.Tell((object)message, actorReference);
		}

		/// <summary>
		/// See Akka.NET's <see cref="IActorRef"/>.Tell.
		/// </summary>
		/// <param name="actorReference"></param>
		public static void Tell<TMessageType>(this IActorRef actorReference)
			where TMessageType : EntityActorMessage, new()
		{
			if(actorReference == null) throw new ArgumentNullException(nameof(actorReference));

			actorReference.Tell((object)new TMessageType());
		}

		/// <summary>
		/// See Akka.NET's <see cref="IActorRef"/>.Tell.
		/// </summary>
		/// <param name="actorReference"></param>
		/// <param name="sender"></param>
		public static void Tell<TMessageType>(this IActorRef actorReference, IActorRef sender)
			where TMessageType : EntityActorMessage, new()
		{
			if(actorReference == null) throw new ArgumentNullException(nameof(actorReference));
			if(sender == null) throw new ArgumentNullException(nameof(sender));

			actorReference.Tell((object)new TMessageType(), sender);
		}

		/// <summary>
		/// See Akka.NET's <see cref="IActorRef"/>.Tell.
		/// Sends self with <see cref="actorReference"/> as the sender.
		/// </summary>
		/// <param name="actorReference"></param>
		public static void TellSelf<TMessageType>(this IActorRef actorReference)
			where TMessageType : EntityActorMessage, new()
		{
			if(actorReference == null) throw new ArgumentNullException(nameof(actorReference));

			actorReference.Tell((object)new TMessageType(), actorReference);
		}

		/// <summary>
		/// Call's <see cref="IActorRef"/>.Tell which will send a <see cref="InitializeStateMessage{T}"/>
		/// containing the provided <see cref="state"/> value.
		/// </summary>
		/// <param name="actorReference">Actor to initialize state for.</param>
		/// <param name="state">The state value to initialize.</param>
		public static void InitializeState<T>(this IActorRef actorReference, T state)
		{
			if(actorReference == null) throw new ArgumentNullException(nameof(actorReference));

			actorReference.Tell(new InitializeStateMessage<T>(state));
		}

		/// <summary>
		/// Sends the specified <see cref="actorReference"/> a request message that implements <see cref="IActorRequestMessage{TResponseMessageType}"/>.
		/// Will async await upon a response of Type <typeparamref name="TResponseType"/>.
		/// </summary>
		/// <typeparam name="TRequestMessage">The request message to send async.</typeparam>
		/// <typeparam name="TResponseType">The response message type.</typeparam>
		/// <param name="actorReference">Actor target.</param>
		/// <param name="message">The message to send.</param>
		/// <param name="token">Cancel token.</param>
		/// <returns>The response.</returns>
		public static async Task<TResponseType> RequestAsync<TRequestMessage, TResponseType>(this IActorRef actorReference, TRequestMessage message, CancellationToken token = default)
			where TRequestMessage : IActorRequestMessage<TResponseType>
		{
			if (actorReference == null) throw new ArgumentNullException(nameof(actorReference));

			return await actorReference
				.Ask<TResponseType>(message, token);
		}
	}
}
