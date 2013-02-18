using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.Workflows
{
	public interface IDependencyResolver
	{
		object GetService(Type serviceType);
		IEnumerable<object> GetServices(Type serviceType);
		IEnumerable<object> GetAllServices();
	}
}
