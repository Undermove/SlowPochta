using System;
using System.Collections.Generic;
using SlowPochta.Data.Model;

namespace SlowPochta.Business.Module.DataContracts
{
	public class MessageAnswerContract
	{
		public long Id { get; set; }

		public string SenderLogin { get; set; }

		public string RecieverLogin { get; set; }

		public string MessageText { get; set; }

		public string LastStatusDescription { get; set; }

		public List<MessageDeliveryStatusVariant> PassedDeliveryStatuses { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime? DeliveryDate { get; set; }
	}
}
