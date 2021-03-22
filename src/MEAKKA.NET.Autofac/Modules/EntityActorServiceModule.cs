using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Akka.Actor;
using Autofac;
using Autofac.Core;
using Booma;
using Glader.Essentials;

namespace MEAKKA
{
	/// <summary>
	/// Service module that registers <see cref="AutofacMEAKKAServiceModule"/>
	/// and <see cref="IActorFactory{TActorType}"/> for the specified actor type.
	/// It also registers the <typeparamref name="TEntityActorType"/> as well as
	/// the <see cref="IMessageHandlerService{TMessageType,TMessageContext}"/> for the actor.
	/// </summary>
	public class EntityActorServiceModule<TEntityActorType> : Autofac.Module 
		where TEntityActorType : ActorBase, IDisposableAttachable
	{
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Prevent this from running multiple times.
			if(builder.Properties.ContainsKey(GetType().AssemblyQualifiedName))
				return;

			builder.Properties.Add(GetType().AssemblyQualifiedName, null);

			//Default MEAKKA autofac module.
			builder.RegisterModule<AutofacMEAKKAServiceModule>();

			//Must be instance per lifetime scope since it depends
			//on the lifetimescope for resolving actor dependencies.
			builder.RegisterType<AutofacActorFactory<TEntityActorType>>()
				.As<IActorFactory<TEntityActorType>>()
				.InstancePerLifetimeScope();

			RegisterActorType<TEntityActorType>(builder);
		}

		private void RegisterActorType<TActorType>(ContainerBuilder builder)
			where TActorType : ActorBase, IDisposableAttachable
		{
			if (builder == null) throw new ArgumentNullException(nameof(builder));

			//Lifetimescope registeration is ok here because
			//the idea is we SHOULD own the lifetime scope and have a new one made EVERY TIME
			//the actor is made.
			builder.RegisterType<TActorType>()
				.AsSelf()
				.OnActivated(a =>
				{
					//TODO: Does this actually work??
					a.Instance.AttachDisposable(a.Context.Resolve<ILifetimeScope>());
				})
				.ExternallyOwned() //owned by Akka
				.InstancePerLifetimeScope();

			builder.Register(context =>
				{
					var handlers = context.ResolveNamed<IEnumerable<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>>(typeof(TActorType).Name);
					return new DefaultActorMessageHandlerService<TActorType>(handlers);
				})
				.As<DefaultActorMessageHandlerService<TActorType>>()
				.Named<IMessageHandlerService<EntityActorMessage, EntityActorMessageContext>>(typeof(TActorType).Name)
				.InstancePerLifetimeScope();

			foreach(var handler in GetSearchableTypes<TActorType>()
				.Where(t => t.IsAssignableTo<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>())
				.Where(t => t.GetCustomAttributes<ActorMessageHandlerAttribute>().Any(a => a.ActorType == typeof(TActorType))))
			{
				//No longer sharing handlers anymore.
				builder.RegisterType(handler)
					.Named<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>(typeof(TActorType).Name)
					.InstancePerLifetimeScope();

				//Now we parse the handler to find any IActorState properties we can register handlers for.
				foreach (var prop in handler.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
				{
					//This checks if a handler's fields are an actor state type
					//and if they are we'll auto-generate and register a state initializer handler for it
					//so it can be externally initialized.
					if (IsActorStateProperty<TActorType>(prop))
					{
						builder.RegisterType(typeof(InitializeStateMessageHandler<>).MakeGenericType(prop.PropertyType.GenericTypeArguments.First()))
							.Named<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>(typeof(TActorType).Name)
							.InstancePerLifetimeScope();
					}
				}
			}
		}

		private static bool IsActorStateProperty<TActorType>(PropertyInfo prop) 
			where TActorType : ActorBase, IDisposableAttachable
		{
			return prop.PropertyType.IsGenericType && (prop.PropertyType.GetGenericTypeDefinition() == typeof(IActorState<>) || prop.PropertyType.GetGenericTypeDefinition() == typeof(IMutableActorState<>));
		}

		/// <summary>
		/// Implementer should a collection of types to be searched
		/// for <see cref="IMessageHandler{TMessageType,TMessageContext}"/>s.
		/// Can be any collection of types, they will be filtered.
		///
		/// Default will search the assembly the <typeparamref name="TEntityActorType"/> is contained in.
		/// </summary>
		/// <typeparam name="TActorType">The actor type.</typeparam>
		/// <returns></returns>
		protected virtual IEnumerable<Type> GetSearchableTypes<TActorType>() 
			where TActorType : ActorBase, IDisposableAttachable
		{
			return typeof(TActorType)
				.Assembly
				.GetTypes();
		}
	}
}
