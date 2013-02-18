using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Globalization;

namespace CloudSoft.Workflows
{
	public class DependencyResolver
	{
		#region Nested Types

		private class DefaultDependencyResolver : IDependencyResolver
		{
			public object GetService(Type serviceType)
			{
				try
				{
					return Activator.CreateInstance(serviceType);
				}
				catch
				{
					return null;
				}
			}

			public IEnumerable<object> GetServices(Type serviceType)
			{
				return Enumerable.Empty<object>();
			}

			public IEnumerable<object> GetAllServices()
			{
				return null;
			}
		}

		private class DelegateBasedDependencyResolver : IDependencyResolver
		{
			Func<Type, object> _getService;
			Func<Type, IEnumerable<object>> _getServices;

			public DelegateBasedDependencyResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
			{
				_getService = getService;
				_getServices = getServices;
			}

			public object GetService(Type type)
			{
				try
				{
					return _getService.Invoke(type);
				}
				catch
				{
					return null;
				}
			}

			public IEnumerable<object> GetServices(Type type)
			{
				return _getServices(type);
			}

			public IEnumerable<object> GetAllServices()
			{
				return _getServices.Invoke(null);
			}
		}

		#endregion

		private static DependencyResolver _instance = new DependencyResolver();

		public static IDependencyResolver Current
		{
			get
			{
				return _instance.InnerCurrent;
			}
		}

		public static void SetResolver(IDependencyResolver resolver)
		{
			_instance.InnerSetResolver(resolver);
		}

		public static void SetResolver(object commonServiceLocator)
		{
			_instance.InnerSetResolver(commonServiceLocator);
		}

		public static void SetResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
		{
			_instance.InnerSetResolver(getService, getServices);
		}

		// Instance implementation (for testing purposes)

		private IDependencyResolver _current = new DefaultDependencyResolver();

		public IDependencyResolver InnerCurrent
		{
			get
			{
				return _current;
			}
		}

		public void InnerSetResolver(IDependencyResolver resolver)
		{
			if (resolver == null)
			{
				throw new ArgumentNullException("resolver");
			}

			_current = resolver;
		}

		public void InnerSetResolver(object commonServiceLocator)
		{
			if (commonServiceLocator == null)
			{
				throw new ArgumentNullException("commonServiceLocator");
			}

			var locatorType = commonServiceLocator.GetType();
			var getInstance = locatorType.GetMethod("GetInstance", new[] { typeof(Type) });
			var getInstances = locatorType.GetMethod("GetAllInstances", new[] { typeof(Type) });

			if (getInstance == null ||
				getInstance.ReturnType != typeof(object) ||
				getInstances == null ||
				getInstances.ReturnType != typeof(IEnumerable<object>))
			{
				throw new ArgumentException(
					String.Format(
						CultureInfo.CurrentCulture,
						"DependencyResolver_DoesNotImplementICommonServiceLocator",
						locatorType.FullName
					),
					"commonServiceLocator"
				);
			}

			var getService = (Func<Type, object>)Delegate.CreateDelegate(typeof(Func<Type, object>), commonServiceLocator, getInstance);
			var getServices = (Func<Type, IEnumerable<object>>)Delegate.CreateDelegate(typeof(Func<Type, IEnumerable<object>>), commonServiceLocator, getInstances);

			_current = new DelegateBasedDependencyResolver(getService, getServices);
		}

		public void InnerSetResolver(Func<Type, object> getService, Func<Type, IEnumerable<object>> getServices)
		{
			if (getService == null)
			{
				throw new ArgumentNullException("getService");
			}
			if (getServices == null)
			{
				throw new ArgumentNullException("getServices");
			}

			_current = new DelegateBasedDependencyResolver(getService, getServices);
		}
	}
}
