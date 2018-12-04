using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;

namespace SlowPochta.Business.Module.Modules
{
	// свзяь многие ко многим реализована вручную, а не при помощи Entity в учебных целях
	public class MessageModule : IDisposable
	{
		private readonly DataContext _dataContext;

		public MessageModule(DesignTimeDbContextFactory contextFactory)
		{
			_dataContext = contextFactory.CreateDbContext(new string[] { });
		}

		public void Dispose()
		{
			_dataContext?.Dispose();
		}

		public event EventHandler<Message> MessageCreated;

		public async Task<bool> CreateMessage(MessageContract messageContract)
		{
		    if (messageContract == null)
		    {
		        return false;
		    }

			// check that sender presents in database
			var fromUser = await GetUserFromDb(messageContract.FromUser);
			if (fromUser == null)
			{
				return false;
			}

			// check that reciever presents in database
			var toUser = await GetUserFromDb(messageContract.ToUser);
			if (toUser == null)
			{
				return false;
			}

			// create new message in db
			EntityEntry<Message> newMessage = await _dataContext.Messages.AddAsync(new Message()
			{
				Status = DeliveryStatus.Created,
				CreationDate = DateTime.UtcNow,
				MessageText = messageContract.MessageText,
				StatusDescription = "In local postbox"
			});

			await _dataContext.SaveChangesAsync();

			// add one to many connections
			await _dataContext.MessagesFromUsers.AddAsync(new MessageFromUser()
			{
				MessageId = newMessage.Entity.Id,
				UserId = fromUser.Id
			});

			await _dataContext.MessagesToUsers.AddAsync(new MessageToUser()
			{
				MessageId = newMessage.Entity.Id,
				UserId = toUser.Id
			});

			await _dataContext.SaveChangesAsync();

			MessageCreated?.Invoke(this, newMessage.Entity);

			return true;
		}

        /// <summary>
        /// Method returns all messages that has been delivered to user
        /// </summary>
        /// <param name="userLogin">user that recieved messages</param>
        /// <returns>List of messages with status delivered</returns>
        public async Task<List<MessageAnswerContract>> GetDeliveredMessagesToUser(string userLogin)
		{
			// check that reciever presents in database
			var toUser = await GetUserFromDb(userLogin);
			if (toUser == null)
			{
				return new List<MessageAnswerContract>();
			}

			// find all messages IDs that sended to user
			List<long> messagesToUserIds = await _dataContext.MessagesToUsers
				.Where(messageToUser => messageToUser.UserId == toUser.Id)
				.Select(messageToUser => messageToUser.MessageId)
				.ToListAsync();

			// select all sended to user messages with status Delivered 
			var messages = await _dataContext.Messages
				.Where(message => messagesToUserIds.Contains(message.Id) &&
				                  message.Status == DeliveryStatus.Delivered)
				.ToListAsync();

			var messageAnswers = await ConvertRecievedMessagesToMessageAnswerContracts(userLogin, messages);

			return messageAnswers;
		}

		// Раньше метод выдавал нам сообщения. Теперь он выдает специальный класс,
		// который содержит чуть больше данных чем класс Message
		public async Task<List<MessageAnswerContract>> GetMessagesFromUser(string userLogin)
	    {
	        // check that sender presents in database
	        var fromUser = await GetUserFromDb(userLogin);
	        if (fromUser == null)
	        {
	            return new List<MessageAnswerContract>();
	        }

	        // find all messages IDs fromUser
	        List<long> messagesFromUserIds = await _dataContext.MessagesFromUsers
	            .Where(messageFromUser => messageFromUser.UserId == fromUser.Id)
	            .Select(messageFromUser => messageFromUser.MessageId)
	            .ToListAsync();

			// select all messages fromUser
			var messages = await _dataContext.Messages
	            .Where(message => messagesFromUserIds.Contains(message.Id))
	            .ToListAsync();

			var messageAnswers = await ConvertSentMessagesToMessageAnswerContracts(userLogin, messages);

		    return messageAnswers;
	    }

