using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SlowPochta.Business.Module;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;
using Xunit;

namespace SlowPochta.Tests
{
	public class AuthModuleTests : IDisposable
	{
		private readonly AuthModule _authModule;
		private readonly DataContext _dataContext;

		public AuthModuleTests()
		{
			var contextFactory = new DesignTimeDbContextFactory();
			_dataContext = contextFactory.CreateDbContext(new string[]{});
			_dataContext.Database.Migrate();
			_authModule = new AuthModule(contextFactory);
		}

		public void Dispose()
		{
			_authModule.Dispose();
			_dataContext.Database.EnsureDeleted();
			_dataContext.Dispose();
		}

		[Fact]
		public void GetIdentitySuccessTest()
		{
			// arrange
			User testUser = new User()
			{
				Login = "test",
				Password = "test",
				Role = RoleTypes.User
			};

			_dataContext.Users.Add(testUser);
			_dataContext.SaveChanges();

			// act
			ClaimsIdentity claims = _authModule.GetIdentity(testUser.Login, testUser.Password);

			// assert
			Assert.NotNull(claims);
		}

		[Fact]
		public void GetIdentityFailTest()
		{
			// arrange
			User testUser = new User()
			{
				Login = "test",
				Password = "test",
				Role = RoleTypes.User
			};

			_dataContext.Users.Add(testUser);

			// act
			ClaimsIdentity claims = _authModule.GetIdentity("wrong login", "wrong password");

			// assert
			Assert.Null(claims);
		}
	}
}
