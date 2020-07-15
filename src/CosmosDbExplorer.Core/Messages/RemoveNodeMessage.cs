using CosmosDbExplorer.Infrastructure.Models;
using Microsoft.Azure.Documents;
using Prism.Events;

namespace CosmosDbExplorer.Messages
{

    public class RemoveNodeMessage : PubSubEvent<string>
    {
        //public RemoveNodeMessage(string altLink)
        //{
        //    AltLink = altLink;
        //}

        //public string AltLink { get; }
    }
}
