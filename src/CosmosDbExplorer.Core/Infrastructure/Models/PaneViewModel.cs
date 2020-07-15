using System.Collections.ObjectModel;
using System.Windows.Media;
using CosmosDbExplorer.Messages;
using CosmosDbExplorer.Services;
using Microsoft.Azure.Documents;
using Prism.Events;
using PropertyChanged;

namespace CosmosDbExplorer.Infrastructure.Models
{
    public abstract class PaneViewModelBase : UIViewModelBase
    {
        private RelayCommand _closeCommand;
        private readonly StatusBarItem _pathStatusBarItem;
        private readonly IUIServices _uiServices;

        protected PaneViewModelBase(IEventAggregator messenger, IUIServices uiServices)
            : base(messenger, uiServices)
        {
            _pathStatusBarItem = new StatusBarItem(new StatusBarItemContext { Value = ToolTip, IsVisible = true }, StatusBarItemType.SimpleText, "Path", System.Windows.Controls.Dock.Left);
            StatusBarItems.Add(_pathStatusBarItem);
            _uiServices = uiServices;
        }

        [DoNotSetChanged]
        public string Title { get; set; }

        [DoNotSetChanged]
        public string ToolTip { get; set; }

        public virtual void OnToolTipChanged()
        {
            _pathStatusBarItem.DataContext.Value = ToolTip;
        }

        [DoNotSetChanged]
        public string Header { get; set; }

        [DoNotSetChanged]
        public string ContentId { get; protected set; }

        [DoNotSetChanged]
        public bool IsSelected { get; set; }

        [DoNotSetChanged]
        public bool IsActive { get; set; }

        [DoNotSetChanged]
        public bool IsClosed { get; set; }

        public virtual void OnIsActiveChanged()
        {
            //DispatcherHelper.RunAsync(() => MessengerInstance.Send(new ActivePaneChangedMessage(this)));
            MessengerInstance.GetEvent<ActivePaneChangedMessage>().Publish(this);
        }

        [DoNotSetChanged]
        public ObservableCollection<StatusBarItem> StatusBarItems { get; protected set; } = new ObservableCollection<StatusBarItem>();

        [DoNotSetChanged]
        public object IconSource { get; set; }

        [DoNotSetChanged]
        public Color? AccentColor { get; set; }

        public RelayCommand CloseCommand
        {
            get
            {
                return _closeCommand
                    ?? (_closeCommand = new RelayCommand(OnClose, CanClose));
            }
        }

        protected virtual bool CanClose()
        {
            return !IsClosed;
        }

        protected virtual void OnClose()
        {
            MessengerInstance.GetEvent<CloseDocumentMessage>().Publish(ContentId);
            Cleanup();
            IsClosed = true;
        }
    }

    public abstract class PaneViewModel<TNodeViewModel> : PaneViewModelBase
        where TNodeViewModel : TreeViewItemViewModel
    {
        protected PaneViewModel(IEventAggregator messenger, IUIServices uiServices)
            : base(messenger, uiServices)
        {

        }

        public abstract void Load(string contentId, TNodeViewModel node, Connection connection, DocumentCollection collection);
    }

    public abstract class PaneWithZoomViewModel<TNodeViewModel> : PaneViewModel<TNodeViewModel>
        where TNodeViewModel : TreeViewItemViewModel
    {
        protected PaneWithZoomViewModel(IEventAggregator messenger, IUIServices uiServices)
            : base(messenger, uiServices)
        {
            StatusBarItems.Add(new StatusBarItem(new StatusBarItemContext { Value = this, IsVisible = true }, StatusBarItemType.Zoom, "Zoom", System.Windows.Controls.Dock.Right));
        }

        [DoNotSetChanged]
        public double Zoom { get; set; } = 0.5;
    }

    public abstract class ToolViewModel : PaneViewModelBase
    {
        protected ToolViewModel(IEventAggregator messenger, IUIServices uiServices)
            : base(messenger, uiServices)
        {
        }

        [DoNotSetChanged]
        public bool IsVisible { get; set; }
    }
}
