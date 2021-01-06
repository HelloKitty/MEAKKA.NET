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
		/// Dependency resolver for the actor creation.
		/// </summary>
		protected IDependencyResolver ActorResolver { get; }

		public AutofacActorFactory(ILifetimeScope container, ActorSystem system)
		{
			if(container == null) throw new ArgumentNullException(nameof(container));
			if(system == null) throw new ArgumentNullException(nameof(system));

			//TODO: We should invert control of the dependency resolver implementation
			ActorResolver = new AutoFacDependencyResolver(container, system);
		}

		/// <inheritdoc />
		public virtual IActorRef Create(ActorCreationContext context)
		{
			if (context == null) throw new ArgumentNullException(nameof(context));

			IActorRef actorRef = context.ActorReferenceFactory.ActorOf(ActorResolver.Create<TActorType>(), typeof(TActorType).Name);

			if(actorRef.IsNobody())
				throw new InvalidOperationException($"Failed to create Actor: {typeof(TActorType).Name}. Path: {actorRef.Path}");

			return actorRef;
		}
	}
}
