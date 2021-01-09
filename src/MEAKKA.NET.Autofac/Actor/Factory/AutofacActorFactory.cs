using System;
using System.Collections.Generic;
using System.Text;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using Booma;

namespace MEAKKA
{
	/// <summary>
	/// Autofac implementation of <see cref="IActorFactory{TActorType}"/>.
	/// </summary>
	/// <typeparam name="TActorType">The actor type to create.</typeparam>
	public class AutofacActorFactory<TActorType> : IActorFactory<TActorType> 
		where TActorType : ActorBase
	{
		/// <summary>
		/// The lifetimescope this factory was resolved with.
		/// </summary>
		protected ILifetimeScope Container { get; }

		/// <summary>
		/// The root actor system.
		/// </summary>
		protected ActorSystem System { get; }

		public AutofacActorFactory(ILifetimeScope container, ActorSystem system)
		{
			Container = container ?? throw new ArgumentNullException(nameof(container));
			System = system ?? throw new ArgumentNullException(nameof(system));
		}

		/// <inheritdoc />
		public virtual IEntityActorRef<TActorType> Create(ActorCreationContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			//Also on the Actor registeration we resolve the lifetimescope
			//and attach it so we don't need to send a dispose message.
			//Do not directly dispose this, needs to be disposed by the Actor stop.
			var lifetimeScope = Container.BeginLifetimeScope();
			Props props = new AutoFacDependencyResolver(lifetimeScope, System)
				.Create<TActorType>();

			//We cannot use type name, because it's not unique for Actor name
			//I also don't have a good way to seed in unique identifiers.
			//We must rely on Akka to produce a unique key.
			IActorRef actorRef = context.ActorReferenceFactory.ActorOf(props);

			if(actorRef.IsNobody())
				throw new InvalidOperationException($"Failed to create Actor: {typeof(TActorType).Name}. Path: {actorRef.Path}");

			//We adapt this to the type-containing actor type interface.
			return new EntityActorGenericAdapter<TActorType>(actorRef);
		}
	}
}
