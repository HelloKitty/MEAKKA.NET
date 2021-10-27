using System;
using System.Collections.Generic;
using Akka.Actor;
using Akka.Routing;

namespace MEAKKA
{
	/// <summary>
	/// Deterministic ordered <see cref="IActorMessageBroadcaster{TActorGroupType}"/> implementation
	/// that maintains a non-threadsafe broadcast group map to broadcast messages to.
	/// </summary>
	/// <typeparam name="TActorGroupType"></typeparam>
	public sealed class DeterministicActorMessageBroadcaster<TActorGroupType> : IActorMessageBroadcaster<TActorGroupType> 
		where TActorGroupType : Enum
	{
		private Dictionary<TActorGroupType, IActorRef> BroadcastRouteMap { get; } = new();

		private IActorContext Parent { get; }

		public DeterministicActorMessageBroadcaster(IActorContext parent)
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

			EnsureBroadcastGroupExists(group).Tell(new AddRoutee(Routee.FromActorRef(actor)), actor);
		}

		/// <inheritdoc />
		public void RemoveFromGroup(TActorGroupType group, IActorRef actor)
		{
			if (@group == null) throw new ArgumentNullException(nameof(@group));
			if (actor == null) throw new ArgumentNullException(nameof(actor));

			EnsureBroadcastGroupExists(group).Tell(new RemoveRoutee(Routee.FromActorRef(actor)), actor);
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
				BroadcastRouteMap[group] = Parent.ActorOf<DeterministicRouterActor>();

			return BroadcastRouteMap[group];
		}
	}
}