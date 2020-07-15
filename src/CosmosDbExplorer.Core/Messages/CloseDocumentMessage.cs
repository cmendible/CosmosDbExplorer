using CosmosDbExplorer.Infrastructure.Models;
using Prism.Events;

namespace CosmosDbExplorer.Messages
{
    public class CloseDocumentMessage : PubSubEvent<string>
    {
        //public CloseDocumentMessage(string contentId)
        //{
        //    ContentId = contentId;
        //}

        //public CloseDocumentMessage(PaneViewModelBase paneViewModel)
        //    : this(paneViewModel.ContentId)
        //{
        //}

        //public string ContentId { get; set; }
    }
}
