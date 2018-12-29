using System;
using System.Collections.Generic;
using SlowPochta.Data.Model;

namespace SlowPochta.Business.Module.DataContracts
{
	public class MessageAnswerContract
	{
		public MessageAnswerContract(){}

		public MessageAnswerContract(
			Message msg, 
			List<MessageDeliveryStatusContract> passedStatuses,
			string recieverLogin,
			string senderLogin)
		{
			Id = msg.Id;
			SenderLogin = senderLogin;
			RecieverLogin = recieverLogin;
			MessageText = msg.MessageText;
			LastStatusDescription = msg.StatusDescription;
			PassedDeliveryStatuses = passedStatuses;
			CreationDate = msg.CreationDate;
			LastUpdateTime = msg.LastUpdateTime;
			IsRead = msg.IsRead;
		}

		public long Id { get; set; }

		public string SenderLogin { get; set; }

		public string RecieverLogin { get; set; }

		public string MessageText { get; set; }

		public string LastStatusDescription { get; set; }

		public bool IsRead { get; set; }

		public List<MessageDeliveryStatusContract> PassedDeliveryStatuses { get; set; }

		public DateTime CreationDate { get; set; }

		public DateTime? LastUpdateTime { get; set; }
	}
}
