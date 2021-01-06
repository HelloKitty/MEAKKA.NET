using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Actor;
using Common.Logging;
using Glader.Essentials;

namespace MEAKKA
{
	/// <summary>
	/// Base Akka actor type for Entities in GladMMO.
	/// </summary>
	/// <typeparam name="TActorStateType"></typeparam>
	public abstract class BaseEntityActor<TActorStateType> : 
		ReceiveActor, 
		IEntityActorStateInitializable<TActorStateType>,
		IDisposableAttachable,
		ISingleDisposable
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

		/// <summary>
		/// The message handler service for the actor.
		/// </summary>
		protected IMessageHandlerService<EntityActorMessage, EntityActorMessageContext> MessageHandlerService { get; }

		/// <summary>
		/// Represents all the disposable dependencies of a <see cref="BaseEntityActor{TActorStateType}"/>.
		/// This will be disposed when the session is disposed.
		/// </summary>
		private List<IDisposable> InternalDisposables { get; } = new List<IDisposable>();

		/// <inheritdoc />
		public bool isDisposed { get; private set; } = false;

		protected BaseEntityActor(ILog logger, IMessageHandlerService<EntityActorMessage, EntityActorMessageContext> messageHandlerService)
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
			MessageHandlerService = messageHandlerService ?? throw new ArgumentNullException(nameof(messageHandlerService));

			ReceiveAsync<EntityActorMessage>(OnReceiveMessageAsync);
		}

		protected async Task OnReceiveMessageAsync(EntityActorMessage message)
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
			
			//TODO: Is it safe to capture the Context message ref forever??
			//TODO: Pool or cache somehow.
			EntityActorMessageContext context = new EntityActorMessageContext(Context);

			try
			{
				if(!await HandleMessageAsync(message, context))
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
		protected Task<bool> HandleMessageAsync(EntityActorMessage message, EntityActorMessageContext context)
		{
			if (message == null) throw new ArgumentNullException(nameof(message));
			if (context == null) throw new ArgumentNullException(nameof(context));

			return MessageHandlerService.HandleMessageAsync(context, message, CancellationToken.None);
		}

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

		[Obsolete]
		public static void InitializeActor(IActorRef actorReference, TActorStateType state)
		{
			if (actorReference == null) throw new ArgumentNullException(nameof(actorReference));
			if (state == null) throw new ArgumentNullException(nameof(state));

			actorReference.Tell(new EntityActorStateInitializeMessage<TActorStateType>(state));
		}

		/// <inheritdoc />
		public void AttachDisposable(IDisposable disposable)
		{
			if(disposable == null) throw new ArgumentNullException(nameof(disposable));

			lock(SyncObj)
			{
				if(isDisposed)
					throw new ObjectDisposedException($"Cannot attach {disposable.GetType().Name} as an attached disposable if the session is already disposed.");

				InternalDisposables.Add(disposable);
			}
		}

		//See: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
		/// <inheritdoc />
		public void Dispose()
		{
			lock(SyncObj)
			{
				if(isDisposed)
					return;

				try
				{
					//Foreach but make sure to guard against exceptions
					//caused by disposal because we need to dispose of ALL resources first or else we
					//may leak.
					Exception optionalException = null;
					foreach(var disposable in InternalDisposables)
						try
						{
							disposable.Dispose();
						}
						catch (Exception e)
						{
							if (Logger.IsErrorEnabled)
								Logger.Error($"Failed to Dispose of Actor Owned Resource: {disposable?.GetType()?.Name} Error: {e}");

							optionalException = e;
						}

					//We throw so we don't silently supress the error.
					if (optionalException != null)
						throw new InvalidOperationException($"Failed to dispose of all resources gracefully. See error log.", optionalException);
				}
				finally
				{
					isDisposed = true;

					// Suppress finalization.
					GC.SuppressFinalize(this);

					// Dispose of unmanaged resources.
					Dispose(true);
				}
			}
		}

		//See: https://docs.microsoft.com/en-us/dotnet/standard/garbage-collection/implementing-dispose
		/// <summary>
		/// Implements can additionally dispose of resources.
		/// This is called via <see cref="Dispose()"/> or the runtime finalizer.
		/// Implementers should always call the base.
		/// </summary>
		/// <param name="disposing">indicates whether the method call comes from a Dispose method (its value is true) or from a finalizer (its value is false).</param>
		protected virtual void Dispose(bool disposing)
		{

		}
	}
}
