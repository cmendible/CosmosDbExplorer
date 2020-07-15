using CosmosDbExplorer.Services;
using CosmosDbExplorer.ViewModel;
using Prism.Events;
using PropertyChanged;

namespace CosmosDbExplorer.Infrastructure.Models
{

    public abstract class UIViewModelBase : ViewModelBase
    {
        private readonly IUIServices _uiServices;

        protected UIViewModelBase(IEventAggregator messenger, IUIServices uiServices)
            : base(messenger)
        {
            _uiServices = uiServices;
        }

        [DoNotSetChanged]
        public bool IsBusy { get; set; }

        protected virtual void OnIsBusyChanged()
        {
            _uiServices.SetBusyState(IsBusy);
        }
    }
}
