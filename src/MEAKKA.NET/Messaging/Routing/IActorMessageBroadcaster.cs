using System;
using System.Text;
using Akka.Actor;
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
}
