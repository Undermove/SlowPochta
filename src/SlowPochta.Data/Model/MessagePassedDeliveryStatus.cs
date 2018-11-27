namespace SlowPochta.Data.Model
{
    public class MessagePassedDeliveryStatus
    {
        public long Id { get; set; }
        public long MessageId { get; set; }
        public long DeliveryStatusVariantId { get; set; }
    }
}
