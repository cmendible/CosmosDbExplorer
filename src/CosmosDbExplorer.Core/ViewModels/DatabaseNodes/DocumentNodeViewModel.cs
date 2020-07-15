using System;
using CosmosDbExplorer.Infrastructure;
using CosmosDbExplorer.Infrastructure.Models;
using CosmosDbExplorer.Messages;

namespace CosmosDbExplorer.ViewModels
{
    public class DocumentNodeViewModel : TreeViewItemViewModel<CollectionNodeViewModel>, IHaveCollectionNodeViewModel, IContent
    {
        private RelayCommand _openDocumentCommand;

        public DocumentNodeViewModel(CollectionNodeViewModel parent)
            : base(parent, parent.MessengerInstance, false)
        {
            Name = "Documents";
        }

        public string Name { get; set; }

        public RelayCommand OpenDocumentCommand
        {
            get
            {
                return _openDocumentCommand
                    ?? (_openDocumentCommand = new RelayCommand(
                        () => {
                            IsSelected = false;
                            MessengerInstance.GetEvent<OpenDocumentsViewMessage>().Publish(new OpenTabMessageBase<DocumentNodeViewModel>(this, Parent.Parent.Parent.Connection, Parent.Collection));
                        }));
            }
        }

        public CollectionNodeViewModel CollectionNode => Parent;

        public string ContentId => Parent.Collection.SelfLink + "/Documents";
    }
}
