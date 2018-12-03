using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SlowPochta.Api.Requests;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Modules;

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

			List<MessageAnswerContract> messages = await _messageModule.GetDeliveredMessagesToUser(currentUserLogin);

	        return Json(messages);
        }

        [Authorize]
		[HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] MessageContract messageContract)
        {
            if (await _messageModule.CreateMessage(messageContract))
            {
                return Ok("Message created");
            }

            return BadRequest("Message was'n created");
        }

	    [Authorize]
	    [HttpGet]
        [Route("getsendedmessages")]
		public async Task<IActionResult> GetSendedMessages()
	    {
	        string currentUserLogin = User.Identity.Name;

	        List<MessageAnswerContract> messages = await _messageModule.GetMessagesFromUser(currentUserLogin);

	        return Json(messages);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SingleItemRequest request)
        {
			if (request.Id == null)
	        {
		        return BadRequest("Request Id is Empty");
	        }

			var message = await _messageModule.GetMessageById(request.Id.Value);

	        return Json(message);
        }
    }
}
