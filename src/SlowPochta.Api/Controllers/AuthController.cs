using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Core;

namespace SlowPochta.Api.Controllers
{
	public class AuthController : Controller
	{
	    private static readonly ILogger Logger = ApplicationLogging.CreateLogger<AuthController>();

        private readonly AuthModule _authModule;

		public AuthController(AuthModule authModule)
		{
			_authModule = authModule;
		}

		[HttpPost("/token")]
		public async Task Token([FromBody]PersonContract personContract)
		{
			Logger.LogInformation($"Token requested from user: {personContract.Login}");

			var authResponse = _authModule.GenerateAuthResponse(personContract);
			if (authResponse == null)
			{
				Response.StatusCode = 400;
				await Response.WriteAsync("Invalid username or password.");

				Logger.LogInformation($"Invalid credentials for user: {personContract.Login}");

				return;
			}

			// сериализация ответа
			Response.ContentType = "application/json";
			await Response.WriteAsync(JsonConvert.SerializeObject(authResponse, new JsonSerializerSettings { Formatting = Formatting.Indented }));

			Logger.LogInformation($"Token creation success for user: {personContract.Login}");
		}
	}
}
