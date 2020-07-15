using CosmosDbExplorer.Infrastructure.Models;
using Microsoft.Azure.Documents;
using Prism.Events;

namespace CosmosDbExplorer.Messages
{
    public class UpdateOrCreateNodeMessage<T> : PubSubEvent<UpdateOrCreateNodePayload<T>>
        where T : Resource
    { }

    public class UpdateOrCreateNodePayload<T> 
        where T: Resource
    {
        public UpdateOrCreateNodePayload(T resource, DocumentCollection collection, string oldAltLink)
        {
            Resource = resource;
            OldAltLink = oldAltLink;
            Collection = collection;
        }

        public T Resource { get; }

        public bool IsNewResource => string.IsNullOrEmpty(OldAltLink);

        public string OldAltLink { get; }

        public DocumentCollection Collection { get; }
    }
}
