using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Activities;

namespace CloudSoft.Workflows
{
	public static class WorkflowQueueUserWorkItem
	{
		public static void RunAsync(System.Activities.Activity activity, Dictionary<string, object> parameters = null, ProgressReporter pr = null, Action<Dictionary<string, object>> completed = null, Action<Exception> failed = null, Action final = null, Action aborted = null)
		{
			var resetEvent = new ManualResetEvent(false);
			if (pr == null)
			{
				pr = new ProgressReporter();
				ProgressReporterCollector.Current.Add(pr);
			}

			var existing = ProgressReporterCollector.Current[pr.Id];
			if (existing != null
				&& pr.StartDate.HasValue)
			{
				// TODO : Throw exception
				return;
			}

			var wi = new WorkItem(pr, resetEvent, activity, parameters, completed, failed, final, aborted);

			WorkflowQueue.Current.RunAsync(wi);
		}
	}
}
