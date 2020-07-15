using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using CosmosDbExplorer.Services;
using CosmosDbExplorer.ViewModel.Assets;
using System.Windows;
using Prism.Ioc;
using CosmosDbExplorer.ViewModel;
using CommonServiceLocator;
using Prism.DryIoc;

namespace CosmosDbExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Prism.DryIoc.PrismApplication
    {
        protected override Window CreateShell()
        {
            var w = Container.Resolve<MainWindow>();
            return w;
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ServiceLocator.SetLocatorProvider(() => new DryIocServiceLocatorAdapter(containerRegistry.GetContainer()));

            containerRegistry.Register<Services.IDialogService, DialogService>();
            //containerRegistry.Register<IMessenger, Messenger>();
            containerRegistry.Register<IDocumentDbService, DocumentDbService>();
            containerRegistry.Register<ISettingsService, SettingsService>();
            containerRegistry.Register<IUIServices, UIServices>();

            containerRegistry.Register<MainWindowViewModel>();
            containerRegistry.Register<DocumentEditorViewModel>();
            containerRegistry.Register<AccountSettingsViewModel>();
            containerRegistry.Register<DatabaseViewModel>();
            containerRegistry.Register<DocumentsTabViewModel>();
            containerRegistry.Register<QueryEditorViewModel>();
            containerRegistry.Register<JsonViewerViewModel>();
            containerRegistry.Register<HeaderEditorViewModel>();
            containerRegistry.Register<ImportDocumentViewModel>();
            containerRegistry.Register<AboutViewModel>();

            containerRegistry.Register<ConnectionNodeViewModel>();
            containerRegistry.Register<StoredProcedureTabViewModel>();
            containerRegistry.Register<UserDefFuncTabViewModel>();
            containerRegistry.Register<TriggerTabViewModel>();
            containerRegistry.Register<ScaleAndSettingsTabViewModel>();
            containerRegistry.Register<UserEditViewModel>();
            containerRegistry.Register<PermissionEditViewModel>();
            containerRegistry.Register<CollectionMetricsTabViewModel>();
            containerRegistry.Register<ThroughputViewModel>();

            containerRegistry.Register<AddCollectionViewModel>();
            containerRegistry.Register<DatabaseScaleTabViewModel>();
        }
    }
}
