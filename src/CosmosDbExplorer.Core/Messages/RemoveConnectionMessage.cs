using CosmosDbExplorer.Infrastructure.Models;
using Prism.Events;

namespace CosmosDbExplorer.Messages
{
    public class RemoveConnectionMessage : PubSubEvent<Connection>
    {
        //public RemoveConnectionMessage(Connection connection)
        //{
        //    Connection = connection;
        //}

        //public Connection Connection { get; }
    }
}
