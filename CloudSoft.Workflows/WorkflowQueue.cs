using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace CloudSoft.Workflows
{
	internal class WorkflowQueue : IDisposable
	{
		private Queue<Action> m_Queue = new Queue<Action>();
		private ManualResetEvent m_NewWorkItem = new ManualResetEvent(false);
		private ManualResetEvent m_Terminate = new ManualResetEvent(false);
		private bool m_Terminated = false;
		private Thread m_Thread;

		private WorkflowQueue()
		{
			m_Thread = new Thread(new ThreadStart(ProcessQueue));
			m_Thread.Name = "WorkflowProcessQueue";
			m_Thread.IsBackground = true;
			m_Thread.Start();
		}

		private static WorkflowQueue m_Current;
		private static object m_Lock = new object();

		public static WorkflowQueue Current
		{
			get
			{
				if (m_Current == null)
				{
					lock (m_Lock)
					{
						if (m_Current == null)
						{
							m_Current = new WorkflowQueue();
						}
					}
				}
				return m_Current;
			}
		}

		void ProcessQueue()
		{
			while (!m_Terminated)
			{
				var waitHandles = new WaitHandle[] { m_Terminate, m_NewWorkItem };
				int result = ManualResetEvent.WaitAny(waitHandles, 60 * 1000, true);
				if (result == 0)
				{
					m_Terminated = true;
					break;
				}
				m_NewWorkItem.Reset();

				if (m_Queue.Count == 0)
				{
					continue;
				}
				// Enqueue
				Queue<Action> queueCopy;
				lock (m_Queue)
				{
					queueCopy = new Queue<Action>(m_Queue);
					m_Queue.Clear();
				}

				foreach (var item in queueCopy)
				{
					item();
				}
			}
		}

		public void RunAsync(WorkItem wi)
		{
			lock (m_Queue)
			{
				m_Queue.Enqueue(() => Run(wi));
			}
			m_NewWorkItem.Set();
		}

		public void Terminate()
		{
			m_Terminated = true;
			if (m_Terminate != null)
			{
				m_Terminate.Set();
			}
		}

		public void Dispose()
		{
			if (m_Terminate != null)
			{
				m_Terminate.Set();
			}
			m_Thread.Join();
		}

		private void Run(WorkItem wi)
		{
			try
			{
				wi.Run();
			}
			catch (Exception ex)
			{
				wi.Failed.Invoke(ex);
			}
		}
	}
}
