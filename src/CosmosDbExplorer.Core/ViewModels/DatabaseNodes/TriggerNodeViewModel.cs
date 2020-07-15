using System.Linq;
using System.Threading.Tasks;
using CosmosDbExplorer.Messages;
using Microsoft.Azure.Documents;

namespace CosmosDbExplorer.ViewModels
{
    public class TriggerRootNodeViewModel : AssetRootNodeViewModelBase<Trigger>
    {
        public TriggerRootNodeViewModel(CollectionNodeViewModel parent)
            : base(parent)
        {
            Name = "Triggers";
        }

        protected override async Task LoadChildren()
        {
            IsLoading = true;

            var triggers = await DbService.GetTriggersAsync(Parent.Parent.Parent.Connection, Parent.Collection).ConfigureAwait(false);

            foreach (var trigger in triggers)
            {
                //await DispatcherHelper.RunAsync(() => Children.Add(new TriggerNodeViewModel(this, trigger)));
                Children.Add(new TriggerNodeViewModel(this, trigger));
            }

            IsLoading = false;
        }

        protected override void OnUpdateOrCreateNodeMessage(UpdateOrCreateNodePayload<Trigger> message)
        {
            if (message.IsNewResource)
            {
                var item = new TriggerNodeViewModel(this, message.Resource);
                //DispatcherHelper.RunAsync(() => Children.Add(item));
                Children.Add(item);
            }
            else
            {
                var item = Children.Cast<TriggerNodeViewModel>().FirstOrDefault(i => i.Resource.AltLink == message.OldAltLink);

                if (item != null)
                {
                    item.Resource = message.Resource;
                }
            }
        }
    }

    public class TriggerNodeViewModel : AssetNodeViewModelBase<Trigger, TriggerRootNodeViewModel>
    {
        public TriggerNodeViewModel(TriggerRootNodeViewModel parent, Trigger resource)
            : base(parent, resource)
        {
        }

        protected override Task DeleteCommandImpl()
        {
            return DialogService.ShowMessage("Are sure you want to delete this Trigger?", "Delete", null, null,
                async confirm =>
                {
                    if (confirm)
                    {
                        await DbService.DeleteTriggerAsync(Parent.Parent.Parent.Parent.Connection, Resource.AltLink).ConfigureAwait(false);
                        //await DispatcherHelper.RunAsync(() => Parent.Children.Remove(this));
                        Parent.Children.Remove(this);
                        MessengerInstance.GetEvent<CloseDocumentMessage>().Publish(ContentId);
                    }
                });
        }

        protected override Task EditCommandImpl()
        {
            MessengerInstance.GetEvent<EditTriggerMessage>().Publish(new OpenTabMessageBase<TriggerNodeViewModel>(this, Parent.Parent.Parent.Parent.Connection, Parent.Parent.Collection));
            return Task.FromResult(0);
        }
    }
}
