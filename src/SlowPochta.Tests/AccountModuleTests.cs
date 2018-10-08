using System;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SlowPochta.Business.Module;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;
using Xunit;

namespace SlowPochta.Tests
{
	public class AccountModuleTests : IDisposable
	{
		private readonly AccountModule _accountModule;
		private readonly DataContext _dataContext;

		public AccountModuleTests()
		{
			var contextFactory = new DesignTimeDbContextFactory();
			_dataContext = contextFactory.CreateDbContext(new string[]{});
			_dataContext.Database.Migrate();
			_accountModule = new AccountModule(contextFactory);
		}

		public void Dispose()
		{
			_accountModule.Dispose();
			_dataContext.Database.EnsureDeleted();
			_dataContext.Dispose();
		}

		[Fact]
		public void GetIdentitySuccessTest()
		{
			// arrange
			Person testPerson = new Person()
			{
				Login = "test",
				Password = "test",
				Role = "admin"
			};

			_dataContext.Persons.Add(testPerson);
			_dataContext.SaveChanges();

			// act
			ClaimsIdentity claims = _accountModule.GetIdentity(testPerson.Login, testPerson.Password);

			// assert
			Assert.NotNull(claims);
		}

		[Fact]
		public void GetIdentityFailTest()
		{
			// arrange
			Person testPerson = new Person()
			{
				Login = "test",
				Password = "test",
				Role = "admin"
			};

			_dataContext.Persons.Add(testPerson);

			// act
			ClaimsIdentity claims = _accountModule.GetIdentity("wrong login", "wrong password");

			// assert
			Assert.Null(claims);
		}
	}
}
