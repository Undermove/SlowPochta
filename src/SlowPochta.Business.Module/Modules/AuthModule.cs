using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using SlowPochta.Api.Configuration;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Responses;
using SlowPochta.Core;
using SlowPochta.Data.Model;
using SlowPochta.Data.Repository;

namespace SlowPochta.Business.Module.Modules
{
	public class AuthModule : IDisposable
	{
		private readonly AuthOptions _authOptions;
		private static readonly ILogger Logger = ApplicationLogging.CreateLogger<AuthModule>();
		private readonly DataContext _context;

		public AuthModule(DesignTimeDbContextFactory context, AuthOptions authOptions)
		{
			_authOptions = authOptions;
			_context = context.CreateDbContext(new string[]{});
		}

		public AuthResponse GenerateAuthResponse(PersonContract personContract)
		{
			var username = personContract.Login;
			var password = personContract.Password;

			var identity = GetIdentity(username, password);
			if (identity == null)
			{
				return null;
			}

			var now = DateTime.UtcNow;
			// создаем JWT-токен
			var jwt = new JwtSecurityToken(
				issuer: _authOptions.Issuer,
				audience: _authOptions.Issuer,
				notBefore: now,
				claims: identity.Claims,
				expires: now.Add(TimeSpan.FromMinutes(_authOptions.LifetimeMinutes)),
				signingCredentials: new SigningCredentials(_authOptions.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256));
			var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

			var response = new AuthResponse(encodedJwt, identity.Name);

			return response;
		}

		private ClaimsIdentity GetIdentity(string username, string password)
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

		public string CheckJwtToken(string token)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authOptions.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			List<Exception> validationFailures = null;
			SecurityToken validatedToken;
			var validator = new JwtSecurityTokenHandler();

			TokenValidationParameters validationParameters = new TokenValidationParameters();
			validationParameters.ValidIssuer = "http://localhost:5000";
			validationParameters.ValidAudience = "http://localhost:5000";
			validationParameters.IssuerSigningKey = key;
			validationParameters.ValidateIssuerSigningKey = true;
			validationParameters.ValidateAudience = true;

			if (validator.CanReadToken(token))
			{
				try
				{
					var principal = validator.ValidateToken(token, validationParameters, out validatedToken);
					if (principal.HasClaim(c => c.Type == ClaimTypes.Email))
					{
						return principal.Claims.First(c => c.Type == ClaimTypes.Email).Value;
					}
				}
				catch (Exception e)
				{
					Logger.LogError(null, e);
				}
			}

			return String.Empty;
		}

		public void Dispose()
		{
			_context?.Database.CloseConnection();
			_context?.Dispose();
		}
	}
}
