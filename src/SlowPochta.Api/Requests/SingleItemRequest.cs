namespace SlowPochta.Api.Requests
{
	public class SingleItemRequest
	{
		public long? Id { get; set; }

		public int Skip { get; set; }

		public int Limit { get; set; }
	}
}
