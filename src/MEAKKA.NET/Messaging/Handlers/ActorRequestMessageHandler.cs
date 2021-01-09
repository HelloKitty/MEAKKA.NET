using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MEAKKA
{
	//TODO: Refactor into Glader.Essentials.MessageHandler
	/// <summary>
	/// Special <see cref="BaseActorMessageHandler{TMessageType}"/> that implements request/response type semantics.
	/// </summary>
	/// <typeparam name="TMessageRequestType">The request message type.</typeparam>
	/// <typeparam name="TMessageResponseType">Response message type.</typeparam>
	public abstract class ActorRequestMessageHandler<TMessageRequestType, TMessageResponseType> : BaseActorMessageHandler<TMessageRequestType>
		where TMessageRequestType : EntityActorMessage
	{
		/// <summary>
		/// Request/response message handler.
		/// </summary>
		protected ActorRequestMessageHandler()
		{

		}

		/// <inheritdoc />
		public override async Task HandleMessageAsync(EntityActorMessageContext context, TMessageRequestType message, CancellationToken token = default)
		{
			//Concept here is to dispatch to the request handler and get a response to send.
			TMessageResponseType response = await HandleRequestAsync(context, message, token);

			//TODO: Validate performance of ordering.
			//Support returning nothing, but ONLY when it's a reference type.
			//Value type responses like 0 or Enum0 or string empty should be considered valid message types.
			if (EqualityComparer<TMessageResponseType>.Default.Equals(response, default) && !typeof(TMessageResponseType).IsValueType)
				return;

			try
			{
				context.Sender.Tell(response, context.Entity);
			}
			finally
			{
				await OnResponseMessageSendAsync(context, message, response);
			}
		}

		/// <summary>
		/// Similar to <see cref="HandleMessageAsync"/> but requires the implementer return an instance of the specified <typeparamref name="TMessageResponseType"/>.
		/// Which will be sent over the network.
		/// </summary>
		/// <param name="context">Message context.</param>
		/// <param name="message">Incoming message.</param>
		/// <param name="token">Cancel token.</param>
		/// <returns></returns>
		protected abstract Task<TMessageResponseType> HandleRequestAsync(EntityActorMessageContext context, TMessageRequestType message, CancellationToken token = default);

		/// <summary>
		/// Implementer can override this method as a callback/event for when the <see cref="response"/> has been sent to the session.
		/// Called after <see cref="HandleMessageAsync"/>.
		/// 
		/// Implementers should not await directly within this message as it blocks the request pipeline unless they are absolutely sure they want this to happen.
		/// </summary>
		/// <param name="context">The message context.</param>
		/// <param name="request">The original request message.</param>
		/// <param name="response">The response message sent.</param>
		/// <returns></returns>
		protected virtual Task OnResponseMessageSendAsync(EntityActorMessageContext context, TMessageRequestType request, TMessageResponseType response)
		{
			return Task.CompletedTask;
		}
	}
}
