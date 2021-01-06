using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Akka.Actor;
using Akka.Util;

namespace MEAKKA
{
	/// <summary>
	/// Type-safe <see cref="IActorRef"/> references to a specified
	/// Entity Actor Type <typeparamref name="TActorType"/>.
	/// </summary>
	/// <typeparam name="TActorType"></typeparam>
	public interface IEntityActorRef<TActorType> : IActorRef
		where TActorType : IInternalActor
	{

	}

	/// <summary>
	/// Adapter for for Entity Actors for the <see cref="IActorRef"/> interface.
	/// Allowing for strongly typed actor references.
	/// </summary>
	/// <typeparam name="TActorType"></typeparam>
	public sealed class EntityActorGenericAdapter<TActorType> : IEntityActorRef<TActorType>, IActorRef
		where TActorType : IInternalActor
	{
		/// <summary>
		/// Adapted actor reference.
		/// </summary>
		private IActorRef Actor { get; }

		public EntityActorGenericAdapter(IActorRef actor)
		{
			Actor = actor ?? throw new ArgumentNullException(nameof(actor));
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Tell(object message, IActorRef sender)
		{
			Actor.Tell(message, sender);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(IActorRef other)
		{
			return Actor.Equals(other);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(IActorRef other)
		{
			return Actor.CompareTo(other);
		}

		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ISurrogate ToSurrogate(ActorSystem system)
		{
			return Actor.ToSurrogate(system);
		}

		//TODO: Examine nullable
		/// <inheritdoc />
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public int CompareTo(object obj)
		{
			return Actor.CompareTo(obj);
		}

		/// <inheritdoc />
		public ActorPath Path
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => Actor.Path;
		}
	}
}
