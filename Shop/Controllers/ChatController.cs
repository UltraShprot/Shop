using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Data;
using Shop.Interfaces;

namespace Shop.Controllers
{
    public class ChatController : Controller
    {
        private readonly IMessageService _messageService;
        public ChatController(IMessageService messageService) 
        {
            _messageService = messageService;
        }
        [Authorize]
        public async Task<IActionResult> Index(string userId)
        {
            var users = await _messageService.GetUsers();
            var messages = await _messageService.GetMessages(userId);
            if (users.StatusCode == Enum.StatusCode.OK && (messages.StatusCode == Enum.StatusCode.OK || messages.StatusCode == Enum.StatusCode.NotFound))
            {
                var _tuple = new Tuple<List<AppIdentityUser>, List<Message>, string>(users.Data, messages.Data, userId);
                return View(_tuple);
            }
            if (users.StatusCode == Enum.StatusCode.OK)
            {
                var _tuple = new Tuple<List<AppIdentityUser>, List<Message>, string>(users.Data,null, userId);
                return View(_tuple); 
            }
            return NotFound();
        }
        [Authorize]
        public async Task<IActionResult> Create(Message message)
        {
            var response = await _messageService.Create(message);
            if (response.StatusCode == Enum.StatusCode.OK)
            {
                return Ok();
            }
            else if (response.StatusCode == Enum.StatusCode.NotFound)
            {
                return NotFound();
            }
            return View("Error", $"{response.Description}");
        }
    }
}
