using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Moq;
using SlowPochta.Business.Module;
using SlowPochta.Business.Module.Configuration;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;
using Xunit;

namespace SlowPochta.Tests
{
    public class MessageStatusUpdaterTests : IDisposable
    {
        private readonly MessageStatusUpdater _messageStatusUpdater;
        private readonly DataContext _dataContext;

        public MessageStatusUpdaterTests()
        {
            var contextFactory = new DesignTimeDbContextFactory();
            _dataContext = contextFactory.CreateDbContext(new string[] { });
            _dataContext.Database.Migrate();

            Mock<IConfigurationRoot> mock = new Mock<IConfigurationRoot>();
            mock.Setup(root => root[It.IsAny<string>()]).Returns("1");

            Mock<MessageStatusUpdaterConfig> configurationMock = new Mock<MessageStatusUpdaterConfig>(mock.Object);
            configurationMock.Setup(config => config.UpdateIntervalSeconds).Returns(1);

            _messageStatusUpdater = new MessageStatusUpdater(
                contextFactory,
                new MessageModule(contextFactory),
                configurationMock.Object);
        }

        public void Dispose()
        {
            _messageStatusUpdater.Dispose();
            _dataContext.Database.EnsureDeleted();
            _dataContext.Dispose();
        }

        [Fact]
        public void MessageStatusDescriptionUpdatingTest()
        {
            //arrange
            _dataContext.Messages.Add(new Message());
	        _dataContext.MessageDeliveryStatusVariants.Add(
		        new MessageDeliveryStatusVariant()
		        {
			        DeliveryStatusDescription = "Some Delivery status"
		        });
            _dataContext.SaveChanges();                                     

            //act
            _messageStatusUpdater.StartService();
            Thread.Sleep(5000);
            _dataContext.SaveChanges();

            //assert
            var msg = _dataContext.Messages.Find(1);
            Assert.False(string.IsNullOrEmpty(msg.StatusDescription));
            var deliveryStatus = _dataContext.MessagePassedDeliveryStatuses.Find(1);
            Assert.NotNull(deliveryStatus);
        }

        [Fact]
        public void TryChangeDeliveredStatusDescriptionTest()
        {
            //arrange
            Message testMessage = _dataContext.Messages.Add(new Message()
            {
                StatusDescription = "",
                Status = DeliveryStatus.Delivered,
            }).Entity;
            _dataContext.SaveChanges();

            //act
            _messageStatusUpdater.StartService();
            Thread.Sleep(5000);

            //assert           
            Assert.True(testMessage.StatusDescription == "");
            var deliveryStatus = _dataContext.MessagePassedDeliveryStatuses.Any();
            Assert.False(deliveryStatus);
        }

        [Fact]
        public void TryGetStatusFromDbTest()
        {
            //arrange
            _dataContext.Messages.Add(new Message());

            MessageDeliveryStatusVariant messageDeliveryStatus = _dataContext.MessageDeliveryStatusVariants.Add(
                new MessageDeliveryStatusVariant()
                {
                    DeliveryStatusDescription = " in the Dark Deep Beneath"
                }).Entity;
            _dataContext.SaveChanges();

            //act
            _messageStatusUpdater.StartService();
            Thread.Sleep(5000);

            //assert           
            Assert.True(messageDeliveryStatus.DeliveryStatusDescription == " in the Dark Deep Beneath");
        }
    }
}