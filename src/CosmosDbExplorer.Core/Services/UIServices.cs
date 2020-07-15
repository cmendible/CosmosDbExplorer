using CosmosDbExplorer.Messages;
using Prism.Events;

namespace CosmosDbExplorer.Services
{
    public class UIServices : IUIServices
    {
        private readonly IEventAggregator _messenger;

        public UIServices(IEventAggregator messenger)
        {
            _messenger = messenger;
        }

        public void SetBusyState(bool isBusy)
        {
            _messenger.GetEvent<IsBusyMessage>().Publish(isBusy);
        }
    }
}
