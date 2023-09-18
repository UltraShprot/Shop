using Shop.Interfaces;

namespace Shop.Services
{
	public class ConnectionService: IConnectionService
    {
		private static List<(string, string, string)> connections = new List<(string, string, string)>();

		public void AddConnection(string user, string user2, string connection)
		{
			connections.Add((user, user2, connection));
		}
		public void RemoveConnection(string connection)
		{
			foreach (var conn in connections)
			{
				if (conn.Item3.Equals(connection))
				{
					connections.Remove(conn);
				}
			}
		}
        public List<(string, string, string)> GetConnectionList(string user, string user2)
        {
			var newList = new List<(string, string, string)>();
			if (connections.Count > 0)
			{
				foreach (var conn in connections)
				{
					if ((conn.Item1 == user || conn.Item1 == user2) && (conn.Item2 == user || conn.Item2 == user2))
					{
						newList.Add(conn);
					}
				}
			}
			return newList;
		}
	}
}
/*public class ConnectionService : IConnectionService
{
	private static Dictionary<string, string> connections = new Dictionary<string, string>();

	public void AddConnection(string user, string connection)
	{
		connections.Add(user, connection);
	}
	public void RemoveConnection(string user)
	{
		connections.Remove(user);
	}
	public string GetConnectionList(string user)
	{
		return connections[user];
	}
}
}*/