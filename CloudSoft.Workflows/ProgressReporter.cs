using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CloudSoft.Workflows
{

	[DataContract]
	public class ProgressReporter : IDisposable
	{
		// Todo : Make internal
		public ProgressReporter()
		{
			Id = Guid.NewGuid().ToString();
			History = new List<string>();
			ProgressList = new List<Progress>();
			CreationDate = DateTime.Now;
			CancelRequested = false;
		}

		public event EventHandler Start;
		internal void OnStart()
		{
			if (Start != null)
			{
				Start(this, EventArgs.Empty);
			}
		}

		public event EventHandler Finish;
		internal void OnFinish()
		{
			if (Finish != null)
			{
				Finish(this, EventArgs.Empty);
			}
		}

		public event EventHandler<EventArgs<string>> Message;
		internal void OnMessage(string message)
		{
			if (Message != null)
			{
				Message(this, new EventArgs<string>(message));
			}
		}

		public event EventHandler<EventArgs<Progress>> PagedProgress;
		internal void OnPagedProgress(Progress progress)
		{
			if (PagedProgress != null)
			{
				PagedProgress(this, new EventArgs<Progress>(progress));
			}
		}

		public event EventHandler<EventArgs<object>> ProgressChanged;
		internal void OnProgressChanged(object data)
		{
			if (ProgressChanged != null)
			{
				ProgressChanged(this, new EventArgs<object>(data));
			}
		}

		public event EventHandler<EventArgs<Exception>> Exception;
		internal void OnException(Exception ex)
		{
			if (Exception != null)
			{
				Exception(this, new EventArgs<Exception>(ex));
			}
		}

		[DataMember]
		public DateTime CreationDate { get; set; }
		[DataMember]
		public string Id { get; set; }
		[DataMember]
		public DateTime? StartDate { get; set; }
		[DataMember]
		public DateTime? TerminatedDate { get; set; }
		[DataMember]
		public List<string> History { get; set; }
		[DataMember]
		public string ErrorStack { get; set; }
		[DataMember]
		public List<Progress> ProgressList { get; set; }

		public bool CancelRequested { get; set; }

		#region IDisposable Members

		public void Dispose()
		{
			this.Finish = null;
			this.Message = null;
			this.PagedProgress = null;
			this.ProgressChanged = null;
			ProgressList.Clear();
			History.Clear();
		}

		#endregion
	}
}
