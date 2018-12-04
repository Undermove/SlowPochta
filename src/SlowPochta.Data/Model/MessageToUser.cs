namespace SlowPochta.Data.Model
{
    /// <summary>
    /// Таблица, в которой указаны получатели
    /// </summary>
	public class MessageToUser
	{
		public long Id { get; set; }

		public long MessageId { get; set; }

		public long UserId { get; set; }
	}
}
