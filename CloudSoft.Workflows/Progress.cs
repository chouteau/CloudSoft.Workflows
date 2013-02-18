using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace CloudSoft.Workflows
{
	[DataContract]
	public class Progress
	{
		public Progress()
		{
			Id = Guid.NewGuid().ToString().Substring(0,10);
			CreationDate = DateTime.Now;
		}

		[DataMember]
		public string Id { get; set; }
		[DataMember]
		public DateTime CreationDate { get; set; }
		[DataMember]
		public int TotalCount { get; set; }
		[DataMember]
		public int PageIndex { get; set; }
		[DataMember]
		public int PageSize { get; set; }
		[DataMember]
		public string Message { get; set; }
	}
}
