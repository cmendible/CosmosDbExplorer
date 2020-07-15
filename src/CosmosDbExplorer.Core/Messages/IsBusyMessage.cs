using Prism.Events;

namespace CosmosDbExplorer.Messages
{
    public class IsBusyMessage : PubSubEvent<bool>
    {
        //public IsBusyMessage(bool isBusy)
        //{
        //    IsBusy = isBusy;
        //}

        //public bool IsBusy { get; }
    }
}
