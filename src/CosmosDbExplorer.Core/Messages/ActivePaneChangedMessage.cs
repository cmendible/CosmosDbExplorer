using CosmosDbExplorer.ViewModels;
using Prism.Events;

namespace CosmosDbExplorer.Messages
{
    public class ActivePaneChangedMessage : PubSubEvent<ViewModelBase>
    {
        //public ActivePaneChangedMessage(ViewModelBase paneViewModel)
        //{
        //    PaneViewModel = paneViewModel;
        //}

        //public ViewModelBase PaneViewModel { get; }
    }
}
