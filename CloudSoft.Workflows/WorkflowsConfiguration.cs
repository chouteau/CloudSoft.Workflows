using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.Workflows
{
	public class WorkflowsConfiguration
	{
		public IDependencyResolver DependencyResolver { get; set; }
		public ILogger Logger { get; set; }
	}
}
