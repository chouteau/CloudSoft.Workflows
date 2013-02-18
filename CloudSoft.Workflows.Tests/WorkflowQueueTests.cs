using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Activities.Statements;
using System.Activities;
using System.Activities.Expressions;

namespace CloudSoft.Workflows.Tests
{
	[TestFixture]
	public class WorkflowQueueTests
	{
		public WorkflowQueueTests()
		{

		}

		[Test]
		public void Run_Workflow()
		{
			Variable<int> isAssigned = new Variable<int>("Test", 0);
			var activity = new Sequence()
					{
						Variables = { isAssigned },
						Activities = {
							new Assign<int>()
							{
								To = ExpressionServices.ConvertReference<int>(ctx => isAssigned.Get(ctx)),
								Value = ExpressionServices.Convert<int>(ctx => 1)
							},
						},
					};

			CloudSoft.Workflows.ManualWorkflowWorkItem.Run(activity
				, null
				, () =>
				{
					Console.WriteLine("ok");
				});
		}
	}
}
