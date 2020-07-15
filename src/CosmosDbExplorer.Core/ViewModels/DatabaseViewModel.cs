using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;
using CommonServiceLocator;
using CosmosDbExplorer.Infrastructure.Models;
using CosmosDbExplorer.Messages;
using CosmosDbExplorer.Services;
using GongSolutions.Wpf.DragDrop;
using Prism.Events;

namespace CosmosDbExplorer.ViewModels
{
    public class DatabaseViewModel : ToolViewModel, IDropTarget
    {
        private readonly IDocumentDbService _dbService;
        private readonly ISettingsService _settingsService;

        public DatabaseViewModel(IEventAggregator messenger, IDocumentDbService dbService, ISettingsService settingsService, IUIServices uiServices)
            : base(messenger, uiServices)
        {
            Header = "Connections";
            Title = Header;
            IsVisible = true;

            _dbService = dbService;
            _settingsService = settingsService;

            RegisterMessages();
        }

        private void RegisterMessages()
        {
            MessengerInstance.GetEvent<ConnectionSettingSavedMessage>().Subscribe(msg => OnConnectionSettingsSaved(msg));
            MessengerInstance.GetEvent<RemoveConnectionMessage>().Subscribe(msg => OnRemoveConnection(msg), ThreadOption.UIThread);
        }

        public ObservableCollection<ConnectionNodeViewModel> Nodes { get; private set; }

        public async Task LoadNodesAsync()
        {
            var connections = await _settingsService.GetConnectionsAsync().ConfigureAwait(false);
            var nodes = connections.Select(c =>
            {
                //var connection = ServiceLocator.Current.GetInstanceWithoutCaching<ConnectionNodeViewModel>();
                var connection = ServiceLocator.Current.GetInstance<ConnectionNodeViewModel>();
                connection.Connection = c.Value;

                return connection;
            });

            Nodes = new ObservableCollection<ConnectionNodeViewModel>(nodes);
        }

        private void OnRemoveConnection(Connection msg)
        {
            var node = Nodes.FirstOrDefault(n => n.Connection == msg);

            if (node != null)
            {
                //DispatcherHelper.RunAsync(() => Nodes.Remove(node));
                Nodes.Remove(node);
            }
        }

        private void OnConnectionSettingsSaved(Connection msg)
        {
            var node = Nodes.FirstOrDefault(n => n.Connection.Equals(msg));

            if (node != null)
            {
                node.Connection = msg;
            }
            else
            {
                var connection = ServiceLocator.Current.GetInstance<ConnectionNodeViewModel>();//.GetInstanceWithoutCaching<ConnectionNodeViewModel>();
                connection.Connection = msg;
                Nodes.Add(connection);
            }
        }

        public void DragOver(IDropInfo dropInfo)
        {
            if (dropInfo.Data != dropInfo.TargetItem && dropInfo.Data is ConnectionNodeViewModel sourceItem && dropInfo.TargetItem is ConnectionNodeViewModel)
            {
                dropInfo.DropTargetAdorner = DropTargetAdorners.Insert;
                dropInfo.Effects = System.Windows.DragDropEffects.Move;
            }
        }

        public async void Drop(IDropInfo dropInfo)
        {
            var sourceItem = dropInfo.Data as ConnectionNodeViewModel;
            var targetItem = dropInfo.TargetItem as ConnectionNodeViewModel;

            if (sourceItem == targetItem)
            {
                return;
            }

            var sourceIndex = Nodes.IndexOf(sourceItem);
            var targetIndex = Nodes.IndexOf(targetItem);

            switch (dropInfo.InsertPosition)
            {
                case RelativeInsertPosition.None:
                    return;
                case RelativeInsertPosition.BeforeTargetItem:
                    if (sourceIndex + 1 == targetIndex)
                    {
                        return;
                    }
                    else if (targetIndex != 0)
                    {
                        targetIndex--;
                    }
                    break;
                case RelativeInsertPosition.AfterTargetItem:
                    if (sourceIndex - 1 == targetIndex)
                    {
                        return;
                    }
                    else if (targetIndex == Nodes.Count)
                    {
                        targetIndex--;
                    }
                    break;
                case RelativeInsertPosition.TargetItemCenter:
                    return;
            }

            Nodes.Move(sourceIndex, targetIndex);
            await _settingsService.ReorderConnections(sourceIndex, targetIndex).ConfigureAwait(false);
        }
    }
}
