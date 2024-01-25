using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using userMgrApi.Core.DbContext;
using userMgrApi.Core.Dtos.General;
using userMgrApi.Core.Dtos.Message;
using userMgrApi.Core.Entities;
using userMgrApi.Core.Interfaces;

namespace userMgrApi.Core.Services
{
	public class MessageService : IMessageServices
	{
        private readonly ApplicationDbContext _context;
        private readonly ILogService _logService;
        private readonly UserManager<ApplicationUser> _userManager;

		public MessageService(ApplicationDbContext context, ILogService logService, UserManager<ApplicationUser> userManager)
		{
            _context = context;

            _logService = logService;

            _userManager = userManager;
		}

        public async Task<GeneralServiceResponseDto> CreateNewMessageAsync(ClaimsPrincipal User, CreateMessageDto createMessageDto)
        {
            if (User.Identity.Name == createMessageDto.ReceiverUserName)
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 400,
                    Message = "Sender and Receiver can not be same",
                };

            var isReceiverUserNameValid = _userManager.Users.Any(q => q.UserName == createMessageDto.ReceiverUserName);
            if(!isReceiverUserNameValid)
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 400,
                    Message = "Receiver username is not valid",
                };

            Message newMessage = new Message()
            {
                SenderUserName = User.Identity.Name,
                ReceiverUserName = createMessageDto.ReceiverUserName,
                Text = createMessageDto.Text
            };

            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            await _logService.SaveNewLog(User.Identity.Name, "Send Message");

            return new GeneralServiceResponseDto()
            {
                isSucceed = true,
                StatusCode = 201,
                Message = "Message sent successfully",
            };
        }

        public async Task<IEnumerable<GetMessageDto>> GetMessagesAsync()
        {
            var messages = await _context.Messages
                 .Select(q => new GetMessageDto()
                 {
                     Id = q.Id,
                     SenderUserName = q.SenderUserName,
                     ReceiverUserName = q.ReceiverUserName,
                     Text = q.Text,
                     CreatedAt = q.CreatedAt
                 })
                 .OrderByDescending(q => q.CreatedAt).ToListAsync();

            return messages;
        }

        public async Task<IEnumerable<GetMessageDto>> GetMyMessagesAsync(ClaimsPrincipal User)
        {
            var loggedInUser = User.Identity.Name;
            var messages = await _context.Messages
                .Where(q=>q.SenderUserName == loggedInUser || q.ReceiverUserName == loggedInUser)
                 .Select(q => new GetMessageDto()
                 {
                     Id = q.Id,
                     SenderUserName = q.SenderUserName,
                     ReceiverUserName = q.ReceiverUserName,
                     Text = q.Text,
                     CreatedAt = q.CreatedAt
                 })
                 .OrderByDescending(q => q.CreatedAt).ToListAsync();

            return messages;
        }
    }
}

