using System;
using System.Collections.Generic;
using System.Text;

namespace SlowPochta.Business.Module.DataContracts
{
	public class MessageContract
	{
		public string FromUser { get; set; }

		public string ToUser { get; set; }

		public string MessageText { get; set; }
	}
}
