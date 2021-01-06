using System;
using System.Collections.Generic;
using System.Text;

namespace MEAKKA
{
	/// <summary>
	/// Internal MEAKKA message type that attaches a disposable to an Entity Actor.
	/// </summary>
	internal sealed class AttachDisposableActorMessage : BaseInternalEntityActorMessage
	{
		/// <summary>
		/// The disposable to attach.
		/// </summary>
		public IDisposable Disposable { get; }

		public AttachDisposableActorMessage(IDisposable disposable)
		{
			Disposable = disposable ?? throw new ArgumentNullException(nameof(disposable));
		}
	}
}
