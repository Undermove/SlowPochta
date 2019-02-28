using System;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Moq;
using SlowPochta.Business.Module.Configuration;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Business.Module.Responses;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;
using Xunit;

namespace SlowPochta.Tests
{
	public class AuthModuleTests : IDisposable
	{
		private readonly AuthModule _authModule;
		private readonly DataContext _dataContext;
		private readonly Mock<AuthOptionsConfig> _authOptionsMock;

		public AuthModuleTests()
		{
			var contextFactory = new DesignTimeDbContextFactory();
			_dataContext = contextFactory.CreateDbContext(new string[]{});
			_dataContext.Database.Migrate();
			Mock<IConfigurationRoot> configMock = new Mock<IConfigurationRoot>();
			configMock.Setup(root => root[It.IsAny<string>()]).Returns("100");
			_authOptionsMock = new Mock<AuthOptionsConfig>(configMock.Object);
			_authOptionsMock.Setup(config => config.SymmetricSecurityKey).Returns(() => 
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes("sometestkeyfortests")));
			_authOptionsMock.Setup(config => config.LifetimeMinutes).Returns(() => 100);
			_authModule = new AuthModule(contextFactory, _authOptionsMock.Object);
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
			AuthResponse claims = _authModule.GenerateAuthResponse(
				new PersonContract
				{
					Login = testUser.Login, Password = testUser.Password
				});

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
			AuthResponse claims = _authModule.GenerateAuthResponse(
				new PersonContract
				{
					Login = "wrong login",
					Password = "wrong pass"
				});

			// assert
			Assert.Null(claims);
		}
	}
}
