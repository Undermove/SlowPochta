using System;
using System.Collections.Generic;
using System.Text;

namespace SlowPochta.Data.Model
{
	public class Message
	{
		public int Id { get; set; }

		public string MessageText { get; set; }

		public DateTime CreationDate { get; set; }

		public DeliveryStatus Status { get; set; }

		public string StatusDescription { get; set; }
	}
}
