using CosmosDbExplorer.Infrastructure;

namespace CosmosDbExplorer.ViewModels.Interfaces
{
    public interface ICanRefreshTab
    {
        RelayCommand RefreshCommand { get; }
    }
}
