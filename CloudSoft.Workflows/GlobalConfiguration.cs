using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.Workflows
{
	public static class GlobalConfiguration
	{
		private static object m_Lock = new object();
		private static WorkflowsConfiguration m_Configuration;

		public static WorkflowsConfiguration Configuration
		{
			get
			{
				if (m_Configuration == null)
				{
					lock (m_Lock)
					{
						if (m_Configuration == null)
						{
							m_Configuration = new WorkflowsConfiguration();
							m_Configuration.DependencyResolver = new DefaultDependencyResolver();
							m_Configuration.Logger = new DiagnosticsLogger();
						}
					}
				}
				return m_Configuration;
			}
		}

	}
}
