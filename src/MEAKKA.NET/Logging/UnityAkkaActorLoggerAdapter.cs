using System;
using System.Collections.Generic;
using System.Text;
using Akka.Event;
using Common.Logging;

namespace MEAKKA
{
	/// <summary>
	/// AKKA.NET <see cref="LoggingAdapterBase"/> implementation of a Logger around <see cref="ILog"/> implementation.
	/// </summary>
	public sealed class UnityAkkaActorLoggerAdapter : LoggingAdapterBase
	{
		/// <summary>
		/// Adapted Common.Logging <see cref="ILog"/> object.
		/// </summary>
		private ILog Logger { get; }

		public UnityAkkaActorLoggerAdapter(ILog logger) 
			: base(new DefaultLogMessageFormatter())
		{
			Logger = logger ?? throw new ArgumentNullException(nameof(logger));
		}

		/// <inheritdoc />
		protected override void NotifyError(object message)
		{
			Logger.Error(message);
		}

		/// <inheritdoc />
		protected override void NotifyError(Exception cause, object message)
		{
			Logger.Error(message, cause);
		}

		/// <inheritdoc />
		protected override void NotifyWarning(object message)
		{
			Logger.Warn(message);
		}

		/// <inheritdoc />
		protected override void NotifyWarning(Exception cause, object message)
		{
			Logger.Warn(message, cause);
		}

		/// <inheritdoc />
		protected override void NotifyInfo(object message)
		{
			Logger.Info(message);
		}

		/// <inheritdoc />
		protected override void NotifyInfo(Exception cause, object message)
		{
			Logger.Info(message, cause);
		}

		/// <inheritdoc />
		protected override void NotifyDebug(object message)
		{
			Logger.Debug(message);
		}

		/// <inheritdoc />
		protected override void NotifyDebug(Exception cause, object message)
		{
			Logger.Debug(message, cause);
		}

		/// <inheritdoc />
		public override bool IsDebugEnabled => Logger.IsDebugEnabled;

		/// <inheritdoc />
		public override bool IsInfoEnabled => Logger.IsInfoEnabled;

		/// <inheritdoc />
		public override bool IsWarningEnabled => Logger.IsWarnEnabled;

		/// <inheritdoc />
		public override bool IsErrorEnabled => Logger.IsErrorEnabled;
	}
}
