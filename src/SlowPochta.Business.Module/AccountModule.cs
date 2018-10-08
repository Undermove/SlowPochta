using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SlowPochta.Data;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;

namespace SlowPochta.Business.Module
{
	public class AccountModule : IDisposable
	{
		private readonly DataContext _context;

		public AccountModule(DesignTimeDbContextFactory context)
		{
			_context = context.CreateDbContext(new string[]{});
		}

		public ClaimsIdentity GetIdentity(string username, string password)
		{
			Person person = _context.Persons.FirstOrDefault(x => x.Login == username && x.Password == password);

			if (person == null)
			{
				// если пользователя нет в БД
				return null;
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, person.Login),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role)
			};
			ClaimsIdentity claimsIdentity =
				new ClaimsIdentity(claims, "Token", ClaimsIdentity.DefaultNameClaimType,
					ClaimsIdentity.DefaultRoleClaimType);
			return claimsIdentity;
		}

		public void Dispose()
		{
			_context?.Database.CloseConnection();
			_context?.Dispose();
		}
	}
}
