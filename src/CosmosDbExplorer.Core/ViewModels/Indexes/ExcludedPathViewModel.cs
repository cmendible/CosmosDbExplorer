using System.ComponentModel;
using CosmosDbExplorer.Infrastructure;
using FluentValidation;
using Microsoft.Azure.Documents;
using Prism.Mvvm;
using Validar;

namespace CosmosDbExplorer.ViewModels.Indexes
{
    [InjectValidation]
    public class ExcludedPathViewModel : BindableBase, System.IEquatable<ExcludedPathViewModel>
    {
        public ExcludedPathViewModel()
        {
            ExcludedPath = new ExcludedPath();
        }

        public ExcludedPathViewModel(ExcludedPath excludedPath)
        {
            ExcludedPath = excludedPath;
        }

        public ExcludedPath ExcludedPath { get; }

        public string Path
        {
            get => ExcludedPath.Path;
            set
            {
                ExcludedPath.Path = value;
                RaisePropertyChanged(nameof(Path));
            }
        }

        public bool HasErrors => ((INotifyDataErrorInfo)this).HasErrors;

        public bool Equals(ExcludedPathViewModel other)
        {
            if (other == null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return Path == other.Path;
        }

        public override int GetHashCode() => Path?.GetHashCode() ?? 0;
    }

    public class ExcludedPathViewModelValidator : AbstractValidator<ExcludedPathViewModel>
    {
        public ExcludedPathViewModelValidator()
        {
            RuleFor(x => x.Path)
                .NotEmpty()
                .Matches(Constants.Validation.PathRegex);
        }
    }
}
