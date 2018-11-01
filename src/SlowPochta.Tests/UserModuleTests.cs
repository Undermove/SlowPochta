using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlowPochta.Business.Module;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;
using Xunit;

namespace SlowPochta.Tests
{
    public class UserModuleTests : IDisposable
    {
        private readonly UsersModule _usersModule;
        private readonly DataContext _dataContext;

        public UserModuleTests()
        {
            var contextFactory = new DesignTimeDbContextFactory();
            _dataContext = contextFactory.CreateDbContext(new string[] { });
            _dataContext.Database.Migrate();
            _usersModule = new UsersModule(contextFactory);
        }

        public void Dispose()
        {
            _usersModule.Dispose();
            _dataContext.Database.EnsureDeleted();
            _dataContext.Dispose();
        }

        [Fact]
        public async void TryRegisterSuccessTest()
        {
            // arrange
            PersonContract testUser = new PersonContract()
            {
                Login = "test",
                Password = "test",                
            };

            // act
            bool isRegistred = await _usersModule.TryRegisterAsync(testUser);

            // assert
            Assert.True(isRegistred);
            Assert.True(_dataContext.Users.Any(user => user.Login == testUser.Login && 
                                                       user.Password == testUser.Password));
            Assert.Equal(1, _dataContext.Users.Count());
        }

        [Fact]
        public async void TryRegisterFailTest()
        {
            // arrange
            PersonContract testUser = new PersonContract()
            {
                Login = "test",
                Password = "test",
            };

            // act
            await _usersModule.TryRegisterAsync(testUser);
            bool isRegistred = await _usersModule.TryRegisterAsync(testUser);

            // assert
            Assert.False(isRegistred);
            Assert.True(_dataContext.Users.Any(user => user.Login == testUser.Login &&
                                                       user.Password == testUser.Password));
            Assert.Equal(1, _dataContext.Users.Count());
        }
    }
}
