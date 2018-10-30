using System;
using System.Collections.Generic;
using System.Text;

namespace SlowPochta.Data.Model
{
	public class MessageToUser
	{
		public int Id { get; set; }

		public int MessageId { get; set; }

		public int UserId { get; set; }
	}
}
