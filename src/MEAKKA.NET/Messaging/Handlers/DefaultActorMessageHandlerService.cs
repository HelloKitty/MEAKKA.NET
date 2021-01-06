using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Akka.Util;
using Glader.Essentials;
using MEAKKA;

namespace Booma
{
	/// <summary>
	/// Default actor message handling strategy.
	/// Routes the messages based on <see cref="Type"/> to multiple potential <see cref="IMessageHandler{TMessageType,TMessageContext}"/>s.
	/// </summary>
	/// <typeparam name="TActorType">The actor type this handler service is for. (Mostly for hinting and future use)</typeparam>
	public sealed class DefaultActorMessageHandlerService<TActorType> : 
		IMessageHandlerService<EntityActorMessage, EntityActorMessageContext>, 
		ITypeBinder<IMessageHandler<EntityActorMessage, EntityActorMessageContext>, EntityActorMessage>
	{
		/// <summary>
		/// Internal private routing map between message type and handler/listeners.
		/// </summary>
		private Dictionary<Type, List<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>> MessageHandlerMap { get; }

		public DefaultActorMessageHandlerService(IEnumerable<IMessageHandler<EntityActorMessage, EntityActorMessageContext>> handlers)
		{
			if (handlers == null) throw new ArgumentNullException(nameof(handlers));

			//TODO: better default size.
			MessageHandlerMap = new Dictionary<Type, List<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>>(30);

			//Bind every handler to this handler service.
			foreach(var handler in handlers)
				handler.BindTo(this);
		}

		/// <inheritdoc />
		public async Task<bool> HandleMessageAsync(EntityActorMessageContext context, EntityActorMessage message, CancellationToken token = default)
		{
			//We don't lock here even though dictionary is publicly mutable
			//But we discourage calling it.
			if (!MessageHandlerMap.ContainsKey(message.GetType()))
				return false;

			foreach (var handler in MessageHandlerMap[message.GetType()])
				await handler.HandleMessageAsync(context, message, token);

			return true;
		}

		//Explictly implement since nothing should really call this externally.
		/// <summary>
		/// Do not call this externally. It's not thread-safe.
		/// </summary>
		/// <typeparam name="TBindType"></typeparam>
		/// <param name="bindable"></param>
		/// <returns></returns>
		bool ITypeBinder<IMessageHandler<EntityActorMessage, EntityActorMessageContext>, EntityActorMessage>.Bind<TBindType>(IMessageHandler<EntityActorMessage, EntityActorMessageContext> bindable)
		{
			if (bindable == null) throw new ArgumentNullException(nameof(bindable));

			if (MessageHandlerMap.ContainsKey(typeof(TBindType)))
				MessageHandlerMap[typeof(TBindType)].Add(bindable);
			else
				MessageHandlerMap[typeof(TBindType)] = new List<IMessageHandler<EntityActorMessage, EntityActorMessageContext>>(4) {bindable};

			return true;
		}
	}
}
