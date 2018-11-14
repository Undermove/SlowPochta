using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
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
            Mock<MessageStatusUpdaterConfig> configurationMock = new Mock<MessageStatusUpdaterConfig>();
            configurationMock.Setup(config => config.UpdateIntervalMinutes).Returns(1);

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
        public void Test()
        {
            //arrange
            _dataContext.Messages.Add(new Message());
            _dataContext.SaveChanges();

            //act


            //assert
        }
    }
}
