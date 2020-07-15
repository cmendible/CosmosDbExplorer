﻿using System.Threading.Tasks;
using CosmosDbExplorer.Infrastructure;
using CosmosDbExplorer.Services;
using Microsoft.Azure.Documents;
using Prism.Events;

namespace CosmosDbExplorer.ViewModels.Assets
{
    public class UserDefFuncTabViewModel : AssetTabViewModelBase<UserDefFuncNodeViewModel, UserDefinedFunction>
    {
        public UserDefFuncTabViewModel(IEventAggregator messenger, IDialogService dialogService, IDocumentDbService dbService, IUIServices uiServices)
            : base(messenger, dialogService, dbService, uiServices)
        {
        }

        protected override string GetDefaultHeader() { return "New User Definied Function"; }
        protected override string GetDefaultTitle() { return "User Definied Function"; }
        protected override string GetDefaultContent() { return Constants.Default.UserDefiniedFunction; }

        protected override void SetInformationImpl(UserDefinedFunction resource)
        {
            SetText(resource.Body);
        }

        protected override Task<UserDefinedFunction> SaveAsyncImpl(IDocumentDbService dbService)
        {
            return dbService.SaveUdfAsync(Connection, Collection, Id, Content.Text, AltLink);
        }

        protected override Task DeleteAsyncImpl(IDocumentDbService dbService)
        {
            return dbService.DeleteUdfAsync(Connection, AltLink);
        }
    }
}