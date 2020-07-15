using CosmosDbExplorer.Infrastructure.Models;
using CosmosDbExplorer.ViewModel;
using Microsoft.Azure.Documents;
using Prism.Events;

namespace CosmosDbExplorer.Messages
{
    public class OpenTabMessageBase<TNodeViewModel> 
    {
        public OpenTabMessageBase(TNodeViewModel node, Connection connection, DocumentCollection collection)
        {
            Node = node;
            Connection = connection;
            Collection = collection;
        }

        public TNodeViewModel Node { get; }

        public Connection Connection { get; }

        public DocumentCollection Collection { get; }
    }

    public class EditUserDefFuncMessage : PubSubEvent<OpenTabMessageBase<UserDefFuncNodeViewModel>>
    {
    }

    public class EditStoredProcedureMessage : PubSubEvent<OpenTabMessageBase<StoredProcedureNodeViewModel>>
    {
    }

    public class OpenCollectionMetricsViewMessage : PubSubEvent<OpenTabMessageBase<CollectionMetricsNodeViewModel>>
    {
    }

    public class OpenScaleAndSettingsViewMessage : PubSubEvent<OpenTabMessageBase<ScaleSettingsNodeViewModel>>
    {
    }

    public class OpenDatabaseScaleViewMessage : PubSubEvent<OpenTabMessageBase<DatabaseScaleNodeViewModel>>
    {
    }

    public class OpenQueryViewMessage : PubSubEvent<OpenTabMessageBase<CollectionNodeViewModel>>
    {
    }

    public class OpenDocumentsViewMessage : PubSubEvent<OpenTabMessageBase<DocumentNodeViewModel>>
    {
    }

    public class OpenImportDocumentViewMessage : PubSubEvent<OpenTabMessageBase<CollectionNodeViewModel>>
    {
    }

    public class EditTriggerMessage : PubSubEvent<OpenTabMessageBase<TriggerNodeViewModel>>
    {
    }

    public class EditUserMessage : PubSubEvent<OpenTabMessageBase<UserNodeViewModel>>
    {
    }

    public class EditPermissionMessage : PubSubEvent<OpenTabMessageBase<PermissionNodeViewModel>>
    {
    }
}
