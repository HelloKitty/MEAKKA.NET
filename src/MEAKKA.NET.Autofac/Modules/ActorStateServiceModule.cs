using System;
using System.Collections.Generic;
using System.Text;
using Autofac;

namespace MEAKKA
{
	/// <summary>
	/// Service module for <see cref="IActorState{T}"/> and <see cref="IInternalActorState{T}"/> implementations.
	/// Registers them generically.
	/// </summary>
	public sealed class ActorStateServiceModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//This is the actor state storage objects.
			builder.RegisterGeneric(typeof(MutableGenericActorState<>))
				.As(typeof(IActorState<>))
				.As(typeof(IMutableActorState<>))
				.InstancePerLifetimeScope();

			//This is the actor state storage objects for internal usage only.
			builder.RegisterGeneric(typeof(MutableGenericActorState<>))
				.As(typeof(IInternalActorState<>))
				.As(typeof(IInternalMutableActorState<>))
				.InstancePerLifetimeScope();
		}
	}
}
