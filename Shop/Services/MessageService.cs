using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis;
using Shop.Data;
using Shop.Enum;
using Shop.Hubs;
using Shop.Interfaces;
using Shop.Models;
using Shop.Response;
using System.Security.Claims;

namespace Shop.Services
{
	public class MessageService : IMessageService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<AppIdentityUser> _userManager;
		public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;
        private readonly IBaseRepository<Message> _messageRepository;
		public MessageService(
            IHttpContextAccessor httpContextAccessor,
            UserManager<AppIdentityUser> userManager,
            IBaseRepository<Message> messageRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _messageRepository = messageRepository;
        }
		public async Task<IBaseResponse<Message>> Create(Message message)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (string.IsNullOrEmpty(message.Text))
                {
                    return new BaseResponse<Message>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                await _messageRepository.Create(
                    new Message()
                    {
                        FromUserId = user.Id,
                        ToUserId = _httpContextAccessor.HttpContext.Request.Cookies["ToUserId"],
                        Text = message.Text,
                        UserName = user.UserName,
                    }
                    );
                return new BaseResponse<Message>()
                {
                    StatusCode = StatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<Message>()
                {
                    Description = $"[Search] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<Message>>> GetMessages(string UserId)
        {
            try
            {
                if (string.IsNullOrEmpty(UserId))
                {
					return new BaseResponse<List<Message>>()
					{
						StatusCode = StatusCode.NotFound
					};
				}
				var user = await _userManager.GetUserAsync(User);
				_httpContextAccessor.HttpContext.Response.Cookies.Append("ToUserId", UserId);
                var messages = _messageRepository.GetAll().Where(x => ((x.FromUserId == UserId && x.ToUserId == user.Id)||(x.FromUserId == user.Id && x.ToUserId == UserId)));
                if (!messages.Any())
                {
                    return new BaseResponse<List<Message>>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                return new BaseResponse<List<Message>>()
                {
                    StatusCode = StatusCode.OK,
                    Data = messages.ToList()
                };
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<Message>>()
                {
                    Description = $"[Search] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<AppIdentityUser>>> GetUsers()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                var users = _userManager.Users.Where(x => x.Id != user.Id);
                if (!users.Any())
                {
                    return new BaseResponse<List<AppIdentityUser>>()
                    {
                        StatusCode = StatusCode.NotFound
                    };
                }
                return new BaseResponse<List<AppIdentityUser>>()
                {
                    StatusCode = StatusCode.OK,
                    Data = users.ToList()
                };


            }
            catch (Exception ex)
            {
                return new BaseResponse<List<AppIdentityUser>>()
                {
                    Description = $"[Search] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
