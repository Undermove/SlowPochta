using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Data.Model;

namespace SlowPochta.Api.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
	    private readonly MessageModule _messageModule;

	    public MessageController(MessageModule messageModule)
	    {
		    _messageModule = messageModule;
	    }

	    [Authorize]
		[HttpGet]
        [Route("getdeliveredmessages")]
        public async Task<IActionResult> GetDeliveredMessages()
	    {
		    string currentUserLogin = User.Identity.Name;

			List<Message> messages = await _messageModule.GetDeliveredMessagesToUser(currentUserLogin);

	        return Json(messages);
        }

        [Authorize(Roles = "test")]
		[HttpPut]
        public async Task<IActionResult> CreateMessage([FromBody] MessageContract messageContract)
        {
            bool message = await _messageModule.CreateMessage(messageContract);

            return Json(message);
        }

	    [Authorize(Roles = "test")]
	    [HttpGet]
        [Route("getsendedmessages")]
		public async Task<IActionResult> GetSendedMessages()
	    {
	        string currentUserLogin = User.Identity.Name;

	        List<Message> messages = await _messageModule.GetMessagesFromUser(currentUserLogin);

	        return Json(messages);
        }
	}
}
