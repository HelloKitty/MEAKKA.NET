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
	public interface IEntityActorRef<TActorType>
		where TActorType : IInternalActor
	{
		//The reason we no longer use an adapter style is AKKA.NET does some hacky type introspection
		//which breaks if we adapt the actor ref.
		/// <summary>
		/// Adapted actor reference.
		/// </summary>
		IActorRef Actor { get; }
	}

	/// <summary>
	/// Adapter for for Entity Actors for the <see cref="IActorRef"/> interface.
	/// Allowing for strongly typed actor references.
	/// </summary>
	/// <typeparam name="TActorType"></typeparam>
	public sealed class EntityActorGenericAdapter<TActorType> : IEntityActorRef<TActorType>
		where TActorType : IInternalActor
	{
		/// <summary>
		/// Adapted actor reference.
		/// </summary>
		public IActorRef Actor
		{ 
			[MethodImpl(MethodImplOptions.AggressiveInlining)] 
			get;
		}

		public EntityActorGenericAdapter(IActorRef actor)
		{
			Actor = actor ?? throw new ArgumentNullException(nameof(actor));
		}
	}
}
