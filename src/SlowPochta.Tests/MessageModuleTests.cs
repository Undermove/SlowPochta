using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SlowPochta.Business.Module;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;
using Xunit;

namespace SlowPochta.Tests
{
    public class MessageModuleTests : IDisposable
    {
        private readonly MessageModule _messageModule;
        private readonly DataContext _dataContext;

        public MessageModuleTests()
        {
            var contextFactory = new DesignTimeDbContextFactory();
            _dataContext = contextFactory.CreateDbContext(new string[] { });
            _dataContext.Database.Migrate();
            _messageModule = new MessageModule(contextFactory);
        }

        public void Dispose()
        {
            _messageModule.Dispose();
            _dataContext.Database.EnsureDeleted();
            _dataContext.Dispose();
        }

        [Fact]
        public async void CreateMessageSuccessTest()
        {
            // arrange
            MessageContract testMessageContract = new MessageContract()
            {
                FromUser = "sender",
                ToUser = "recipient",
                MessageText = "textMessage"
            };

            User sender = _dataContext.Users.Add(new User()
            {
                Login = "sender",
                Password = "test",
                Role = RoleTypes.User
            }).Entity;

            User recipient = _dataContext.Users.Add(new User()
            {
                Login = "recipient",
                Password = "test",
                Role = RoleTypes.User
            }).Entity;

            _dataContext.SaveChanges();

            // act
            bool result = await _messageModule.CreateMessage(testMessageContract);

            // assert
            Assert.True(result);

            var message =
                _dataContext.Messages
                    .FirstOrDefault(msg => msg.MessageText == testMessageContract.MessageText);
            Assert.NotNull(message);
            Assert.Equal(1, _dataContext.Messages.Count());

            var messageFromUser = _dataContext.MessagesFromUsers
                .FirstOrDefault(mfu => mfu.UserId == sender.Id);
            Assert.NotNull(messageFromUser);
            Assert.Equal(1, _dataContext.MessagesFromUsers.Count());

            var messageToUser = _dataContext.MessagesToUsers
                .FirstOrDefault(mtu => mtu.UserId == recipient.Id);
            Assert.NotNull(messageToUser);
            Assert.Equal(1, _dataContext.MessagesToUsers.Count());
        }

        [Fact]
        public async void CreateMessageForAbsentRecipientTest()
        {
            // arrange
            MessageContract testMessageContract = new MessageContract()
            {
                FromUser = "sender",
                ToUser = "badRecipient",
                MessageText = "textMessage"
            };

            User sender = _dataContext.Users.Add(new User()
            {
                Login = "sender",
                Password = "test",
                Role = RoleTypes.User
            }).Entity;

            User recipient = _dataContext.Users.Add(new User()
            {
                Login = "recipient",
                Password = "test",
                Role = RoleTypes.User
            }).Entity;

            _dataContext.SaveChanges();

            // act
            bool result = await _messageModule.CreateMessage(testMessageContract);

            // assert
            Assert.False(result);

            var message =
                _dataContext.Messages
                    .FirstOrDefault(msg => msg.MessageText == testMessageContract.MessageText);
            Assert.Null(message);
            Assert.Equal(0, _dataContext.Messages.Count());

            var messageFromUser = _dataContext.MessagesFromUsers
                .FirstOrDefault(mfu => mfu.UserId == sender.Id);
            Assert.Null(messageFromUser);
            Assert.Equal(0, _dataContext.MessagesFromUsers.Count());

            var messageToUser = _dataContext.MessagesToUsers
                .FirstOrDefault(mtu => mtu.UserId == recipient.Id);
            Assert.Null(messageToUser);
            Assert.Equal(0, _dataContext.MessagesToUsers.Count());
        }

        [Fact]
        public async void CreateMessageForAbsentSenderTest()
        {
            // arrange
            MessageContract testMessageContract = new MessageContract()
            {
                FromUser = "badSender",
                ToUser = "recipient",
                MessageText = "textMessage"
            };

            User sender = _dataContext.Users.Add(new User()
            {
                Login = "sender",
                Password = "test",
                Role = RoleTypes.User
            }).Entity;

            User recipient = _dataContext.Users.Add(new User()
            {
                Login = "recipient",
                Password = "test",
                Role = RoleTypes.User
            }).Entity;

            _dataContext.SaveChanges();

            // act
            bool result = await _messageModule.CreateMessage(testMessageContract);

            // assert
            Assert.False(result);

            var message =
                _dataContext.Messages
                    .FirstOrDefault(msg => msg.MessageText == testMessageContract.MessageText);
            Assert.Null(message);
            Assert.Equal(0, _dataContext.Messages.Count());

            var messageFromUser = _dataContext.MessagesFromUsers
                .FirstOrDefault(mfu => mfu.UserId == sender.Id);
            Assert.Null(messageFromUser);
            Assert.Equal(0, _dataContext.MessagesFromUsers.Count());

            var messageToUser = _dataContext.MessagesToUsers
                .FirstOrDefault(mtu => mtu.UserId == recipient.Id);
            Assert.Null(messageToUser);
            Assert.Equal(0, _dataContext.MessagesToUsers.Count());
        }

        [Fact]
        public async void CreateNullMessageTest()
        {
            // act
            bool result = await _messageModule.CreateMessage(null);

            // assert
            Assert.False(result);
            Assert.Equal(0, _dataContext.Messages.Count());
            Assert.Equal(0, _dataContext.MessagesFromUsers.Count());
            Assert.Equal(0, _dataContext.MessagesToUsers.Count());
        }

        [Fact]
        public async void GetDeliveredMessagesToUserSuccessTest()
        {
            // arrange
            User toUser = _dataContext.Users.Add(new User()
            {
                Login = "recipient",
            }).Entity;

            Message message = _dataContext.Messages.Add(new Message()
            {
                Status = DeliveryStatus.Delivered,
                MessageText = "msgText",
            }).Entity;

            _dataContext.SaveChanges();

            _dataContext.MessagesToUsers.Add(new MessageToUser()
            {
                MessageId = message.Id,
                UserId = toUser.Id
            });

            _dataContext.SaveChanges();

            // act
            List<Message> result = await _messageModule.GetDeliveredMessagesToUser(toUser.Login);

            // assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal(message.MessageText, result[0].MessageText);
        }

        [Fact]
        public async void GetDeliveredMessagesForUnknownUserTest()
        {
            // arrange
            User toUser = _dataContext.Users.Add(new User()
            {
                Login = "recipient",
            }).Entity;

            Message message = _dataContext.Messages.Add(new Message()
            {
                Status = DeliveryStatus.Delivered,
                MessageText = "msgText",
            }).Entity;

            _dataContext.SaveChanges();

            _dataContext.MessagesToUsers.Add(new MessageToUser()
            {
                MessageId = message.Id,
                UserId = toUser.Id
            });

            _dataContext.SaveChanges();

            // act
            List<Message> result = await _messageModule.GetDeliveredMessagesToUser("UnknownUser");

            // assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        //todo Create a test for the user without messages
        // todo Create test for the user without messages with 'delivered' status
    }
}
