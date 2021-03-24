using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.Routing;
using MEAKKA;

namespace MEAKKA
{
	/// <summary>
	/// Contract for a service that provides grouping and message broadcasting support.
	/// </summary>
	/// <typeparam name="TActorGroupType">The actor grouping enumeration.</typeparam>
	public interface IActorMessageBroadcaster<in TActorGroupType>
		where TActorGroupType : Enum
	{
		/// <summary>
		/// Broadcasts a message to the specified group type.
		/// </summary>
		/// <param name="group">The group type.</param>
		/// <param name="message">The message.</param>
		void BroadcastMessage(TActorGroupType group, EntityActorMessage message);

		/// <summary>
		/// Broadcasts a message to the specified group type.
		/// </summary>
		/// <param name="group">The group type.</param>
		/// <param name="message">The message.</param>
		/// <param name="sender">The actor sender of the message.</param>
		void BroadcastMessage(TActorGroupType group, EntityActorMessage message, IActorRef sender);

		/// <summary>
		/// Adds the specified actor to the specified group.
		/// </summary>
		/// <param name="group">The group type.</param>
		/// <param name="actor">The actor to add to the group.</param>
		void AddToGroup(TActorGroupType group, IActorRef actor);

		/// <summary>
		/// Removes the specified actor to the specified group.
		/// </summary>
		/// <param name="group">The group type.</param>
		/// <param name="actor">The actor to remove from the group.</param>
		void RemoveFromGroup(TActorGroupType group, IActorRef actor);
	}

	/// <summary>
	/// Default <see cref="IActorMessageBroadcaster{TActorGroupType}"/> implementation
	/// that maintains a non-threadsafe broadcast group map to broadcast messages to.
	/// </summary>
	/// <typeparam name="TActorGroupType"></typeparam>
	public sealed class DefaultGenericActorMessageBroadcaster<TActorGroupType> : IActorMessageBroadcaster<TActorGroupType> 
		where TActorGroupType : Enum
	{
		private Dictionary<TActorGroupType, IActorRef> BroadcastRouteMap { get; } = new();

		private IActorContext Parent { get; }

		public DefaultGenericActorMessageBroadcaster(IActorContext parent)
		{
			Parent = parent ?? throw new ArgumentNullException(nameof(parent));
		}

		/// <inheritdoc />
		public void BroadcastMessage(TActorGroupType group, EntityActorMessage message)
		{
			if (@group == null) throw new ArgumentNullException(nameof(@group));
			if (message == null) throw new ArgumentNullException(nameof(message));

			EnsureBroadcastGroupExists(group).Tell(new Broadcast(message));
		}

		/// <inheritdoc />
		public void BroadcastMessage(TActorGroupType group, EntityActorMessage message, IActorRef sender)
		{
			if (@group == null) throw new ArgumentNullException(nameof(@group));
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (sender == null) throw new ArgumentNullException(nameof(sender));

			EnsureBroadcastGroupExists(group).Tell(new Broadcast(message), sender);
		}

		/// <inheritdoc />
		public void AddToGroup(TActorGroupType group, IActorRef actor)
		{
			if (@group == null) throw new ArgumentNullException(nameof(@group));
			if (actor == null) throw new ArgumentNullException(nameof(actor));

			EnsureBroadcastGroupExists(group).Tell(new AddRoutee(Routee.FromActorRef(actor)));
		}

		/// <inheritdoc />
		public void RemoveFromGroup(TActorGroupType group, IActorRef actor)
		{
			if (@group == null) throw new ArgumentNullException(nameof(@group));
			if (actor == null) throw new ArgumentNullException(nameof(actor));

			EnsureBroadcastGroupExists(group).Tell(new RemoveRoutee(Routee.FromActorRef(actor)));
		}

		/// <summary>
		/// Ensures a broadcast actor exists for the specified group type.
		/// If it doesn't exist it creates it and puts it in <see cref="BroadcastRouteMap"/>.
		/// </summary>
		/// <param name="group">Broadcast group type.</param>
		/// <returns>The broadcast actor.</returns>
		/// <exception cref="ArgumentNullException">Throws if group is null.</exception>
		public IActorRef EnsureBroadcastGroupExists(TActorGroupType group)
		{
			if (@group == null) throw new ArgumentNullException(nameof(@group));

			if (!BroadcastRouteMap.ContainsKey(group))
				BroadcastRouteMap[group] = Parent.ActorOf(Props.Empty.WithRouter(new BroadcastGroup()));

			return BroadcastRouteMap[group];
		}
	}
}
