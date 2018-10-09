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
	public class AuthModule : IDisposable
	{
		private readonly DataContext _context;

		public AuthModule(DesignTimeDbContextFactory context)
		{
			_context = context.CreateDbContext(new string[]{});
		}

		public ClaimsIdentity GetIdentity(string username, string password)
		{
			User user = _context.Users.FirstOrDefault(x => x.Login == username && x.Password == password);

			if (user == null)
			{
				// если пользователя нет в БД
				return null;
			}

			var claims = new List<Claim>
			{
				new Claim(ClaimsIdentity.DefaultNameClaimType, user.Login),
				new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
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