		private async Task<List<MessageAnswerContract>> ConvertSentMessagesToMessageAnswerContracts(string userLogin, List<Message> messages)
		{
			List<MessageAnswerContract> messageAnswers = new List<MessageAnswerContract>();

			foreach (var message in messages)
			{
				var recieverUsersLogins = await GetRecieversUsersLogins(message);

				var passed = await GetPassedDeliveryStatuses(message);

				// todo Сделать нормальные конструкторы для MAC
				messageAnswers.Add(new MessageAnswerContract()
				{
					Id = message.Id,
					RecieverLogin = recieverUsersLogins.Aggregate("", (current, element) => current + (element)),
					SenderLogin = userLogin,
					LastStatusDescription = message.StatusDescription,
					PassedDeliveryStatuses = passed,
					MessageText = message.MessageText,
					DeliveryDate = message.DeliveryDate,
					CreationDate = message.CreationDate
				});
			}

			return messageAnswers;
		}

		private async Task<List<string>> GetRecieversUsersLogins(Message message)
		{
			var toUsersIds = await _dataContext.MessagesToUsers
				.Where(mtu => mtu.MessageId == message.Id)
				.Select(user => user.UserId)
				.ToListAsync();

			var toUsersLogins = await _dataContext.Users
				.Where(user => toUsersIds.Contains(user.Id))
				.Select(user => user.Login).ToListAsync();
			return toUsersLogins;
		}

		private async Task<List<MessageAnswerContract>> ConvertRecievedMessagesToMessageAnswerContracts(string userLogin, List<Message> messages)
		{
			List<MessageAnswerContract> messageAnswers = new List<MessageAnswerContract>();

			foreach (var message in messages)
			{
				var senderUserLogin = await GetSenderUserLogin(message);

				var passed = await GetPassedDeliveryStatuses(message);

				messageAnswers.Add(new MessageAnswerContract()
				{
					Id = message.Id,
					RecieverLogin = userLogin,
					SenderLogin = senderUserLogin,
					LastStatusDescription = message.StatusDescription,
					PassedDeliveryStatuses = passed,
					MessageText = message.MessageText,
					DeliveryDate = message.DeliveryDate,
					CreationDate = message.CreationDate
				});
			}

			return messageAnswers;
		}

		private async Task<string> GetSenderUserLogin(Message message)
		{
			var fromUsersIds = await _dataContext.MessagesFromUsers
				.Where(mtu => mtu.MessageId == message.Id)
				.Select(user => user.UserId)
				.ToListAsync();

			var fromUsersLogins = await _dataContext.Users
				.Where(user => fromUsersIds.Contains(user.Id))
				.Select(user => user.Login).ToListAsync();
			return fromUsersLogins.FirstOrDefault();
		}

		private async Task<List<MessageDeliveryStatusContract>> GetPassedDeliveryStatuses(Message message)
		{
			List<MessagePassedDeliveryStatus> passedDeliveryStatuses = await _dataContext.MessagePassedDeliveryStatuses
				.Where(status => status.MessageId == message.Id).ToListAsync();

			List<MessageDeliveryStatusContract> passed = await _dataContext.MessageDeliveryStatusVariants
				.Join(
					passedDeliveryStatuses,
					variant => variant.Id,
					status => status.DeliveryStatusVariantId,
					(variant, status) => new MessageDeliveryStatusContract(variant, status) )
				.OrderBy(contract => contract.TransitionDateTime)
				.ToListAsync();
			return passed;
		}

	    public async Task<MessageAnswerContract> GetMessageById(long id, string requesterName)
        {
	        // check that messageId presents in database
            var msg = await GetMessageFromDb(id);
			if (msg == null)
	        {
                return new MessageAnswerContract();
	        }

		    var reciever = await GetRecieversUsersLogins(msg);
		    var sender = await GetSenderUserLogin(msg);

            if (sender != requesterName && !reciever.Contains(requesterName))
            {
                return new MessageAnswerContract();
            }

			var passed = await GetPassedDeliveryStatuses(msg);
			var msgContract = new MessageAnswerContract()
			{
				Id = msg.Id,
				PassedDeliveryStatuses = passed,
				CreationDate = msg.CreationDate,
				LastStatusDescription = msg.StatusDescription,
				DeliveryDate = msg.DeliveryDate,
				MessageText = msg.MessageText,
				RecieverLogin = reciever.FirstOrDefault(),
				SenderLogin = sender,
			};

			return msgContract;
	    }

	    private async Task<Message> GetMessageFromDb(long idNumber)
	    {
	        return await _dataContext.Messages.FirstOrDefaultAsync(id => id.Id.Equals(idNumber));
	    }

        private async Task<User> GetUserFromDb(string login)
		{
			return await _dataContext.Users.FirstOrDefaultAsync(person => person.Login.Equals(login));
		}
	}
}
