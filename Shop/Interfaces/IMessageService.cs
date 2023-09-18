using Shop.Data;
using Shop.Response;

namespace Shop.Interfaces
{
	public interface IMessageService 
    {
		Task<IBaseResponse<List<Message>>> GetMessages(string UserId);
        Task<IBaseResponse<Message>> Create(Message message);

        Task<IBaseResponse<List<AppIdentityUser>>> GetUsers();
    }
}
