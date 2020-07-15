using CosmosDbExplorer.Infrastructure.Models;
using Prism.Events;

namespace CosmosDbExplorer.Messages
{
    class ConnectionSettingSavedMessage : PubSubEvent<Connection>
    {
        //public ConnectionSettingSavedMessage(Connection connection)
        //{
        //    Connection = connection;
        //}

        //public Connection Connection { get; }
    }
}
