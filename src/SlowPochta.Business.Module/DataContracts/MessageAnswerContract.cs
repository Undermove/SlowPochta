using System;
using System.Collections.Generic;
using System.Text;

namespace SlowPochta.Business.Module.DataContracts
{
	class MessageAnswerContract
	{
		public string FromUser { get; set; }

		public string ToUser { get; set; }

		public string MessageText { get; set; }

		public string StatusDescription { get; set; }
	}
}
