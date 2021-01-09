using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Akka.Actor;

namespace MEAKKA
{
	public static class ActorSelectionExtensions
	{
		/// <summary>
		/// Tells an <see cref="ActorSelection"/> the provided <see cref="message"/>.
		/// </summary>
		/// <param name="selection">The actor selection.</param>
		/// <param name="message">The message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntitySelection(this ActorSelection selection, EntityActorMessage message)
		{
			selection
				.Tell(message);
		}

		/// <summary>
		/// Tells an <see cref="ActorSelection"/> the provided <see cref="message"/>.
		/// </summary>
		/// <param name="selection">The actor selection.</param>
		/// <param name="message">The message.</param>
		/// <param name="sender">The sender of the message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntitySelection(this ActorSelection selection, EntityActorMessage message, IActorRef sender)
		{
			selection
				.Tell(message, sender);
		}

		/// <summary>
		/// Tells an <see cref="ActorSelection"/> the provided <see cref="message"/>.
		/// </summary>
		/// <param name="selection">The actor selection.</param>
		/// <param name="message">The message.</param>
		/// <param name="sender">The sender of the message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntitySelection(this ActorSelection selection, EntityActorMessage message, IEntityActorRef sender)
		{
			TellEntitySelection(selection, message, sender.Actor);
		}

		/// <summary>
		/// Tells an <see cref="ActorSelection"/> the provided paramterlessly constructed <typeparamref name="TMessageType"/>.
		/// </summary>
		/// <param name="selection">The actor selection.</param>
		/// <param name="sender">The sender of the message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntitySelection<TMessageType>(this ActorSelection selection, IActorRef sender)
			where TMessageType : EntityActorMessage, new()
		{
			TellEntitySelection(selection, new TMessageType(), sender);
		}

		/// <summary>
		/// Tells an actor's direct children the provided <see cref="message"/>.
		/// </summary>
		/// <param name="selector">The ActorSelection provider.</param>
		/// <param name="message">The message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntityChildren(this IActorRefFactory selector, EntityActorMessage message)
		{
			selector
				.ActorSelection(MEAKKASelectionConstants.ALL_DIRECT_CHILDREN_SELECTOR)
				.TellEntitySelection(message);
		}

		/// <summary>
		/// Tells an actor's direct children the provided paramterlessly constructed <typeparamref name="TMessageType"/>.
		/// </summary>
		/// <param name="selector">The ActorSelection provider.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntityChildren<TMessageType>(this IActorRefFactory selector) 
			where TMessageType : EntityActorMessage, new()
		{
			TellEntityChildren(selector, new TMessageType());
		}

		/// <summary>
		/// Tells an actor's direct children the provided paramterlessly constructed <typeparamref name="TMessageType"/>.
		/// </summary>
		/// <param name="selector">The ActorSelection provider.</param>
		/// <param name="sender">The sender of the message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntityChildren<TMessageType>(this IActorRefFactory selector, IActorRef sender)
			where TMessageType : EntityActorMessage, new()
		{
			selector
				.ActorSelection(MEAKKASelectionConstants.ALL_DIRECT_CHILDREN_SELECTOR)
				.TellEntitySelection(new TMessageType(), sender);
		}

		/// <summary>
		/// Tells an actor's direct children the provided paramterlessly constructed <typeparamref name="TMessageType"/>.
		/// </summary>
		/// <param name="selector">The ActorSelection provider.</param>
		/// <param name="sender">The sender of the message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntityChildren<TMessageType>(this IActorRefFactory selector, IEntityActorRef sender)
			where TMessageType : EntityActorMessage, new()
		{
			TellEntityChildren<TMessageType>(selector, sender.Actor);
		}

		/// <summary>
		/// Tells an actor's direct children the provided <see cref="message"/>
		/// </summary>
		/// <param name="selector">The ActorSelection provider.</param>
		/// <param name="message">The message.</param>
		/// <param name="sender">The sender of the message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntityChildren(this IActorRefFactory selector, EntityActorMessage message, IActorRef sender)
		{
			selector
				.ActorSelection(MEAKKASelectionConstants.ALL_DIRECT_CHILDREN_SELECTOR)
				.TellEntitySelection(message, sender);
		}

		/// <summary>
		/// Tells an actor's direct children the provided <see cref="message"/>
		/// </summary>
		/// <param name="selector">The ActorSelection provider.</param>
		/// <param name="message">The message.</param>
		/// <param name="sender">The sender of the message.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void TellEntityChildren(this IActorRefFactory selector, EntityActorMessage message, IEntityActorRef sender)
		{
			TellEntityChildren(selector, message, sender.Actor);
		}
	}
}
