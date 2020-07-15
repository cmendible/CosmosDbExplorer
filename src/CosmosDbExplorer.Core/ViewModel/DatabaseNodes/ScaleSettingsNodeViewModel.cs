using CosmosDbExplorer.Infrastructure;
using CosmosDbExplorer.Infrastructure.Models;
using CosmosDbExplorer.Messages;

namespace CosmosDbExplorer.ViewModel
{
    public class ScaleSettingsNodeViewModel : TreeViewItemViewModel<CollectionNodeViewModel>, IHaveCollectionNodeViewModel, IContent
    {
        private RelayCommand _openCommand;

        public ScaleSettingsNodeViewModel(CollectionNodeViewModel parent)
            : base(parent, parent.MessengerInstance, false)
        {
            Name = parent.IsDatabaseLevelThroughput
                        ? "Settings"
                        : "Scale & Settings";
        }

        public string Name { get; private set; }

        public string ContentId => Parent.Collection.SelfLink + "/ScaleSettings";

        public RelayCommand OpenCommand
        {
            get
            {
                return _openCommand
                    ?? (_openCommand = new RelayCommand(
                        () => MessengerInstance.GetEvent<OpenScaleAndSettingsViewMessage>().Publish(new OpenTabMessageBase<ScaleSettingsNodeViewModel>(this, Parent.Parent.Parent.Connection, Parent.Collection))
                        ));
            }
        }

        public CollectionNodeViewModel CollectionNode => Parent;
    }
}
