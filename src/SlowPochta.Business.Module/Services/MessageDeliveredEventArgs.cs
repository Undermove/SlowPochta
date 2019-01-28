using System;

namespace SlowPochta.Business.Module.Services
{
	public class MessageDeliveredEventArgs : EventArgs 
	{
		public long MessageId { get; set; }
	}
}