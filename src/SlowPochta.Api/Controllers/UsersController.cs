using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Core;
using Microsoft.Extensions.Logging;

namespace SlowPochta.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : Controller
	{
	    private static readonly ILogger Logger = ApplicationLogging.CreateLogger<UsersController>();
        private readonly UsersModule _usersModule;

		public UsersController(UsersModule usersModule)
		{
			_usersModule = usersModule;
		}

		[HttpPut]
		public async Task<IActionResult> RegisterUser([FromBody] PersonContract personContract)
		{
			if (await _usersModule.TryRegisterAsync(personContract))
			{
			    Logger.LogInformation($"The user {personContract.Login} is registered");
                return Ok("Accepted");
			}
		    Logger.LogInformation($"The user {personContract.Login} is not registered");
            return BadRequest("Not registred");
		}
	}
}
