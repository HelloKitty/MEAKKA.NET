using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Logging;

namespace MEAKKA
{
	/// <summary>
	/// Base Akka actor type for Entities in GladMMO.
	/// </summary>
	/// <typeparam name="TActorStateType"></typeparam>
	public abstract class BaseEntityActor<TActorStateType> : ReceiveActor, IEntityActor, IEntityActorStateInitializable<TActorStateType>
		where TActorStateType : class
	{
		/// <summary>
		/// Internal locking object.
		/// </summary>
		private readonly object SyncObj = new object();

		/// <summary>
		/// Potentially mutable state for the actor.
		/// </summary>
		protected TActorStateType ActorState { get; private set; }

		/// <summary>
		/// Internal Common.Logging actor logger. <see cref="ILog"/>
		/// </summary>
		protected ILog Logger { get; }

		/// <summary>
		/// Indicates if the actor is initialized.
		/// </summary>
		public bool isInitialized { get; private set; } = false;

		protected BaseEntityActor(ILog logger)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			ReceiveAsync<EntityActorMessage>(OnInternalReceiveMessageAsync);
		}

		protected async Task OnInternalReceiveMessageAsync(EntityActorMessage message)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));

			if(!isInitialized)
			{
				//Only 1 thread ever will call OnInternalReceiveMessageAsync but it prevents
				//any external initialization from happening.
				lock(SyncObj)
				{
					if(!isInitialized)
					{
						if(ExtractPotentialStateMessage(message, out var initMessage))
						{
							InitializeState(initMessage.State);

							//Send successful initialization message to the entity, immediately.
							//Some entities may not care.
							OnInitialized(new EntityActorInitializationSuccessMessage());
						}
						else
						{
							if(Logger.IsWarnEnabled)
								Logger.Warn($"{GetType().Name} encountered MessageType: {message.GetType().Name} before INITIALIZATION.");
						}

						//Even if we're initialized now, it's an init message we shouldn't continue with.
						return;
					}
				}
			}

			//TODO: Pool or cache somehow.
			EntityActorMessageContext context = new EntityActorMessageContext(Sender, Self, Context.System.Scheduler);

			try
			{
				if(!await OnReceiveMessageAsync(message, context))
					if(Logger.IsWarnEnabled)
						Logger.Warn($"EntityActor encountered unhandled MessageType: {message.GetType().Name}");
			}
			catch(Exception e)
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"Actor: {Self.Path.Address} failed to handle MessageType: {message.GetType().Name} without Exception: {e.Message}\n\nStack: {e.StackTrace}");
				throw;
			}
		}

		/// <summary>
		/// Implements must override this and implement domain-specific message handling logic.
		/// </summary>
		/// <param name="message">The message to handle.</param>
		/// <param name="context">The actor message context.</param>
		/// <returns>True if the message was successfully handled.</returns>
		protected abstract Task<bool> OnReceiveMessageAsync(EntityActorMessage message, EntityActorMessageContext context);

		/// <summary>
		/// Implementer can override the behavior of extracting the state from the provided initialization message.
		/// </summary>
		/// <param name="message"></param>
		/// <param name="entityActorStateInitializeMessage"></param>
		/// <returns></returns>
		protected virtual bool ExtractPotentialStateMessage(EntityActorMessage message, out EntityActorStateInitializeMessage<TActorStateType> entityActorStateInitializeMessage)
		{
			if (message is EntityActorStateInitializeMessage<TActorStateType> initMessage)
			{
				entityActorStateInitializeMessage = initMessage;
				return true;
			}

			entityActorStateInitializeMessage = null;
			return false;
		}

		/// <summary>
		/// Implementer can override this to preform post-initialization logic.
		/// </summary>
		/// <param name="successMessage">The success entity initialization message.</param>
		protected virtual void OnInitialized(EntityActorInitializationSuccessMessage successMessage)
		{

		}

		//This is never called publicly, but I used an interface because I didn't fully grasp Akka when I wrote this.
		public void InitializeState(TActorStateType state)
		{
			lock (SyncObj)
			{
				if(isInitialized)
					throw new InvalidOperationException($"Cannot initialize actor: {GetType().Name} more than once.");

				ActorState = state;
				isInitialized = true;
			}
		}

		/// <summary>
		/// Default's to <see cref="OneForOneStrategy"/> which stops the Actor on exception.
		/// </summary>
		/// <returns></returns>
		protected override SupervisorStrategy SupervisorStrategy()
		{
			return new OneForOneStrategy(0, -1, exception =>
			{
				if(Logger.IsErrorEnabled)
					Logger.Error($"{GetType().Name} Exception ACTOR STOP: {exception.Message}\n\nStack: {exception.StackTrace}");

				return Directive.Stop;
			});
		}

		public static void InitializeActor(IActorRef actorReference, TActorStateType state)
		{
			if (actorReference == null) throw new ArgumentNullException(nameof(actorReference));
			if (state == null) throw new ArgumentNullException(nameof(state));

			actorReference.Tell(new EntityActorStateInitializeMessage<TActorStateType>(state));
		}
	}
}
