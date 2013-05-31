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
			var isAssigned = new Variable<int>("Test", 0);
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
				, (dic) =>
				{
					Console.WriteLine("ok");
				});
		}

		[Test]
		public void Run_Async_Workflow()
		{
			var activity = new TestActivity();

			var parameters = new Dictionary<string, object>();
			parameters.Add("Text", "Hello world");

			var m = new System.Threading.ManualResetEvent(false);
			string result = null;
			CloudSoft.Workflows.WorkflowQueueUserWorkItem.RunAsync(
				activity,
				parameters,
				null,
				(dic) =>
				{
					result = dic["Text"].ToString();
				},
				null,
				() =>
				{
					m.Set();
				});

			m.WaitOne();

			Assert.AreEqual(result, "Hello Workflow !");
		}
	}
}
