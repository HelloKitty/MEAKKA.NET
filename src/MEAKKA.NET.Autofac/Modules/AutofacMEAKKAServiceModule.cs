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
	/// Registers default Autofac MEAKKA service modules.
	/// </summary>
	public sealed class AutofacMEAKKAServiceModule : Autofac.Module
	{
		/// <inheritdoc />
		protected override void Load(ContainerBuilder builder)
		{
			base.Load(builder);

			//Prevent this from running multiple times.
			if(builder.Properties.ContainsKey(typeof(AutofacMEAKKAServiceModule).AssemblyQualifiedName))
				return;

			builder.Properties.Add(typeof(AutofacMEAKKAServiceModule).AssemblyQualifiedName, null);

			builder.RegisterType<AutoFacDependencyResolver>()
				.As<IDependencyResolver>()
				.InstancePerLifetimeScope();
		}
	}
}
