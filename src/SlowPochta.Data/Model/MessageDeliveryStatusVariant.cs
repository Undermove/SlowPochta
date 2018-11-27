using System.ComponentModel.DataAnnotations;

namespace SlowPochta.Data.Model
{
    public  class MessageDeliveryStatusVariant
    {
        public long Id { get; set; }

		[MaxLength(256)]
	    public string DeliveryStatusHeader { get; set; }

	    [MaxLength(2000)]
		public string DeliveryStatusDescription { get; set; }
    }
}
