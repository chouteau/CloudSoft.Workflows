using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;

namespace CloudSoft.Workflows
{
	public static class WorkflowExtensions
	{
		public static TService GetService<TService>(this IDependencyResolver resolver)
		{
			return (TService)resolver.GetService(typeof(TService));
		}

		public static IEnumerable<TService> GetServices<TService>(this IDependencyResolver resolver)
		{
			return resolver.GetServices(typeof(TService)).Cast<TService>();
		}

		public static T GetService<T>(this ActivityContext context)
			where T : class 
		{
			return GlobalConfiguration.Configuration.DependencyResolver.GetService<T>();
		}

		public static IEnumerable<T> GetServices<T>(this ActivityContext context)
			where T : class
		{
			return GlobalConfiguration.Configuration.DependencyResolver.GetServices<T>();
		}

		public static void ReportProgress(this ActivityContext context, int totalCount, int pageIndex, int pageSize, string message = null)
		{
			var pr = context.GetExtension<ProgressReporter>();
			if (pr != null)
			{
				var progress = pr.ProgressList.LastOrDefault();
				if (progress == null
					|| progress.TotalCount != totalCount)
				{
					progress = new Progress();
					pr.ProgressList.Add(progress);
				}
				progress.Message = message;
				progress.TotalCount = totalCount;
				progress.PageIndex = pageIndex;
				progress.PageSize = pageSize;
				pr.OnPagedProgress(progress);
			}
		}

		public static void ReportProgress(this ActivityContext context, string message)
		{
			var pr = context.GetExtension<ProgressReporter>();
			if (pr != null)
			{
				pr.History.Add(message);
				pr.OnMessage(message);
			}
		}

		public static void ProgressChanged(this ActivityContext context, object data)
		{
			var pr = context.GetExtension<ProgressReporter>();
			if (pr != null)
			{
				pr.OnProgressChanged(data);
			}
		}

		public static void ReportException(this ActivityContext context, Exception ex)
		{
			var pr = context.GetExtension<ProgressReporter>();
			if (pr == null)
			{
				return;
			}
			pr.ErrorStack = ex.ToString();
		}

		public static bool IsCancelRequested(this ActivityContext context)
		{
			var pr = context.GetExtension<ProgressReporter>();
			if (pr != null)
			{
				return pr.CancelRequested;
			}
			return false;
		}
	}
}
