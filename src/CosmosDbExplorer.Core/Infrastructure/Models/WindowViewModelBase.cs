using System;
using CosmosDbExplorer.Services;
using Prism.Events;

namespace CosmosDbExplorer.Infrastructure.Models
{
    public abstract class WindowViewModelBase : UIViewModelBase
    {
        public event Action RequestClose;

        protected WindowViewModelBase(IEventAggregator messenger, IUIServices uiServices) 
            : base(messenger, uiServices)
        {
        }

        public virtual void Close()
        {
            RequestClose?.Invoke();
        }
    }
}
