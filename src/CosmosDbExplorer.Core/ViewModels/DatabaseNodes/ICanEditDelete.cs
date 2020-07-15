using System.Windows.Input;
using CosmosDbExplorer.Infrastructure;

namespace CosmosDbExplorer.ViewModels
{
    public interface ICanEditDelete
    {
        RelayCommand EditCommand { get; }

        RelayCommand DeleteCommand { get; }
    }
}
