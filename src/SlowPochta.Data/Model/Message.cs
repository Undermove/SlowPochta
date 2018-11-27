using System;

namespace SlowPochta.Data.Model
{
	public class Message
	{
		public long Id { get; set; }

		public string MessageText { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime? DeliveryDate { get; set; }

		public DeliveryStatus Status { get; set; }

		public string StatusDescription { get; set; }
	}
}
