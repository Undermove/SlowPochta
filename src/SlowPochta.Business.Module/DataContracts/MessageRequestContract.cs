namespace SlowPochta.Business.Module.DataContracts
{
	public class MessageRequestContract
	{
		public string FromUser { get; set; }

		public string ToUser { get; set; }

		public string MessageText { get; set; }
	}
}
