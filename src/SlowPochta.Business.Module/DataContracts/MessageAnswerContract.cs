using System;
using System.Collections.Generic;
using System.Text;

namespace SlowPochta.Business.Module.DataContracts
{
	public class MessageAnswerContract
	{
		public string FromUser { get; set; }

		public string ToUser { get; set; }

		public string MessageText { get; set; }

		public string StatusDescription { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime? DeliveryDate { get; set; }
	}
}
