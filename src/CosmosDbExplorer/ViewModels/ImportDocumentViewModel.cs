﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CosmosDbExplorer.Contracts.Services;
using CosmosDbExplorer.Contracts.ViewModels;
using CosmosDbExplorer.Core.Models;
using CosmosDbExplorer.Core.Services;
using CosmosDbExplorer.Models;
using CosmosDbExplorer.Services.DialogSettings;
using CosmosDbExplorer.ViewModels.DatabaseNodes;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Mvvm.Input;

namespace CosmosDbExplorer.ViewModels
{
    public class ImportDocumentViewModel : PaneWithZoomViewModel<ContainerNodeViewModel>
    {
        private readonly IDialogService _dialogService;
        private readonly StatusBarItem _progessBarStatusBarItem;
        private readonly CosmosDocumentService _cosmosDocumentService;

        private AsyncRelayCommand? _executeCommand;
        private RelayCommand? _openFileCommand;
        private RelayCommand? _resetRequestOptionsCommand;
        private RelayCommand? _cancelCommand;

        private CancellationTokenSource? _cancellationToken;

        public ImportDocumentViewModel(IServiceProvider serviceProvider, IDialogService dialogService, IUIServices uiServices, string contentId, NodeContext<ContainerNodeViewModel> nodeContext)
            : base(uiServices, contentId, nodeContext)
        {
            _dialogService = dialogService;

            _progessBarStatusBarItem = new StatusBarItem(new StatusBarItemContextCancellableCommand { Value = CancelCommand, IsVisible = IsRunning, IsCancellable = false }, StatusBarItemType.ProgessBar, "Progress", System.Windows.Controls.Dock.Left);
            StatusBarItems.Add(_progessBarStatusBarItem);

            if (nodeContext.Node is null || nodeContext.Connection is null || nodeContext.Container is null || nodeContext.Database is null)
            {
                throw new NullReferenceException("Node context is not correctly initialized!");
            }

            ContentId = Guid.NewGuid().ToString();
            Node = nodeContext.Node;
            Header = "Import";
            Connection = nodeContext.Connection;
            Container = nodeContext.Container;

            //var split = Container.SelfLink.Split(new char[] { '/' });
            ToolTip = $"{Connection.Label}/{nodeContext.Database.Id}/{Container.Id}";
            AccentColor = Connection.AccentColor;

            _cosmosDocumentService = ActivatorUtilities.CreateInstance<CosmosDocumentService>(serviceProvider, Connection, nodeContext.Database, Container);
        }

        public override Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public bool IsRunning { get; set; }

        public void OnIsRunningChanged()
        {
            _progessBarStatusBarItem.DataContext.IsVisible = IsRunning;

            if (IsRunning)
            {
                _cancellationToken = new CancellationTokenSource();
            }
            else
            {
                _cancellationToken = null;
            }
        }

        public ContainerNodeViewModel Node { get; protected set; }

        protected CosmosConnection Connection { get; set; }

        protected CosmosContainer Container { get; set; }

        public string? Content { get; set; }

        protected void OnContentChanged()
        {
            ExecuteCommand.NotifyCanExecuteChanged();
        }

        public bool IsDirty { get; set; }

        public AsyncRelayCommand ExecuteCommand => _executeCommand ??= new(ExecuteCommandAsync, () => !IsRunning && !string.IsNullOrEmpty(Content));

        private async Task ExecuteCommandAsync()
        {
            if (string.IsNullOrEmpty(Content))
            {
                return;
            }

            try
            {
                IsRunning = true;
                var count = await _cosmosDocumentService.ImportDocumentsAsync(Content, _cancellationToken?.Token ?? new CancellationToken());
                await _dialogService.ShowMessage($"{count} document(s) imported!", "Import");
            }
            catch (OperationCanceledException)
            {
                await _dialogService.ShowMessage("The Import operation has been cancelled!", "Operation Cancelled");
            }
            catch (Exception ex)
            {
                await _dialogService.ShowError(ex, "Error durring document import!");
            }
            finally
            {
                IsRunning = false;
            }
        }


        public RelayCommand CancelCommand => _cancelCommand ??= new(() => _cancellationToken?.Cancel(), () => IsRunning);

        public RelayCommand OpenFileCommand => _openFileCommand ??= new(OpenFileCommandExecuteAsync);

        private void OpenFileCommandExecuteAsync()
        {
            var settings = new OpenFileDialogSettings
            {
                DefaultExt = "json",
                Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*",
                AddExtension = true,
                CheckFileExists = true,
                Multiselect = false,
                Title = "Open document"
            };

            async void OnDialogClose(bool confirm, FileDialogResult result)
            {
                if (!confirm)
                {
                    return;
                }

                try
                {
                    using var reader = File.OpenText(result.FileName);
                    //Content.FileName = result.FileName;
                    //Content.Text = await reader.ReadToEndAsync().ConfigureAwait(true);
                    Content = await reader.ReadToEndAsync();
                }
                catch (Exception ex)
                {
                    await _dialogService.ShowError(ex, $"Error during container {Container.Id} deletion");
                }
            }

            _dialogService.ShowOpenFileDialog(settings, OnDialogClose);
        }

        //public IndexingDirective? IndexingDirective { get; set; }
        //public ConsistencyLevel? ConsistencyLevel { get; set; }
        public string? PartitionKeyValue { get; set; }
        //public AccessConditionType? AccessConditionType { get; set; }
        public string? AccessCondition { get; set; }
        public string? PreTrigger { get; set; }
        public string? PostTrigger { get; set; }

        public RelayCommand ResetRequestOptionsCommand => _resetRequestOptionsCommand ??= new(ResetRequestOptionsCommandExecute);

        private void ResetRequestOptionsCommandExecute()
        {
            //IndexingDirective = null;
            //ConsistencyLevel = null;
            PartitionKeyValue = null;
            //AccessConditionType = null;
            AccessCondition = null;
            PreTrigger = null;
            PostTrigger = null;
        }
    }
}

