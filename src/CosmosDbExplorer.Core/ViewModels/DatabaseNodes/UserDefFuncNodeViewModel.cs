using System.Linq;
using System.Threading.Tasks;
using CosmosDbExplorer.Infrastructure;
using CosmosDbExplorer.Messages;
using Microsoft.Azure.Documents;

namespace CosmosDbExplorer.ViewModels
{
    public class UserDefFuncRootNodeViewModel : AssetRootNodeViewModelBase<UserDefinedFunction>
    {
        public UserDefFuncRootNodeViewModel(CollectionNodeViewModel parent)
            : base(parent)
        {
            Name = "User Defined Functions";
        }

        protected override async Task LoadChildren()
        {
            IsLoading = true;

            var function = await DbService.GetUdfsAsync(Parent.Parent.Parent.Connection, Parent.Collection).ConfigureAwait(false);

            foreach (var func in function)
            {
                await DispatcherHelper.RunAsync(() => Children.Add(new UserDefFuncNodeViewModel(this, func)));
            }

            IsLoading = false;
        }

        protected override void OnUpdateOrCreateNodeMessage(UpdateOrCreateNodePayload<UserDefinedFunction> message)
        {
            if (message.IsNewResource)
            {
                var item = new UserDefFuncNodeViewModel(this, message.Resource);
                DispatcherHelper.RunAsync(() => Children.Add(item));
            }
            else
            {
                var item = Children.Cast<UserDefFuncNodeViewModel>().FirstOrDefault(i => i.Resource.AltLink == message.OldAltLink);

                if (item != null)
                {
                    item.Resource = message.Resource;
                }
            }
        }
    }

    public class UserDefFuncNodeViewModel : AssetNodeViewModelBase<UserDefinedFunction, UserDefFuncRootNodeViewModel>
    {
        public UserDefFuncNodeViewModel(UserDefFuncRootNodeViewModel parent, UserDefinedFunction resource)
            : base(parent, resource)
        {
        }

        protected override Task DeleteCommandImpl()
        {
            return DialogService.ShowMessage("Are sure you want to delete this User Definied Function?", "Delete", null, null,
                async confirm =>
                {
                    if (confirm)
                    {
                        await DbService.DeleteUdfAsync(Parent.Parent.Parent.Parent.Connection, Resource.AltLink).ConfigureAwait(false);
                        await DispatcherHelper.RunAsync(() => Parent.Children.Remove(this));
                        MessengerInstance.GetEvent<CloseDocumentMessage>().Publish(ContentId);
                    }
                });
        }

        protected override Task EditCommandImpl()
        {
            MessengerInstance.GetEvent<EditUserDefFuncMessage>().Publish(new OpenTabMessageBase<UserDefFuncNodeViewModel>(this, Parent.Parent.Parent.Parent.Connection, null));
            return Task.FromResult(0);
        }
    }
}
