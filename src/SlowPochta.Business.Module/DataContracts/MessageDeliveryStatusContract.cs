using System;
using SlowPochta.Data.Model;

namespace SlowPochta.Business.Module.DataContracts
{
	public class MessageDeliveryStatusContract
	{
		public MessageDeliveryStatusContract(
			MessageDeliveryStatusVariant variant, 
			MessagePassedDeliveryStatus passedDeliveryStatus)
		{
			DeliveryStatusVariantId = variant.Id;
			DeliveryStatusDescription = variant.DeliveryStatusDescription;
			DeliveryStatusHeader = variant.DeliveryStatusHeader;
			TransitionDateTime = passedDeliveryStatus.TransitionDateTime;
		}

		public long DeliveryStatusVariantId { get; }

		public string DeliveryStatusHeader { get; }

		public string DeliveryStatusDescription { get; }

		public DateTime TransitionDateTime { get; }
	}
}
