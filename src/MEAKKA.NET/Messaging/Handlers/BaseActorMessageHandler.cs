using System;
using System.Collections.Generic;
using System.Text;
using Glader.Essentials;
using MEAKKA;

namespace MEAKKA
{
	/// <summary>
	/// Base message handler for actors.
	/// </summary>
	/// <typeparam name="TMessageType"></typeparam>
	public abstract class BaseActorMessageHandler<TMessageType> : BaseSpecificMessageHandler<TMessageType, EntityActorMessage, EntityActorMessageContext> 
		where TMessageType : EntityActorMessage
	{
		//TODO: Move this to Glader Essentials handler API
		/// <summary>
		/// Indicates the message type this handles.
		/// </summary>
		internal static Type HandledMessageType => typeof(TMessageType);
	}
}
