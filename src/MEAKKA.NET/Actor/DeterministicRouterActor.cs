//-----------------------------------------------------------------------
// <copyright file="RouterActor.cs" company="Akka.NET Project">
//     Copyright (C) 2009-2021 Lightbend Inc. <http://www.lightbend.com>
//     Copyright (C) 2013-2021 .NET Foundation <https://github.com/akkadotnet/akka.net>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Akka.Actor;
using Akka.Routing;

//From: https://github.com/akkadotnet/akka.net/blob/21762b90ac1d7db670487dda1368d84c61d693e5/src/core/Akka/Routing/RouterActor.cs
namespace MEAKKA
{
	/// <summary>
	/// Similar to <see cref="RouterActor"/> but handles forwarding to all routees, and routee list management,
	/// within the single actor. This ensures <see cref="AddRoutee"/> and <see cref="RemoveRoutee"/> messages are
	/// handled in deterministic order compared to <see cref="RouterActor"/>.
	/// </summary>
	public class DeterministicRouterActor : UntypedActor
	{
		private sealed class ActorEqualityComparer : EqualityComparer<IActorRef>
		{
			/// <inheritdoc />
			public override bool Equals(IActorRef x, IActorRef y)
			{
				return x?.Path == y?.Path;
			}

			public override int GetHashCode(IActorRef obj)
			{
				return obj.GetHashCode();
			}
		}

		/// <summary>
		/// Internal routing set.
		/// </summary>
		private HashSet<IActorRef> Routees { get; } = new(new ActorEqualityComparer());

		public DeterministicRouterActor()
		{

		}

		/// <summary>
		/// TBD
		/// </summary>
		/// <param name="message">TBD</param>
		protected override void OnReceive(object message)
		{
			switch(message)
			{
				case AddRoutee addRoutee:
					AddRoutee(addRoutee, Sender);
					break;
				case RemoveRoutee removeRoutee:
					RemoveRoutee(Sender);
					break;
				case Terminated terminated:
					RemoveRoutee(terminated.ActorRef);
					break;
				case BroadcastTargeted broadcast:
					if (Routees.Contains(broadcast.Target))
						broadcast.Target.Forward(broadcast.Message);
					break;
				case Broadcast broadcast:
					foreach (var routee in Routees)
						routee.Forward(broadcast.Message);
					break;
			}
		}

		private void RemoveRoutee(IActorRef sender)
		{
			Routees.Remove(sender);
		}

		private void AddRoutee(AddRoutee add, IActorRef sender)
		{
			Routees.Add(sender);
		}

		/// <summary>
		/// TBD
		/// </summary>
		/// <param name="cause">TBD</param>
		/// <param name="message">TBD</param>
		protected override void PreRestart(Exception cause, object message)
		{
			//do not scrap children
		}
	}
}