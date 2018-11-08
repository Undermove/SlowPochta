using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SlowPochta.Api.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        [Authorize]
        [Route("getdeliveredmessages")]
        public IActionResult GetDeliveredMessages()
        {
            return Ok();
        }

        [Authorize(Roles = "test")]
        [Route("createmessage")]
        public IActionResult CreateMessage()
        {
            return Ok();
        }
    }
}
