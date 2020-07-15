using CosmosDbExplorer.Infrastructure.Models;
using Prism.Events;

namespace CosmosDbExplorer.Messages
{
    public class TreeNodeSelectedMessage : PubSubEvent<TreeViewItemViewModel>
    {
        //public TreeNodeSelectedMessage(TreeViewItemViewModel item)
        //{
        //    Item = item;
        //}

        //public TreeViewItemViewModel Item { get; }
    }
}
