using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SlowPochta.Api.Configuration;
using SlowPochta.Business.Module;

namespace SlowPochta.Api.Controllers
{
	public class AccountController : Controller
	{
		private readonly AccountModule _accountModule;
		private readonly AuthOptions _authOptions;

		public AccountController(
			AccountModule accountModule,
			AuthOptions authOptions)
		{
			_accountModule = accountModule;
			_authOptions = authOptions;
		}

		[HttpPost("/token")]
		public async Task Token()
		{
			var username = Request.Form["username"];
			var password = Request.Form["password"];

			var identity = _accountModule.GetIdentity(username, password);
			if (identity == null)
			{
				Response.StatusCode = 400;
				await Response.WriteAsync("Invalid username or password.");
				return;
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

			var response = new
			{
				access_token = encodedJwt,
				username = identity.Name
			};

			// сериализация ответа
			Response.ContentType = "application/json";
			await Response.WriteAsync(JsonConvert.SerializeObject(response, new JsonSerializerSettings { Formatting = Formatting.Indented }));
		}
	}
}
