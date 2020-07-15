using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Timers;
using CommonServiceLocator;
using CosmosDbExplorer.Infrastructure;
using CosmosDbExplorer.Infrastructure.Models;
using CosmosDbExplorer.Messages;
using CosmosDbExplorer.Services;
using CosmosDbExplorer.ViewModels.Assets;
using CosmosDbExplorer.ViewModels.Interfaces;
using Prism.Events;

namespace CosmosDbExplorer.ViewModels
{

    /// <summary>
    /// This class contains properties that the main View can data bind to.
    /// <para>
    /// Use the <strong>mvvminpc</strong> snippet to add bindable properties to this ViewModel.
    /// </para>
    /// <para>
    /// You can also use Blend to data bind with the tool's support.
    /// </para>
    /// <para>
    /// See http://www.galasoft.ch/mvvm
    /// </para>
    /// </summary>
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IDialogService _dialogService;
        private readonly DatabaseViewModel _databaseViewModel;
        private RelayCommand _showAboutCommand;
        private RelayCommand _showAccountSettingsCommand;
        private RelayCommand _exitCommand;
        private IEnumerable<ToolViewModel> _tools;
        private RelayCommand _refreshCommand;

        public event Action RequestClose;

        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        /// <param name="dialogService"></param>
        /// <param name="messenger"></param>
        /// <param name="ioc"></param>
        public MainWindowViewModel(IDialogService dialogService, DatabaseViewModel databaseViewModel, IEventAggregator messenger/*, ISimpleIoc ioc*/)
            : base(messenger)
        {

            // Code runs "for real"
            var assembly = Assembly.GetEntryAssembly();
            Title = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(assembly, typeof(AssemblyTitleAttribute), false))?.Title ?? "error retriving assembly title";

            _dialogService = dialogService;
            //_ioc = ioc;
            //_databaseViewModel = _ioc.GetInstance<DatabaseViewModel>();
            _databaseViewModel = databaseViewModel;
            Tabs = new ObservableCollection<PaneViewModelBase>();

            SpyUsedMemory();

