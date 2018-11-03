using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;

namespace SlowPochta.Business.Module
{
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

			return true;
		}

		/// <summary>
		/// Method returns all messages that has been delivered to user
		/// </summary>
		/// <param name="userLogin">user that recieved messages</param>
		/// <returns>List of messages with status delivered</returns>
		public async Task<List<Message>> GetDeliveredMessagesToUser(string userLogin)
		{
			// check that reciever presents in database
			var toUser = await GetUserFromDb(userLogin);
			if (toUser == null)
			{
				return new List<Message>();
			}

			// find all messages IDs that sended to user
			List<int> messagesToUserIds = await _dataContext.MessagesToUsers
				.Where(messageToUser => messageToUser.UserId == toUser.Id)
				.Select(messageToUser => messageToUser.MessageId)
				.ToListAsync();

			// select all sended to user messages with status Delivered 
			var messages = await _dataContext.Messages
				.Where(message => messagesToUserIds.Contains(message.Id) &&
				                  message.Status == DeliveryStatus.Delivered)
				.ToListAsync();

			return messages;
		}

		private async Task<User> GetUserFromDb(string login)
		{
			return await _dataContext.Users.FirstOrDefaultAsync(person => person.Login.Equals(login));
		}
	}
}
