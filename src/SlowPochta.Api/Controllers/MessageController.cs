using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SlowPochta.Api.Requests;
using SlowPochta.Business.Module.DataContracts;
using SlowPochta.Business.Module.Modules;
using SlowPochta.Core;

namespace SlowPochta.Api.Controllers
{
    [Route("api/[controller]")]
    public class MessageController : Controller
    {
        private static readonly ILogger Logger = ApplicationLogging.CreateLogger<MessageController>();

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
	        Logger.LogInformation($"All delivered messages were requested from user: {currentUserLogin}");
            List<MessageAnswerContract> messages = await _messageModule.GetDeliveredMessagesToUser(currentUserLogin);
	        Logger.LogInformation($"The user: {currentUserLogin} got his delivered messages successfully");
            return Json(messages);
        }

	    [Authorize]
	    [HttpGet]
	    [Route("getnewmessagescount")]
	    public async Task<IActionResult> GetNewMessagesCount()
	    {
		    string currentUserLogin = User.Identity.Name;
		    Logger.LogInformation($"Delivered messages count were requested from user: {currentUserLogin}");
		    List<MessageAnswerContract> messages = await _messageModule.GetDeliveredMessagesToUser(currentUserLogin);
		    Logger.LogInformation($"The user: {currentUserLogin} got his delivered messages successfully");
		    return Json(messages.Count(contract => !contract.IsRead));
	    }

		[Authorize]
		[HttpPost]
        public async Task<IActionResult> CreateMessage([FromBody] MessageRequestContract messageRequestContract)
        {
            if (await _messageModule.CreateMessage(messageRequestContract))
            {
                Logger.LogInformation($"The message from {messageRequestContract.FromUser} to {messageRequestContract.ToUser} was created");
                return Ok("Message created");
            }
            Logger.LogInformation($"Message {messageRequestContract.MessageText} from {messageRequestContract.FromUser} to {messageRequestContract.ToUser} wasn't created");
            return BadRequest("Message was'n created");
        }

	    [Authorize]
	    [HttpGet]
        [Route("getsendedmessages")]
		public async Task<IActionResult> GetSendedMessages()
	    {
	        string currentUserLogin = User.Identity.Name;
	        Logger.LogInformation($"All sended messages were requested from user: {currentUserLogin}");
            List<MessageAnswerContract> messages = await _messageModule.GetMessagesFromUser(currentUserLogin);
	        Logger.LogInformation($"The user: {currentUserLogin} got his sended messages successfully");
            return Json(messages);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] SingleItemRequest request)
        {
			if (request.Id == null)
	        {
	            Logger.LogInformation($"The request ID: {request.Id} was empty");
                return BadRequest("Request Id is Empty");
	        }

            string currentUserLogin = User.Identity.Name;
            Logger.LogInformation($"Message ID info was requested from user: {currentUserLogin}");
            var message = await _messageModule.GetMessageById(request.Id.Value, currentUserLogin);
            Logger.LogInformation($"The user: {currentUserLogin} got his message ID info successfully");
            return Json(message);
        }

	    [Authorize]
	    [HttpGet]
	    [Route("markread")]
		public async Task<IActionResult> MarkRead([FromQuery] SingleItemRequest request)
	    {
		    if (request.Id == null)
		    {
			    Logger.LogInformation($"The request ID: {request.Id} was empty");
			    return BadRequest("Request Id is Empty");
		    }

		    string currentUserLogin = User.Identity.Name;
		    Logger.LogInformation($"Message {request.Id} was marked read by user: {currentUserLogin}");
		    await _messageModule.MarkMessageRead(request.Id.Value);
		    Logger.LogInformation($"The user: {currentUserLogin} marked message {request.Id} info successfully");
		    return Ok();
	    }
	}
}