            RegisterMessages();
        }

        private void SpyUsedMemory()
        {
            var timer = new Timer(TimeSpan.FromSeconds(3).TotalMilliseconds);
            timer.Elapsed += (s, e) => RaisePropertyChanged(() => UsedMemory);
            timer.Start();
        }



        private void RegisterMessages()
        {
            // TODO: Replace MessengerInstance
            MessengerInstance.GetEvent<ActivePaneChangedMessage>().Subscribe(msg => OnActivePaneChanged(msg));


            MessengerInstance.GetEvent<OpenDocumentsViewMessage>().Subscribe(msg => OpenOrSelectTab<DocumentsTabViewModel, DocumentNodeViewModel>(msg));
            MessengerInstance.GetEvent<OpenQueryViewMessage>().Subscribe(msg => OpenOrSelectTab<QueryEditorViewModel, CollectionNodeViewModel>(msg));
            MessengerInstance.GetEvent<OpenImportDocumentViewMessage>().Subscribe(msg => OpenOrSelectTab<ImportDocumentViewModel, CollectionNodeViewModel>(msg));
            MessengerInstance.GetEvent<OpenDatabaseScaleViewMessage>().Subscribe(msg => OpenOrSelectTab<DatabaseScaleTabViewModel, DatabaseScaleNodeViewModel>(msg));
            MessengerInstance.GetEvent<OpenScaleAndSettingsViewMessage>().Subscribe(msg => OpenOrSelectTab<ScaleAndSettingsTabViewModel, ScaleSettingsNodeViewModel>(msg));
            MessengerInstance.GetEvent<EditUserMessage>().Subscribe(msg => OpenOrSelectTab<UserEditViewModel, UserNodeViewModel>(msg));
            MessengerInstance.GetEvent<EditPermissionMessage>().Subscribe(msg => OpenOrSelectTab<PermissionEditViewModel, PermissionNodeViewModel>(msg));
            MessengerInstance.GetEvent<OpenCollectionMetricsViewMessage>().Subscribe(msg => OpenOrSelectTab<CollectionMetricsTabViewModel, CollectionMetricsNodeViewModel>(msg));

            MessengerInstance.GetEvent<EditStoredProcedureMessage>().Subscribe(msg => OpenOrSelectTab<StoredProcedureTabViewModel, StoredProcedureNodeViewModel>(msg));
            MessengerInstance.GetEvent<EditUserDefFuncMessage>().Subscribe(msg => OpenOrSelectTab<UserDefFuncTabViewModel, UserDefFuncNodeViewModel>(msg));
            MessengerInstance.GetEvent<EditTriggerMessage>().Subscribe(msg => OpenOrSelectTab<TriggerTabViewModel, TriggerNodeViewModel>(msg));

            MessengerInstance.GetEvent<TreeNodeSelectedMessage>().Subscribe(msg => OnTreeNodeSelected(msg));
            MessengerInstance.GetEvent<CloseDocumentMessage>().Subscribe(msg => CloseDocument(msg), ThreadOption.UIThread);
            MessengerInstance.GetEvent<IsBusyMessage>().Subscribe(msg => IsBusy = msg);
        }

        private void OnActivePaneChanged(ViewModelBase paneViewModel)
        {
            if (paneViewModel is DatabaseViewModel)
            {
                IsRequestOptionsVisible = false;
                IsConnectionOptionsVisible = true;
                SelectedRibbonTab = 1;
            }
            else
            {
                IsConnectionOptionsVisible = ShouldConnectionOptionBeVisible();
                OnSelectedTabChanged();
                SelectedRibbonTab = 0;
            }
        }

        private void OnTreeNodeSelected(TreeViewItemViewModel item)
        {
            CanRefreshNodeViewModel = item as ICanRefreshNode;
            Connection = item as ConnectionNodeViewModel;
            Database = item as DatabaseNodeViewModel;
            Collection = (item as IHaveCollectionNodeViewModel)?.CollectionNode;
            Users = item as UsersNodeViewModel;
            UserNode = item as UserNodeViewModel;
            CanEditDelete = item as ICanEditDelete;

            IsConnectionOptionsVisible = ShouldConnectionOptionBeVisible();
        }

        private bool ShouldConnectionOptionBeVisible()
        {
            return CanRefreshNodeViewModel != null
                                    || Connection != null
                                    || Database != null
                                    || Collection != null
                                    || CanEditDelete != null
                                    || Users != null
                                    || UserNode != null;
        }

        private void OpenOrSelectTab<TTabViewModel, TNodeViewModel>(OpenTabMessageBase<TNodeViewModel> message)
            where TTabViewModel : PaneViewModel<TNodeViewModel>
            where TNodeViewModel : TreeViewItemViewModel, IContent
        {
            var contentId = message.Node?.ContentId ?? Guid.NewGuid().ToString();
            var tab = Tabs.FirstOrDefault(t => t.ContentId == contentId);

            if (tab != null)
            {
                SelectedTab = tab;
            }
            else
            {
                //var content = ServiceLocator.Current.GetInstanceWithoutCaching<TTabViewModel>(contentId); //_ioc.GetInstance<TTabViewModel>(contentId);
                var content = ServiceLocator.Current.GetInstance<TTabViewModel>(contentId); //_ioc.GetInstance<TTabViewModel>(contentId);
                content.Load(contentId, message.Node, message.Connection, message.Collection);

                Tabs.Add(content);
                SelectedTab = content;
            }
        }

        private void CloseDocument(string contentId)
        {
            var vm = Tabs.FirstOrDefault(t => t.ContentId == contentId);

            if (vm != null)
            {
                Tabs.Remove(vm);
                //_ioc.Unregister(vm);
                vm = null;
                SelectedTab = Tabs.LastOrDefault();
            }
        }

        public string Title { get; set; }

        public long UsedMemory => GC.GetTotalMemory(true) / 1014;

        public bool IsBusy { get; set; }

        public double Zoom { get; set; }

        public ObservableCollection<PaneViewModelBase> Tabs { get; }

        public IEnumerable<ToolViewModel> Tools
        {
            get
            {
                return _tools
                     ?? (_tools = new ToolViewModel[] { _databaseViewModel });
            }
        }

        public PaneViewModelBase SelectedTab { get; set; }

        public void OnSelectedTabChanged()
        {
            IsTabDocumentsVisible = SelectedTab is DocumentsTabViewModel;
            IsSettingsTabVisible = SelectedTab is ScaleAndSettingsTabViewModel || SelectedTab is DatabaseScaleTabViewModel;
            IsAssetTabVisible = SelectedTab is IAssetTabCommand;
            IsQueryTabVisible = SelectedTab is QueryEditorViewModel || SelectedTab is StoredProcedureTabViewModel ;
            IsImportTabVisible = SelectedTab is ImportDocumentViewModel;
            IsQuerySettingsVisible = SelectedTab is IHaveQuerySettings;
            IsSystemPropertiesVisible = SelectedTab is IHaveSystemProperties;
            IsRequestOptionsVisible = SelectedTab is IHaveRequestOptions;
            IsConnectionOptionsVisible = false; // Only visible when selecting a tab
            IsRefreshTabVisible = SelectedTab is ICanRefreshTab;
        }

        public int SelectedRibbonTab { get; set; }
        public bool IsConnectionOptionsVisible { get; set; }
        public bool IsTabDocumentsVisible { get; set; }
        public bool IsSettingsTabVisible { get; set; }
        public bool IsAssetTabVisible { get; set; }
        public bool IsQueryTabVisible { get; set; }
        public bool IsImportTabVisible { get; set; }
        public bool IsQuerySettingsVisible { get; set; }
        public bool IsRequestOptionsVisible { get; set; }
        public bool IsRefreshTabVisible { get; set; }
        public bool IsSystemPropertiesVisible { get; set; }

        public ConnectionNodeViewModel Connection { get; set; }
        public DatabaseNodeViewModel Database { get; set; }
        public CollectionNodeViewModel Collection { get; set; }
        public UsersNodeViewModel Users { get; set; }
        public UserNodeViewModel UserNode { get; set; }
        public ICanRefreshNode CanRefreshNodeViewModel { get; set; }
        public ICanEditDelete CanEditDelete { get; set; }

        public RelayCommand ShowAboutCommand
        {
            get
            {
                return _showAboutCommand
                    ?? (_showAboutCommand = new RelayCommand(
                    async () =>
                    {
                        var fvi = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location);
                        var name = ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false))?.Title ?? "Unknown Title";
                        await _dialogService.ShowMessageBox($"{name}\nVersion {fvi.FileVersion}", "About").ConfigureAwait(false);
                    }));
            }
        }

        public RelayCommand ShowAccountSettingsCommand
        {
            get
            {
                return _showAccountSettingsCommand
                    ?? (_showAccountSettingsCommand = new RelayCommand(
                    () =>
                    {
                        var form = new Views.AccountSettingsView();
                        var vm = (AccountSettingsViewModel)form.DataContext;
                        vm.SetConnection(new Connection(Guid.NewGuid()));

                        var result = form.ShowDialog();
                    }));
            }
        }

        public RelayCommand ExitCommand
        {
            get
            {
                return _exitCommand
                    ?? (_exitCommand = new RelayCommand(Close));
            }
        }

        public RelayCommand RefreshCommand
        {
            get
            {
                return _refreshCommand
                    ?? (_refreshCommand = new RelayCommand(
                        () => CanRefreshNodeViewModel.RefreshCommand.Execute(null),
                        () => CanRefreshNodeViewModel?.RefreshCommand.CanExecute(null) == true
                        ));
            }
        }

        public virtual void Close()
        {
            RequestClose?.Invoke();
        }
    }
}
