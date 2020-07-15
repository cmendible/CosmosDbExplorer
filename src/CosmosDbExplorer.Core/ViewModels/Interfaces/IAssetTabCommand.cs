using CosmosDbExplorer.Infrastructure;

namespace CosmosDbExplorer.ViewModels.Interfaces
{
    public interface IAssetTabCommand
    {
        RelayCommand SaveCommand { get;  } 
        RelayCommand DiscardCommand { get; }
        RelayCommand DeleteCommand { get;  }
    }
}
