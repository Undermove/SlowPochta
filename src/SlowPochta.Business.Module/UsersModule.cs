using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;

namespace SlowPochta.Business.Module
{
	public class UsersModule : IDisposable
	{
		private readonly DataContext _dataContext;

		public UsersModule(DesignTimeDbContextFactory contextFactory)
		{
			_dataContext = contextFactory.CreateDbContext(new string[]{});
		}

		public void Dispose()
		{
			_dataContext?.Dispose();
		}

		public async Task<bool> TryRegisterAsync(PersonContract personContract)
		{
            if (await _dataContext.Users.AnyAsync(person => person.Login.Equals(personContract.Login)))
            {
                return false;
            }

            await _dataContext.Users.AddAsync(new User()
			{
				Login = personContract.Login,
				Password = personContract.Password,
				Role = RoleTypes.User
			});

            await _dataContext.SaveChangesAsync();

			return true;
		}
	}
}
