using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Shop.Data;
using Shop.Interfaces;
using Shop.Repositories;
using Shop.Services;
using System.Security.Claims;

namespace Shop.Hubs
{
    public class ChatHub : Hub
    {
		private readonly UserManager<AppIdentityUser> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IConnectionService _connectionService;
		public ClaimsPrincipal User => _httpContextAccessor.HttpContext.User;
		public ChatHub(
			UserManager<AppIdentityUser> userManager,
	        IHttpContextAccessor httpContextAccessor,
			IConnectionService connectionService)
		{
			_httpContextAccessor = httpContextAccessor;
			_userManager = userManager;
			_connectionService = connectionService;
		}
		public async override Task OnConnectedAsync()
		{
			var user = await _userManager.GetUserAsync(User);
			_connectionService.AddConnection(user.Id, _httpContextAccessor.HttpContext.Request.Cookies["ToUserId"], Context.ConnectionId);
			await base.OnConnectedAsync();
		}
		public async Task SendMessage(Message message)
        {
			var fromUser = await _userManager.GetUserAsync(User);
			foreach ((string, string, string) connection in _connectionService.GetConnectionList(fromUser.Id, _httpContextAccessor.HttpContext.Request.Cookies["ToUserId"]))
			{
				await Clients.Client(connection.Item3).SendAsync("receiveMessage", message);
			}
		}
		public async override Task OnDisconnectedAsync(Exception? e)
		{
			var user = await _userManager.GetUserAsync(User);
			_connectionService.RemoveConnection(Context.ConnectionId);
			await base.OnDisconnectedAsync(e);
		}
	}
}
