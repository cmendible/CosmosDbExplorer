using CosmosDbExplorer.Infrastructure;

namespace CosmosDbExplorer.ViewModels
{
    public interface ICanRefreshNode
    {
        RelayCommand RefreshCommand { get; }
    }
}
