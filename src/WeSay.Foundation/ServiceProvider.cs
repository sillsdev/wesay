using System;
using System.Collections.Generic;
using Autofac;
using System.Linq;
using Microsoft.Practices.ServiceLocation;

namespace Microsoft.Practices.ServiceLocation
{

	public interface IServiceLocator : IServiceProvider
	{

		object GetInstance(Type serviceType);

		object GetInstance(Type serviceType, string key);

		IEnumerable<object> GetAllInstances(Type serviceType);



		TService GetInstance<TService>();

		TService GetInstance<TService>(string key);

		IEnumerable<TService> GetAllInstances<TService>();

	}
}
namespace WeSay.Foundation
{
	/// <summary>
	/// From autofac google list, author Justin Rudd... looks like it might show up
	/// in a future version of autofac
	/// </summary>
	public class ServiceLocatorAdapter : IServiceLocator
	{
		readonly IContainer container;

		public ServiceLocatorAdapter(IContainer container)
		{
			if (container == null)
				throw new ArgumentNullException("container");

			this.container = container;
		}

		#region IServiceProvider methods

		public object GetService(Type serviceType)
		{
			return GetInstance(serviceType);
		}

		#endregion

		#region IServiceLocator methods

		public object GetInstance(Type serviceType)
		{
			return GetInstanceWhichWrapsExceptions(() => container.Resolve(serviceType));
		}

		public object GetInstance(Type serviceType, string key)
		{
			return
				string.IsNullOrEmpty(key)
					? GetInstanceWhichWrapsExceptions(() => container.Resolve(serviceType))
					: GetInstanceWhichWrapsExceptions(() => container.Resolve(key));
		}

		public TService GetInstance<TService>()
		{
			return GetInstanceWhichWrapsExceptions(() => container.Resolve<TService>());
		}

		public TService GetInstance<TService>(string key)
		{
			return GetInstanceWhichWrapsExceptions(() => container.Resolve<TService>(key));
		}

		public IEnumerable<object> GetAllInstances(Type serviceType)
		{
			// go through all the registrations and find TypedService instances that
			// equal the serviceType.
			var servicesToActivate
				= from reg in container.ComponentRegistrations
				  from svc in reg.Descriptor.Services.OfType<TypedService>()
				  where svc.ServiceType.Equals(serviceType)
				  select svc;

			// where we'll collect them....
			var result = new List<object>();

			// Then we create each one...
			foreach (var service in servicesToActivate)
				result.Add(GetInstanceWhichWrapsExceptions(() => container.Resolve(service)));

			return result;
		}

		public IEnumerable<TService> GetAllInstances<TService>()
		{
			foreach (var item in GetAllInstances(typeof(TService)))
				yield return (TService)item;
		}

		#endregion

		#region Implementation

		private T GetInstanceWhichWrapsExceptions<T>(Func<T> resolve)
		{
			try
			{
				return resolve();
			}
			catch (DependencyResolutionException ex)
			{
				throw new ActivationException(ex.Message, ex);
			}
			catch (ComponentNotRegisteredException ex)
			{
				throw new ActivationException(ex.Message, ex);
			}
		}

		#endregion
	}

	/// <summary>
	/// jh made up this implementation to fit the code above I don't know what the actual
	/// members are
	/// </summary>
	internal class ActivationException : Exception
	{
		public string Message { get; set; }
		public Exception OriginalException { get; set; }

		public ActivationException(string message, Exception ex)
		{
			Message = message;
			OriginalException = ex;
		}
	}
}
