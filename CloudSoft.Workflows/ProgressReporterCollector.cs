using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CloudSoft.Workflows
{
	public class ProgressReporterCollector
	{
		private static object m_Lock = new object();
		private SynchronizedCollection<ProgressReporter> m_ProgressReporterList;
		private System.Timers.Timer m_Timer;

		private ProgressReporterCollector()
		{
			m_ProgressReporterList = new SynchronizedCollection<ProgressReporter>();
			m_Timer = new System.Timers.Timer();
			m_Timer.Interval = 1000 * 60; // Toutes les minutes
			m_Timer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerElapsed);
		}

		private static ProgressReporterCollector m_Current;
		internal static ProgressReporterCollector Current
		{
			get
			{
				if (m_Current == null)
				{
					lock (m_Lock)
					{
						if (m_Current == null)
						{
							m_Current = new ProgressReporterCollector();
						}
					}
				}
				return m_Current;
			}
		}

		public ProgressReporter this[string id]
		{
			get
			{
				return m_ProgressReporterList.SingleOrDefault(i => i.Id == id);
			}
		}

		public void Add(ProgressReporter pr)
		{
			lock (m_ProgressReporterList.SyncRoot)
			{
				if (m_ProgressReporterList.Any(i => i.Id == pr.Id))
				{
					return;
				}
				pr.Finish += new EventHandler(OnProgressReporterFinished);
				m_ProgressReporterList.Add(pr);
			}
			if (!m_Timer.Enabled)
			{
				m_Timer.Start();
			}
		}

		void OnTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
		{
			lock (m_ProgressReporterList.SyncRoot)
			{
				while (true)
				{
					// Wait 1 minute before remove
					var item = m_ProgressReporterList.FirstOrDefault(i => i.TerminatedDate.HasValue 
						&& i.TerminatedDate.Value.AddMinutes(1) <= DateTime.Now);
					if (item == null)
					{
						break;
					}
					Remove(item);
				}
				if (m_ProgressReporterList.Count == 0)
				{
					m_Timer.Stop();
				}
			}
		}

		void Remove(ProgressReporter pr)
		{
			lock (m_ProgressReporterList.SyncRoot)
			{
				m_ProgressReporterList.Remove(pr);
				pr.Finish -= new EventHandler(OnProgressReporterFinished);
				pr.Dispose();
			}
		}

		void OnProgressReporterFinished(object sender, EventArgs e)
		{
			var pr = sender as ProgressReporter;
			if (pr != null)
			{
				// TODO : Dispose after 1 minute
			}
		}

		public static IEnumerable<ProgressReporter> GetList()
		{
			return Current.m_ProgressReporterList;
		}

		public static ProgressReporter GetNew()
		{
			var pr = new ProgressReporter();
			Current.Add(pr);
			return pr;
		}

		public static ProgressReporter Get(string id)
		{
			var result = Current[id];
			if (result != null
				&& result.TerminatedDate.HasValue)
			{
				Current.Remove(result);
				return null;
			}
			return result;
		}
	}
}
