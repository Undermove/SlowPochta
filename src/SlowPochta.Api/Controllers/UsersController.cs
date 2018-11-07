using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Modules;

namespace SlowPochta.Api.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : Controller
	{
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
				return Ok("Accepted");
			}

			return BadRequest("Not registred");
		}
	}
}
