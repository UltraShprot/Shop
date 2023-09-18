namespace Shop.Interfaces
{
	public interface IConnectionService
    {
		void AddConnection(string user, string user2, string connection);
		void RemoveConnection(string connection);
        List<(string,string,string)> GetConnectionList(string user, string user2);
	}
}
