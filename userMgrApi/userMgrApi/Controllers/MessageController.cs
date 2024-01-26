using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using userMgrApi.Core.Constants;
using userMgrApi.Core.Dtos.Message;
using userMgrApi.Core.Interfaces;

namespace userMgrApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class MessageController : ControllerBase 
	{
		private readonly IMessageServices _messageService;

		public MessageController(IMessageServices messageServices)
		{
			_messageService = messageServices;
		}

		//send message to another user

		[HttpPost]
		[Route("create")]
		[Authorize]
		public async Task<IActionResult> CreateNewMessage([FromBody] CreateMessageDto createMessageDto)
		{
			var result = await _messageService.CreateNewMessageAsync(User, createMessageDto);
			if(result.isSucceed)
			{
				return Ok(result.Message);
			}

			return StatusCode(result.StatusCode, result.Message);

		}

		//get messages of the current user
		[HttpGet]
		[Route("mine")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<GetMessageDto>>>GetMessages()
		{
			var messages = await _messageService.GetMyMessagesAsync(User);
			return Ok(messages);
		}

		//owner get all the messages including other people messages
		[HttpGet]
		[Authorize(Roles =StaticUserRoles.OwnerAdmin)]
		public async Task<ActionResult <IEnumerable<GetMessageDto>>>GetMessages()
		{
			var messages = await _messageService.GetMessagesAsync();
			return Ok(messages);
		}


	}
}

