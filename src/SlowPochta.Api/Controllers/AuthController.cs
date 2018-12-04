using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using SlowPochta.Api.Configuration;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Core;

namespace SlowPochta.Api.Controllers
{
	public class AuthController : Controller
	{
		private static readonly ILogger Logger = ApplicationLogging.CreateLogger<AuthController>();

		private readonly AuthModule _authModule;
		private readonly AuthOptions _authOptions;

		public AuthController(
			AuthModule authModule,
			AuthOptions authOptions)
		{
			_authModule = authModule;
			_authOptions = authOptions;
		}

		[HttpPost("/token")]
		public async Task Token([FromBody]PersonContract personContract)
		{
			Logger.LogInformation($"Token requested from user: {personContract.Login}");
			var username = personContract.Login;
			var password = personContract.Password;

			var identity = _authModule.GetIdentity(username, password);
			if (identity == null)
			{
				Response.StatusCode = 400;
				await Response.WriteAsync("Invalid username or password.");

				Logger.LogInformation($"Invalid credentials for user: {personContract.Login}");

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

			Logger.LogInformation($"Token creation success for user: {personContract.Login}");
		}
	}
}
