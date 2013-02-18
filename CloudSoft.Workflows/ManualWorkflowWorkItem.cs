using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CloudSoft.Workflows
{
	public class ManualWorkflowWorkItem
	{
		public static void Run(System.Activities.Activity activity, Dictionary<string, object> parameters = null, Action completed = null, Action<Exception> failed = null, Action final = null)
		{
			var resetEvent = new ManualResetEvent(false);
			var pr = new ProgressReporter();
			var wi = new WorkItem(pr, resetEvent, activity, parameters, completed, failed, final);

			wi.Run();
		}

	}
}
