using System;
using System.Security.Claims;
using userMgrApi.Core.Dtos.General;
using userMgrApi.Core.Dtos.Message;

namespace userMgrApi.Core.Interfaces
{
	public interface IMessageServices
	{
		Task<GeneralServiceResponseDto> CreateNewMessageAsync(ClaimsPrincipal User, CreateMessageDto createMessageDto);

		Task<IEnumerable<GetMessageDto>> GetMessagesAsync();

		Task<IEnumerable<GetMessageDto>> GetMyMessagesAsync(ClaimsPrincipal User);
	}
}

